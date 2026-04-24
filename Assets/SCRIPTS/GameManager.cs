using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Puntaje")]
    private int kaorukosRecolectadas = 0;
    public TextMeshProUGUI Text;

    [Header("Temporizador")]
    public float tiempoRestante = 120f;
    public TextMeshProUGUI timerText;
    private bool juegoTerminado = false;

    [Header("Victoria/Final")]
    public GameObject panelVictoria;
    public float tiempoEspera = 3f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ContadorUI(); 
    }

    private void Update()
    {
        if (!juegoTerminado && tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            UpdateTimerUI();

            if (tiempoRestante <= 0)
            {
                FinalizarJuego();
            }
        }
    }

    public void AddKaoruko()
    {
        if (juegoTerminado) return;

        kaorukosRecolectadas++;
        ContadorUI();

       
    }

    void ContadorUI()
    {
        if (Text != null)
        {
            Text.text = "Kaorukos: " + kaorukosRecolectadas.ToString();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);
        }
    }

    void FinalizarJuego()
    {
        tiempoRestante = 0;
        juegoTerminado = true;
        StartCoroutine(ProcesoFinal());
    }

    IEnumerator ProcesoFinal()
    {        
        int puntajeFinal = kaorukosRecolectadas;

        if (PlayManager.Instance != null)
        {
            PlayManager.Instance.UpdateScoreFromGame(puntajeFinal);
        }

        if (panelVictoria != null)
            panelVictoria.SetActive(true);

        yield return new WaitForSeconds(tiempoEspera);

        SceneManager.LoadScene("Leaderboards");
    }
}