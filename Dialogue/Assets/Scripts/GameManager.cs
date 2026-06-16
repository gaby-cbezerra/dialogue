using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState
    {
        Iniciando,
        MenuPrincipal,
        CharacterSelect,
        Gameplay,
        Victory
    }

    public GameState CurrentState;

    [Header("Player Input")]
    public PlayerInput playerInput;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(GameState.Iniciando);
        LoadScene("Splash");
    }

    // =============================
    // STATE CONTROL
    // =============================
    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Estado atual: " + CurrentState);
    }

    // =============================
    // SCENE CONTROL
    // =============================
    public void LoadScene(string sceneName)
    {
        if (CurrentState == GameState.Iniciando ||
            CurrentState == GameState.MenuPrincipal ||
            CurrentState == GameState.CharacterSelect ||
            CurrentState == GameState.Gameplay ||
            CurrentState == GameState.Victory)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Mudança de cena não permitida no estado atual.");
        }
    }
    // =============================
    // INPUT CONTROL
    // =============================
    public void AssignPlayerInput(PlayerInput input)
    {
        playerInput = input;
        Debug.Log("Input atribuído ao jogador.");
    }

    // =============================
    // TRANSIÇÕES ESPECÍFICAS
    // =============================
    public void GoToMenu()
    {
        SetState(GameState.MenuPrincipal);

        // Remove GUI se estiver carregada
        if (SceneManager.GetSceneByName("GUI").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GUI");
        }

        LoadScene("MainMenu");
    }
    
    public void GoToCharacterSelect()
    {
        SetState(GameState.CharacterSelect);

        LoadScene("CharacterSelect");
    }

    public void StartMatch()
    {
        SetState(GameState.Gameplay);

        SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);

        SceneManager.LoadScene("GUI", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
    
    public void GoToVictory()
    {
        SetState(GameState.Victory);

        if (SceneManager.GetSceneByName("GUI").isLoaded)
        {
            SceneManager.UnloadSceneAsync("GUI");
        }

        LoadScene("Victory");
    }
}