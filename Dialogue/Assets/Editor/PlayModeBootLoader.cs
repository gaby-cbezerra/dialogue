using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Editor utility that makes the Editor load a designated _Boot scene when Play is pressed,
// then loads the previously active scene additively and unloads the _Boot scene.

    [InitializeOnLoad]
    public static class PlayModeBootLoader
    {
        private const string KSessionKeyOriginal = "PlayModeBootLoader_OriginalScene";
        private const string KSessionKeyBootPath = "PlayModeBootLoader_BootPath";

        static PlayModeBootLoader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                PrepareBootSceneBeforePlay();
            }

            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                LoadOriginalSceneAfterPlayStarted();
            }

            if (state == PlayModeStateChange.EnteredEditMode)
            {
                // Clean up any leftover state if Play was stopped before finishing transitions
                SessionState.EraseString(KSessionKeyOriginal);
                SessionState.EraseString(KSessionKeyBootPath);
                EditorSceneManager.playModeStartScene = null;
            }
        }

        private static void PrepareBootSceneBeforePlay()
        {
            // Get currently active scene path (may be empty for unsaved scenes)
            var activeScene = EditorSceneManager.GetActiveScene();
            string activePath = string.IsNullOrEmpty(activeScene.path) ? string.Empty : activeScene.path;

            // Try to find a scene asset named _Boot
            string bootGuid = FindBootSceneGuid();
            if (string.IsNullOrEmpty(bootGuid))
            {
                // No boot scene found; ensure nothing is forced
                SessionState.EraseString(KSessionKeyOriginal);
                SessionState.EraseString(KSessionKeyBootPath);
                EditorSceneManager.playModeStartScene = null;
                Debug.Log("PlayModeBootLoader: No _Boot scene found in project. Play will behave normally.");
                return;
            }

            string bootPath = AssetDatabase.GUIDToAssetPath(bootGuid);

            // Store original scene path for later
            SessionState.SetString(KSessionKeyOriginal, activePath);
            SessionState.SetString(KSessionKeyBootPath, bootPath);

            // Set the editor to start play mode with the _Boot scene
            SceneAsset bootAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootPath);
            if (bootAsset != null)
            {
                EditorSceneManager.playModeStartScene = bootAsset;
                Debug.Log($"PlayModeBootLoader: Setting play mode start scene to '{bootPath}'. Original scene: '{activePath}'");
            }
        }

        private static void LoadOriginalSceneAfterPlayStarted()
        {
            string originalPath = SessionState.GetString(KSessionKeyOriginal, string.Empty);
            string bootPath = SessionState.GetString(KSessionKeyBootPath, string.Empty);

            // Clear the playModeStartScene so future plays are not forced unintentionally
            EditorSceneManager.playModeStartScene = null;

            if (string.IsNullOrEmpty(bootPath))
            {
                // Nothing to do
                SessionState.EraseString(KSessionKeyOriginal);
                SessionState.EraseString(KSessionKeyBootPath);
                return;
            }

            if (string.IsNullOrEmpty(originalPath))
            {
                // No original scene saved (e.g., unsaved scene). Nothing to load additively.
                SessionState.EraseString(KSessionKeyOriginal);
                SessionState.EraseString(KSessionKeyBootPath);
                Debug.Log("PlayModeBootLoader: No original scene path was recorded (unsaved or empty). Skipping additive load.");
                return;
            }

            // Load the original scene in Single mode (replace current scene, which unloads _Boot)
            try
            {
                if (originalPath == bootPath)
                {
                    // The original scene is the same as _Boot — nothing to replace.
                    Debug.Log("PlayModeBootLoader: Original scene is the same as _Boot; no replacement required.");
                }
                else
                {
                    // Use the runtime SceneManager to load the scene by name in Single mode so it replaces the current scene (_Boot).
                    string sceneName = Path.GetFileNameWithoutExtension(originalPath);
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
                    Debug.Log($"PlayModeBootLoader: Requested SceneManager.LoadScene('{sceneName}', Single) to replace _Boot with '{originalPath}'.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"PlayModeBootLoader: Exception while loading original scene: {ex}");
            }

            // Clean session state
            SessionState.EraseString(KSessionKeyOriginal);
            SessionState.EraseString(KSessionKeyBootPath);
        }

        private static string FindBootSceneGuid()
        {
            // Try to find a scene named exactly _Boot (most likely Assets/Scenes/_Boot.unity)
            var guids = AssetDatabase.FindAssets("_Boot t:Scene");
            if (guids != null && guids.Length > 0)
                return guids[0];

            // Fallback: find any scene asset that ends with _Boot
            guids = AssetDatabase.FindAssets("t:Scene");
            foreach (var g in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(g);
                string name = Path.GetFileNameWithoutExtension(path);
                if (name == "_Boot") return g;
            }

            return null;
        }
    }




