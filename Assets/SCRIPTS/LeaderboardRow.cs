using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardRow : MonoBehaviour //para el prefab del leaderboard
{
    public TextMeshProUGUI rankingNumero;
    public TextMeshProUGUI nombreJugador;
    public TextMeshProUGUI score;
    public GameObject icon; 

    public void Setup(int posicion, string nombre, int puntaje, bool esElJugadorActual)
    {
        rankingNumero.text = posicion.ToString();
        nombreJugador.text = nombre;
        score.text = puntaje.ToString();

        icon.SetActive(esElJugadorActual);
    }
}