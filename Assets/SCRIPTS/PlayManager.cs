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
    [SerializeField] private int passwordMin = 6;

    [Header("Register")]
    [SerializeField] private TMP_InputField CAEmailInput;
    [SerializeField] private TMP_InputField CAUsernameInput;
    [SerializeField] private TMP_InputField CAPasswordInput;

    [Header("Login")]
    [SerializeField] private TMP_InputField LogUsernameInput;
    [SerializeField] private TMP_InputField LogPasswordInput;

    [Header("Leaderboards")]
    [SerializeField] private int score;
    [SerializeField] private Transform rowsParent;
    [SerializeField] private GameObject rowPrefab;

    private string loggedInPlayerId;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
            PlayFabSettings.TitleId = "B3871";//el ID de mi playfab

        CheckSceneInitialization(SceneManager.GetActiveScene());
    }

    private void CheckSceneInitialization(Scene scene)
    //dependiendo de la escena es para que sepa que va a hacer. Si es Log in,busca los inputfields, y si es leaderboard, pues busca el scrollview
    {
        if (scene.name == "Leaderboards") GetLeaderboard();
        if (scene.name == "Login y Signin") AlCargarEscena(scene, LoadSceneMode.Single);
    }

    public void RegisterPlayer()//para sign in
    {
        if (CAPasswordInput.text.Length < passwordMin) return;

        var request = new RegisterPlayFabUserRequest()
        {
            Username = CAUsernameInput.text,
            Email = CAEmailInput.text,
            Password = CAPasswordInput.text,
            DisplayName = CAUsernameInput.text
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, result => {
            UpdateDisplayName(CAUsernameInput.text);
        }, OnError);
    }

    public void LoginWithPlayFab()//para el boton de login en la pantalla de log in
    {
        var request = new LoginWithPlayFabRequest()
        {
            Username = LogUsernameInput.text,
            Password = LogPasswordInput.text,
        };

        PlayFabClientAPI.LoginWithPlayFab(request, result => {
            loggedInPlayerId = result.PlayFabId;
            UpdateDisplayName(LogUsernameInput.text);
            SceneManager.LoadScene("Juego");
        }, OnError);
    }

    private void UpdateDisplayName(string newName)//para que se muestre el nombre y asi
    {
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = newName };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, null, null);
    }

    public void UpdateScoreFromGame(int incomingScore)
    {
        this.score = incomingScore;
        var request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate { StatisticName = "Score", Value = score }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, null, OnError);
    }

    public void GetLeaderboard()
    //para que el playmanager busque el leaderboard porque como es persistente entre escenas,
    //pues tiene que bscar las cosas automatico
    {
        if (rowsParent == null)
        {
            GameObject found = GameObject.Find("Scroll View/Viewport/Content") ?? GameObject.Find("Content");
            if (found != null) rowsParent = found.transform;
        }

        if (rowsParent == null) return;

        var request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10,
            ProfileConstraints = new PlayerProfileViewConstraints { ShowDisplayName = true }
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);
    }

    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        foreach (Transform item in rowsParent) Destroy(item.gameObject);

        foreach (var item in result.Leaderboard)
        {
            GameObject newRow = Instantiate(rowPrefab, rowsParent);
            LeaderboardRow rowScript = newRow.GetComponent<LeaderboardRow>();

            if (rowScript != null)
            {
                rowScript.Setup(
                    item.Position + 1,
                    item.Profile?.DisplayName ?? "Jugador Anónimo",//en caso de que no se registraran
                    item.StatValue,
                    item.PlayFabId == loggedInPlayerId
                );
            }
        }
    }

    public void OnError(PlayFabError error)
    {
        if (errorDisplay != null)
        {
            //este lo hice para qe se vieran los errores del playfab en la pantalla de inicio porque luego me confundia xd
            errorDisplay.text = "Error: " + error.ErrorMessage;
            CancelInvoke(nameof(ClearError));
            Invoke(nameof(ClearError), 5f);
        }
    }
    
    
    private void ClearError() => errorDisplay.text = "";

    private void OnEnable() => SceneManager.sceneLoaded += AlCargarEscena;
    private void OnDisable() => SceneManager.sceneLoaded -= AlCargarEscena;

    private void AlCargarEscena(Scene scene, LoadSceneMode mode)
    {
        //para cuando se pasa a la escena de leaderboards, que playermanager muestre la info en el scrollview
        if (scene.name == "Leaderboards") GetLeaderboard();
        else if (scene.name == "Login y Signin")
        {
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null) return;

            var inputs = canvas.GetComponentsInChildren<TMP_InputField>(true);
            foreach (var input in inputs)
            {
                switch (input.name)
                {
                    case "LogUsernameInput": LogUsernameInput = input; break;
                    case "LogPasswordInput": LogPasswordInput = input; break;
                    case "CAEmailInput": CAEmailInput = input; break;
                    case "CAUsernameInput": CAUsernameInput = input; break;
                    case "CAPasswordInput": CAPasswordInput = input; break;
                }
            }
        }
    }

    public void RestartGame()
    {
        //es para el boton del final del jego
        score = 0;
        SceneManager.LoadScene("Juego");
    }
}