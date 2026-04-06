using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    // Reference to a collection of audio clips (set in inspector)
    public AudioCollection myAudioCollection;

    // Index of the clip inside the collection that will be played when requested
    [Tooltip("Index of the clip inside the selected AudioCollection to play via AudioManager at runtime.")]
    public int selectedIndex = 0;

    // When true, play using AudioManager.audioCollections at runtime by specifying a collection index.
    [Tooltip("When true, use AudioManager.audioCollections[managerCollectionIndex] as the source when in play mode.")]
    public bool useManagerCollections = true;

    // Index into AudioManager.audioCollections to choose which collection to play from when using the manager.
    [Tooltip("Index of the AudioCollection inside the AudioManager.audioCollections list to use when playing in runtime.")]
    public int managerCollectionIndex = 0;

    // Local AudioSource used as a fallback when AudioManager is not available or when in edit-time previews
    private AudioSource localSource;

    private void Awake()
    {
        localSource = GetComponent<AudioSource>();
    }

    // Play the clip at selectedIndex using the AudioManager singleton when in Play Mode.
    // If AudioManager is not available, plays using the local AudioSource.
    public void PlaySelected()
    {
        var clip = GetSelectedClip();
        if (clip == null && !(Application.isPlaying && AudioManager.Instance != null && useManagerCollections))
        {
            Debug.LogWarning("AudioPlayer: No clip available at selected index.");
            return;
        }

        if (Application.isPlaying && AudioManager.Instance != null && useManagerCollections)
        {
            // Use AudioManager's collections by index (safe even if indices are out of range — AudioManager will warn)
            AudioManager.Instance.PlayFromCollection(managerCollectionIndex, selectedIndex);
        }
        else if (Application.isPlaying && AudioManager.Instance != null)
        {
            // AudioManager exists but not using its collections: play the local clip through the manager
            AudioManager.Instance.PlaySound(clip);
        }
        else
        {
            // Fallback for edit-mode or missing AudioManager
            localSource.Stop();
            localSource.clip = clip;
            localSource.Play();
        }
    }

    public void Stop()
    {
        if (Application.isPlaying && AudioManager.Instance != null)
        {
            AudioManager.Instance.StopSound();
        }
        else
        {
            localSource.Stop();
        }
    }

    public void Pause()
    {
        if (Application.isPlaying && AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseSound();
        }
        else
        {
            localSource.Pause();
        }
    }

    public void Resume()
    {
        if (Application.isPlaying && AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeSound();
        }
        else
        {
            // UnPause is only valid if there is a clip
            if (localSource.clip != null)
                localSource.UnPause();
        }
    }

    // Set the selected index and (optionally) play immediately via AudioManager when in Play Mode.
    public void SetIndexAndPlay(int index, bool playImmediately = true)
    {
        selectedIndex = index;
        if (playImmediately)
        {
            // If using AudioManager collections in play mode, directly call the manager to play the requested collection/index
            if (Application.isPlaying && AudioManager.Instance != null && useManagerCollections)
            {
                AudioManager.Instance.PlayFromCollection(managerCollectionIndex, selectedIndex);
            }
            else
            {
                PlaySelected();
            }
        }
    }

    // Helper to safely get the AudioClip at the selected index
    public AudioClip GetSelectedClip()
    {
        if (myAudioCollection == null || myAudioCollection.AudioClipCollection == null)
            return null;

        if (selectedIndex < 0 || selectedIndex >= myAudioCollection.AudioClipCollection.Count)
            return null;

        return myAudioCollection.AudioClipCollection[selectedIndex];
    }
}
