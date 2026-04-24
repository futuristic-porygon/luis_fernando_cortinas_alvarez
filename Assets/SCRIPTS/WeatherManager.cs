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
    [SerializeField] private WeatherData[] weatherDatas;
    [SerializeField] private UnitType unitType;
    [SerializeField] private Volume globalvolume;
    [SerializeField] private TextMeshProUGUI cityText;
    //[SerializeField] private Light light;

    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;
    private FilmGrain filmGrain;
    private Vignette vignette;

    public string apiURL;
    private string appID = "08ec0161715e187c93ac6af67f6c9ac3";

    private string rawJson;

    void Start()
    {

        //globalvolume.profile.TryGet(out colorAdjustments);
        if (!globalvolume.profile.TryGet(out colorAdjustments))
            Debug.LogError("Falta ColorAdjustments");

        if (!globalvolume.profile.TryGet(out chromaticAberration))
            Debug.LogError("Falta ChromaticAberration");

        if (!globalvolume.profile.TryGet(out filmGrain))
            Debug.LogError("Falta FilmGrain");

        if (!globalvolume.profile.TryGet(out vignette))
            Debug.LogError("Falta Vignette en el Volumen");

        globalvolume.profile.TryGet(out chromaticAberration);

        StartCoroutine(RetrieveWeatherData());
    }

    public void SetURL(WeatherData data)
    {
        apiURL = "https://api.openweathermap.org/data/2.5/weather?" +
            $"lat={data.latitude}&" +
            $"lon={data.longitude}&" +
            $"appid={appID}&" +
            $"units={(unitType == UnitType.Standard ? "standard" : unitType == UnitType.Imperial ? "imperial" : "metric")}";
    }

    IEnumerator RetrieveWeatherData()
    {
        while (true)
        {

            int randomIndex = UnityEngine.Random.Range(0, weatherDatas.Length);
            WeatherData selectedData = weatherDatas[randomIndex];

            SetURL(selectedData);

            UnityWebRequest request = new UnityWebRequest(apiURL);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Clima de: " + selectedData.location);

                rawJson = request.downloadHandler.text;
                DecodeJson(randomIndex);
            }

            yield return new WaitForSeconds(15f);
        }
    }

    private void DecodeJson(int index)
    {
        var currentWeather = JSON.Parse(rawJson);

        // 1. ASIGNACIÓN DIRECTA (Sin efecto de escritura)
        if (cityText != null)
        {
            cityText.text = "Clima de: " + weatherDatas[index].location;
        }

        weatherDatas[index].timezone = currentWeather["timezone"]?.Value ?? "Unknown";

        if (currentWeather["main"] != null && currentWeather["main"]["temp"] != null)
        {
            string tempValue = currentWeather["main"]["temp"].Value;
            if (float.TryParse(tempValue, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out float temperature))
            {
                weatherDatas[index].temp = temperature;
            }
            else
            {
                Debug.LogWarning("No se pudo convertir la temperatura: " + tempValue);
                weatherDatas[index].temp = 0;
            }
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

        if (weather.Contains("rain") || weather.Contains("drizzle") || weather.Contains("thunderstorm"))
        {
            // LLUVIA: Muy oscuro, sin colores (Saturación -50), mucha distorsión
            StartCoroutine(ChangeWeatherEffects(-2.0f, 1.0f, 0.8f, -50f, 0.45f));
        }
        else if (weather.Contains("cloud") || weather.Contains("mist") || weather.Contains("fog"))
        {
            // NUBLADO: Un poco gris, viñeta suave
            StartCoroutine(ChangeWeatherEffects(-1.0f, 0.4f, 0.2f, -20f, 0.3f));
        }
        else
        {
            // DESPEJADO: Colores vivos (+10), mucha luz, sin viñeta
            StartCoroutine(ChangeWeatherEffects(0.4f, 0.05f, 0.05f, 10f, 0.25f));
        }
    }
  
    IEnumerator ChangeWeatherEffects(float targetExposure, float targetChromatic, float targetGrain, float targetSat, float targetVignette)
    {
        float duration = 3f; // Un poco más lento para que sea cinemático
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
        public string location;
        public float latitude;
        public float longitude;

        public string timezone;
        public float temp;
        public string description;
    }

    public enum UnitType
    {
        Standard, Metric, Imperial
    }
}