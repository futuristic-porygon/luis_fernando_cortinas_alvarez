using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayManager : MonoBehaviour
{
    public static PlayManager Instance;

    [SerializeField] private TextMeshProUGUI errorDisplay;

    [Header("Register")]
    [SerializeField] private TMP_InputField CAEmailInput;
    [SerializeField] private TMP_InputField CAUsernameInput;
    [SerializeField] private TMP_InputField CAPasswordInput;

    [Header("Login")]
    [SerializeField] private TMP_InputField LogUsernameInput;
    [SerializeField] private TMP_InputField LogPasswordInput;
    private string loggedInPlayerId;

    [Header("UI Feedback")]// Arrastra el texto aquí

    [SerializeField] private int passwordMin;

    private string username;

    [Header("Leaderboards")]
    [SerializeField] private int score;
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform rowsParent;
    [SerializeField] private GameObject rowPrefab;



    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Importante: Esto le dice a Unity que no lo borre al cambiar de escena
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe uno (el que viene de la escena anterior), borramos el nuevo
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.DeveloperSecretKey))
            PlayFabSettings.DeveloperSecretKey = "438EGT8XTBT1DYYEM7FPPU14A4WY7MI4EZI6C7ZEGG51IJDOQ4";

        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "B3871";

        // Si iniciamos directamente en Leaderboards (por pruebas en el editor)
        if (SceneManager.GetActiveScene().name == "Leaderboards")
            GetLeaderboard();

        // Si iniciamos en Login, forzamos la primera conexión manualmente
        if (SceneManager.GetActiveScene().name == "Login y Signin")
            AlCargarEscena(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    // RegisterPlayer se va a encargar de registrar al usuario en playfab
    //Es publico porque se tiene que mandar a llamar desde un boton
    public void RegisterPlayer()
    {
        if (CAPasswordInput.text.Length < passwordMin)
        {

            Debug.Log($"Tu contrasena debe tener al menos {passwordMin} caracteres");
        }
        // Crea una peticion nueva para registrar a un usuario nuevo
        var request = new RegisterPlayFabUserRequest()
        {
            Username = CAUsernameInput.text,
            Email = CAEmailInput.text,
            Password = CAPasswordInput.text,

        };

        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterUserSucces, OnError);
    }

    public void RegisterUserSucces(RegisterPlayFabUserResult result)
    {

        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = CAUsernameInput.text
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            onSuccess => Debug.Log("¡Nombre de pantalla actualizado!"),
            OnError);

        Debug.Log("Usuario Registrado");
    }

    public void LoginWithPlayFab()
    {
        var request = new LoginWithPlayFabRequest()
        {
            Username = LogUsernameInput.text,
            Password = LogPasswordInput.text,
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        loggedInPlayerId = result.PlayFabId;

        var nameRequest = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = LogUsernameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(nameRequest, null, null);

        SceneManager.LoadScene("Juego");
    }

    [ContextMenu("Update Score")]
    public void UpdateScoreFromGame(int incomingScore)
    {
        this.score = incomingScore; // Asigna el puntaje calculado en el juego
        UpdateScore(); // Llama a la función que ya tenías para subirlo a la nube
    }
    public void UpdateScore()
    {
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<PlayFab.ClientModels.StatisticUpdate>()
        {
            new PlayFab.ClientModels.StatisticUpdate
            {
                StatisticName = "Score",
                Value = score
            }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnUpdateStatisticsSuccess, OnError);
    }


    public void GetLeaderboard()
    {
        // Si la referencia es nula, intentamos buscarla una última vez por ruta completa
        if (rowsParent == null)
        {
            GameObject found = GameObject.Find("Scroll View/Viewport/Content");
            if (found != null) rowsParent = found.transform;
        }

        if (rowsParent == null)
        {
            Debug.LogWarning("Esperando a que la escena cargue el Content...");
            return;
        }

        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10,
            ProfileConstraints = new PlayerProfileViewConstraints
            {
                ShowDisplayName = true,
                ShowAvatarUrl = true
            }
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);
    }
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log("¡Datos recibidos! Cantidad de filas: " + result.Leaderboard.Count); // <--- LOG 2

        if (rowsParent == null) return;

        foreach (Transform item in rowsParent) Destroy(item.gameObject);

        foreach (var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(rowPrefab, rowsParent);
            LeaderboardRow rowScript = newRow.GetComponent<LeaderboardRow>();

            if (rowScript != null)
            {
                bool JugadorLoggeado = (item.PlayFabId == loggedInPlayerId);

                // Obtenemos la URL de la imagen del perfil
                string urlImagen = item.Profile?.AvatarUrl;

                rowScript.Setup(
    item.Position + 1,
    // Accedemos a Profile para sacar el DisplayName
    (item.Profile != null && !string.IsNullOrEmpty(item.Profile.DisplayName))
        ? item.Profile.DisplayName
        : "Jugador Anónimo",
    item.StatValue,
    JugadorLoggeado
);
            }
        }
    }
    public void OnUpdateStatisticsSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Scoreboard subido a la nube con éxito");
        //GetLeaderboard();
    }
    public void OnError(PlayFabError error)
    {
        // Seguimos imprimiendo en consola para ti como desarrollador
        Debug.LogError(error.GenerateErrorReport());

        // Si tenemos la referencia del texto, mostramos el error al jugador
        if (errorDisplay != null)
        {
            // error.ErrorMessage es una descripción corta y legible del problema
            errorDisplay.text = "Error: " + error.ErrorMessage;

            // Opcional: Si quieres que el mensaje desaparezca después de unos segundos
            CancelInvoke("ClearError");
            Invoke("ClearError", 5f);
        }
    }

    // Función auxiliar para limpiar el mensaje
    private void ClearError()
    {
        if (errorDisplay != null) errorDisplay.text = "";
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
    }

    private void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Leaderboards")
        {
            GameObject foundContent = GameObject.Find("Content");
            if (foundContent != null) rowsParent = foundContent.transform;
            leaderboardPanel = GameObject.Find("Leaderboard");
            GetLeaderboard();
        }
        else if (scene.name == "Login y Signin")
        {
            // 1. Buscamos el Canvas
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null) return;

            // 2. Función mágica: GetComponentInChildren con el parámetro 'true' 
            // busca incluso en objetos DESACTIVADOS.
            TMP_InputField[] todosLosInputs = canvas.GetComponentsInChildren<TMP_InputField>(true);

            TMP_InputField lUser = null, lPass = null, rEmail = null, rUser = null, rPass = null;

            foreach (var input in todosLosInputs)
            {
                // Comparamos por el nombre exacto que tienen en tu jerarquía (según tus capturas)
                if (input.name == "LogUsernameInput") lUser = input;
                if (input.name == "LogPasswordInput") lPass = input;
                if (input.name == "CAEmailInput") rEmail = input;
                if (input.name == "CAUsernameInput") rUser = input;
                if (input.name == "CAPasswordInput") rPass = input;
            }

            // 3. Mandamos a reconectar
            ReconectarTodo(lUser, lPass, rEmail, rUser, rPass);

            Debug.Log("Reconexión exhaustiva completada (incluyendo objetos ocultos)");
        }
    }
    public void RestartGame()
    {
       
        this.score = 0;
        SceneManager.LoadScene("Juego");
    }

    public void ReconectarTodo(TMP_InputField logUser, TMP_InputField logPass, TMP_InputField regEmail, TMP_InputField regUser, TMP_InputField regPass)
    {
        LogUsernameInput = logUser;
        LogPasswordInput = logPass;

        CAEmailInput = regEmail;
        CAUsernameInput = regUser;
        CAPasswordInput = regPass;

        Debug.Log("¡Toda la UI de Login y Registro reconectada!");
    }
}