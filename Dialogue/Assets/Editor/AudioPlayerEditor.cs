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
            // Mark scene dirty when playing in edit mode? Usually not needed for temporary playback.
        }

        if (GUILayout.Button("Stop"))
        {
            player.Stop();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Pause"))
        {
            player.Pause();
        }
        if (GUILayout.Button("Resume"))
        {
            player.Resume();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Play via AudioManager (runtime only)", EditorStyles.boldLabel);
        player.selectedIndex = EditorGUILayout.IntField("Selected Index", player.selectedIndex);

        if (GUILayout.Button("Set Index and Play (Runtime)"))
        {
            // If the editor isn't in play mode, try to still play using local source; otherwise use AudioManager singleton
            if (EditorApplication.isPlaying)
            {
                player.SetIndexAndPlay(player.selectedIndex, true);
            }
            else
            {
                // In edit mode, set index and play using local source
                player.SetIndexAndPlay(player.selectedIndex, true);
            }
        }

        // Ensure changes to the player are saved to the scene/prefab
        if (GUI.changed)
        {
            EditorUtility.SetDirty(player);
        }
    }
}

