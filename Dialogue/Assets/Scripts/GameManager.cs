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
        Gameplay
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
        // Controle de permissão por estado
        if (CurrentState == GameState.Iniciando ||
            CurrentState == GameState.MenuPrincipal ||
            CurrentState == GameState.Gameplay)
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
        LoadScene("MainMenu");
    }

    public void StartGame()
    {
        SetState(GameState.Gameplay);
        LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }
}