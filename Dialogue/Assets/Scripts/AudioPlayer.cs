using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    // Reference to a collection of audio clips (set in inspector)
    public AudioCollection myAudioCollection;

    // Index of the clip inside the collection that will be played when requested
    [Tooltip("Index of the clip inside the selected AudioCollection to play via AudioManager at runtime.")]
    public int selectedIndex = 0;

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
        if (clip == null)
        {
            Debug.LogWarning("AudioPlayer: No clip available at selected index.");
            return;
        }

        if (Application.isPlaying && AudioManager.Instance != null)
        {
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
            PlaySelected();
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
