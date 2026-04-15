using System;
using System.Collections;
using UnityEngine;
using UEObject = UnityEngine.Object;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Initializing,
        MainMenu,
        Gameplay,
    }

    // Singleton instance
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find existing in scene (use newer API to avoid obsolete warning)
                _instance = UnityEngine.Object.FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    var go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    // Current state (read-only publicly)
    public GameState CurrentState { get; private set; } = GameState.Initializing;

    // Events
    public event Action<GameState, GameState> OnStateChanged; // (old, new)
    public event Action<string> OnSceneLoadStarted; // scene name
    public event Action<string> OnSceneLoadCompleted; // scene name

    // Loading protection
    public bool IsLoadingScene { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(true);

        // Start in Initializing state
        CurrentState = GameState.Initializing;
    }
    
    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "_Boot")
            return;

        RequestLoadSceneByName("Splash");
    }

    private void OnEnable()
    {
        // Ensure we relocate PlayerInput for the current scene and future scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Attempt relocation immediately in case a PlayerInput is already present
        RelocatePlayerInputInstances();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "_Boot":
                ChangeState(GameState.Initializing);
                break;

            case "MainMenu":
                ChangeState(GameState.MainMenu);
                break;

            case "SampleScene":
                ChangeState(GameState.Gameplay);
                break;
        }
    }

    /// <summary>
    /// Find PlayerInput instances in the loaded scenes and relocate the primary one
    /// to be a child of the GameManager and persist across scenes. Destroy duplicates.
    /// </summary>
    private void RelocatePlayerInputInstances()
    {
        try
        {
            var inputs = UEObject.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
            if (inputs == null || inputs.Length == 0)
                return;

            // Choose the first found as primary
            PlayerInput primary = inputs[0];

            // If primary is not already child of GameManager, reparent it
            if (primary.gameObject.transform.root != this.transform.root)
            {
                primary.transform.SetParent(this.transform, true);
                DontDestroyOnLoad(primary.gameObject);
                Debug.Log($"GameManager: Relocated PlayerInput '{primary.gameObject.name}' under GameManager and marked DontDestroyOnLoad.");
            }

            // Destroy any duplicates (other PlayerInput instances)
            for (int i = 1; i < inputs.Length; i++)
            {
                var dup = inputs[i];
                if (dup == null) continue;
                // If duplicate is the same GameObject as primary, skip
                if (dup.gameObject == primary.gameObject) continue;
                Debug.Log($"GameManager: Destroying duplicate PlayerInput on '{dup.gameObject.name}'.");
                UnityEngine.Object.Destroy(dup.gameObject);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Exception while relocating PlayerInput instances: {ex}");
        }
    }

    /// <summary>
    /// Change the current game state. This triggers OnStateChanged.
    /// </summary>
    public void ChangeState(GameState newState)
    {
        var old = CurrentState;

        Debug.Log($"Tentando mudar de {old} para {newState}");

        if (newState == CurrentState)
            return;

        Debug.Log($"GameManager: Estado mudando de {old} para {newState}");

        OnExitState(old);
        CurrentState = newState;
        OnEnterState(newState);
    }
    private void OnExitState(GameState _)
    {
        // Add state exit logic here if needed
        // For now keep lightweight
    }

    private void OnEnterState(GameState _)
    {
        // Add state enter logic here if needed
        // Example: could automatically load a scene when entering a state
    }

    /// <summary>
    /// Request the GameManager to load a scene by name. Returns false if a scene is already loading.
    /// This is the only supported way in this project to perform scene loads from game code.
    /// </summary>
    public bool RequestLoadSceneByName(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onComplete = null)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("GameManager.RequestLoadSceneByName: sceneName is null or empty.");
            return false;
        }

        if (IsLoadingScene)
        {
            Debug.LogWarning($"GameManager: Scene load for '{sceneName}' requested while another load is in progress.");
            return false;
        }

        StartCoroutine(LoadSceneCoroutine(sceneName, mode, onComplete));
        return true;
    }
    public void LoadSampleScene()
    {
        // RequestLoadSceneByName("SampleScene");
        
        ChangeState(GameState.Gameplay);
        
        SceneManager.LoadScene(3);
    }

    /// <summary>
    /// Request the GameManager to load a scene by build index.
    /// </summary>
    public bool RequestLoadSceneByIndex(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single, Action onComplete = null)
    {
        if (buildIndex < 0)
        {
            Debug.LogError("GameManager.RequestLoadSceneByIndex: buildIndex must be >= 0.");
            return false;
        }

        if (IsLoadingScene)
        {
            Debug.LogWarning($"GameManager: Scene load for index '{buildIndex}' requested while another load is in progress.");
            return false;
        }

        StartCoroutine(LoadSceneByIndexCoroutine(buildIndex, mode, onComplete));
        return true;
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode mode, Action onComplete)
    {
        IsLoadingScene = true;
        OnSceneLoadStarted?.Invoke(sceneName);

        AsyncOperation op;
        try
        {
            op = SceneManager.LoadSceneAsync(sceneName, mode);
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Exception while starting LoadSceneAsync('{sceneName}'): {ex}");
            IsLoadingScene = false;
            yield break;
        }

        if (op == null)
        {
            Debug.LogError($"GameManager: Failed to start async load for scene '{sceneName}'. Make sure the scene is added to Build Settings or the name is correct.");
            IsLoadingScene = false;
            yield break;
        }

        while (!op.isDone)
            yield return null;

        // Slight delay to ensure the scene's Awake/Start run before signalling completion
        yield return null;

        OnSceneLoadCompleted?.Invoke(sceneName);
        onComplete?.Invoke();
        IsLoadingScene = false;
    }

    private IEnumerator LoadSceneByIndexCoroutine(int buildIndex, LoadSceneMode mode, Action onComplete)
    {
        IsLoadingScene = true;
        string sceneName = $"BuildIndex:{buildIndex}";
        OnSceneLoadStarted?.Invoke(sceneName);

        AsyncOperation op;
        try
        {
            op = SceneManager.LoadSceneAsync(buildIndex, mode);
        }
        catch (Exception ex)
        {
            Debug.LogError($"GameManager: Exception while starting LoadSceneAsync(index {buildIndex}): {ex}");
            IsLoadingScene = false;
            yield break;
        }

        if (op == null)
        {
            Debug.LogError($"GameManager: Failed to start async load for build index '{buildIndex}'. Make sure the index is valid in Build Settings.");
            IsLoadingScene = false;
            yield break;
        }

        while (!op.isDone)
            yield return null;

        yield return null;

        OnSceneLoadCompleted?.Invoke(sceneName);
        onComplete?.Invoke();
        IsLoadingScene = false;
    }
}
