using SimpleJSON;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WeatherManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private WeatherData[] weatherDatas;
    [SerializeField] private UnitType unitType;
    [SerializeField] private Volume globalvolume;
    [SerializeField] private TextMeshProUGUI cityText;

    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private FilmGrain filmGrain;
    private Vignette vignette;

    private string apiURL;
    private string appID = "08ec0161715e187c93ac6af67f6c9ac3";
    private string rawJson;

    void Start()
    {
        if (globalvolume.profile.TryGet(out colorAdjustments) &&
            globalvolume.profile.TryGet(out chromaticAberration) &&
            globalvolume.profile.TryGet(out filmGrain) &&
            globalvolume.profile.TryGet(out vignette))
        {
            StartCoroutine(RetrieveWeatherData());
        }
        else
        {
            Debug.LogError("Checa componentes Post Process");
        }
    }

    public void SetURL(WeatherData data)
    {
        string unit = unitType.ToString().ToLower();
        apiURL = $"https://api.openweathermap.org/data/2.5/weather?lat={data.latitude}&lon={data.longitude}&appid={appID}&units={unit}";
    }

    IEnumerator RetrieveWeatherData()
    {
        while (true)
        {
            //para sacar los datos de weatrhermap
            int randomIndex = UnityEngine.Random.Range(0, weatherDatas.Length);
            WeatherData selectedData = weatherDatas[randomIndex];

            SetURL(selectedData);

            using (UnityWebRequest request = UnityWebRequest.Get(apiURL))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    rawJson = request.downloadHandler.text;
                    DecodeJson(randomIndex);
                }
            }

            yield return new WaitForSeconds(15f);
        }
    }

    private void DecodeJson(int index)
    {
        var currentWeather = JSON.Parse(rawJson);

        if (cityText != null)
            //para mostrar la ciudad en el UI
            cityText.text = "Clima de: " + weatherDatas[index].location;

        weatherDatas[index].timezone = currentWeather["timezone"]?.Value ?? "Unknown";

        if (currentWeather["main"] != null && float.TryParse(currentWeather["main"]["temp"].Value,
            System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float temperature))
        {
            weatherDatas[index].temp = temperature;
        }

        if (currentWeather["weather"] != null && currentWeather["weather"][0] != null)
        {
            weatherDatas[index].description = currentWeather["weather"][0]["description"]?.Value ?? "clear";
        }

        ChangeWeather(weatherDatas[index]);
    }

    private void ChangeWeather(WeatherData data)
    {
        string weather = data.description.ToLower();
        //para cambiar los valores del post procesing dependiendo del clima de la cidad
        if (weather.Contains("rain") || weather.Contains("drizzle") || weather.Contains("thunderstorm"))
        {
            StartCoroutine(ChangeWeatherEffects(-2.0f, 1.0f, 0.8f, -50f, 0.45f));
        }
        else if (weather.Contains("cloud") || weather.Contains("mist") || weather.Contains("fog"))
        {
            StartCoroutine(ChangeWeatherEffects(-1.0f, 0.4f, 0.2f, -20f, 0.3f));
        }
        else
        {
            StartCoroutine(ChangeWeatherEffects(0.4f, 0.05f, 0.05f, 10f, 0.25f));
        }
    }

    IEnumerator ChangeWeatherEffects(float targetExposure, float targetChromatic, float targetGrain, float targetSat, float targetVignette)
    {
        float duration = 3f;
        float time = 0;

        float startExposure = colorAdjustments.postExposure.value;
        float startChromatic = chromaticAberration.intensity.value;
        float startGrain = filmGrain.intensity.value;
        float startSat = colorAdjustments.saturation.value;
        float startVignette = vignette.intensity.value;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            //los cosos de post processing
            colorAdjustments.postExposure.value = Mathf.Lerp(startExposure, targetExposure, t);
            chromaticAberration.intensity.value = Mathf.Lerp(startChromatic, targetChromatic, t);
            filmGrain.intensity.value = Mathf.Lerp(startGrain, targetGrain, t);
            colorAdjustments.saturation.value = Mathf.Lerp(startSat, targetSat, t);
            vignette.intensity.value = Mathf.Lerp(startVignette, targetVignette, t);

            yield return null;
        }
    }

    [Serializable]
    public struct WeatherData
    {
        //los datos del clima
        public string location;
        public float latitude;
        public float longitude;
        public string timezone;
        public float temp;
        public string description;
    }

    public enum UnitType { Standard, Metric, Imperial }
}