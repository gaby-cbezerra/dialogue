using UnityEditor;
using UnityEngine;

// Custom inspector to control AudioPlayer from the inspector (play/pause/stop/resume and set index)
[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioPlayer player = (AudioPlayer)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play Selected"))
        {
            // Call PlaySelected. If in edit-mode, this will use the local AudioSource fallback.
            player.PlaySelected();
        }

        if (GUILayout.Button("Stop"))
        {
            // If running, prefer AudioManager; otherwise fallback to player
            if (EditorApplication.isPlaying && AudioManager.Instance != null)
                AudioManager.Instance.StopSound();
            else
                player.Stop();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Pause"))
        {
            if (EditorApplication.isPlaying && AudioManager.Instance != null)
                AudioManager.Instance.PauseSound();
            else
                player.Pause();
        }
        if (GUILayout.Button("Resume"))
        {
            if (EditorApplication.isPlaying && AudioManager.Instance != null)
                AudioManager.Instance.ResumeSound();
            else
                player.Resume();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Play via AudioManager (runtime only)", EditorStyles.boldLabel);
        player.selectedIndex = EditorGUILayout.IntField("Clip Index", player.selectedIndex);
        player.managerCollectionIndex = EditorGUILayout.IntField("Manager Collection Index", player.managerCollectionIndex);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play via AudioManager"))
        {
            if (EditorApplication.isPlaying && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFromCollection(player.managerCollectionIndex, player.selectedIndex);
            }
            else
            {
                // fallback to local play
                player.SetIndexAndPlay(player.selectedIndex, true);
            }
        }

        if (GUILayout.Button("Set Index and Play (Runtime)"))
        {
            if (EditorApplication.isPlaying && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFromCollection(player.managerCollectionIndex, player.selectedIndex);
            }
            else
            {
                player.SetIndexAndPlay(player.selectedIndex, true);
            }
        }
        EditorGUILayout.EndHorizontal();

        // Additional helper: let user type an arbitrary clip index and play via AudioManager singleton directly
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Play (via AudioManager singleton)", EditorStyles.boldLabel);
        int quickIndex = EditorGUILayout.IntField("Clip Index to Play", player.selectedIndex);
        int quickCollection = EditorGUILayout.IntField("Collection Index", player.managerCollectionIndex);
        if (GUILayout.Button("Play Collection/Clip via Singleton (Play Mode only)"))
        {
            if (EditorApplication.isPlaying && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFromCollection(quickCollection, quickIndex);
            }
            else
            {
                Debug.LogWarning("AudioPlayerEditor: Play via singleton is only available in Play Mode and requires an AudioManager instance.");
            }
        }

        // Ensure changes to the player are saved to the scene/prefab
        if (GUI.changed)
        {
            EditorUtility.SetDirty(player);
        }
    }
}

