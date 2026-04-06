using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
   public static AudioManager Instance;
   
   private AudioSource _systemSource;
   private List<AudioSource> _activeSources;
   
   // List of AudioCollection references that the user can populate in the inspector.
   [Tooltip("Populate with AudioCollection components (e.g. music collections). Use PlayFromCollection to play by collection and clip index.")]
   public List<AudioCollection> audioCollections = new List<AudioCollection>();

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
         _systemSource = gameObject.GetComponent<AudioSource>();
         _activeSources = new List<AudioSource>();
      }
      else
      {
         Destroy(gameObject);
      }
   }
   
   //Funções de gerenciamneto de áudio2d
   public void PlaySound(AudioClip clip)
   { 
      _systemSource.Stop();
      _systemSource.clip = clip;
      _systemSource.Play();
   }

   // Play a clip by collection index and clip index. Validates indices.
   public void PlayFromCollection(int collectionIndex, int clipIndex)
   {
      if (audioCollections == null || collectionIndex < 0 || collectionIndex >= audioCollections.Count)
      {
         Debug.LogWarning($"AudioManager: collectionIndex {collectionIndex} out of range.");
         return;
      }

      var collection = audioCollections[collectionIndex];
      if (collection == null || collection.AudioClipCollection == null)
      {
         Debug.LogWarning($"AudioManager: collection at index {collectionIndex} is null or has no clips.");
         return;
      }

      if (clipIndex < 0 || clipIndex >= collection.AudioClipCollection.Count)
      {
         Debug.LogWarning($"AudioManager: clipIndex {clipIndex} out of range for collection {collectionIndex}.");
         return;
      }

      var clip = collection.AudioClipCollection[clipIndex];
      if (clip == null)
      {
         Debug.LogWarning($"AudioManager: clip at index {clipIndex} in collection {collectionIndex} is null.");
         return;
      }

      PlaySound(clip);
   }

   public void StopSound()
   {
      _systemSource.Stop();
   }
   
   public void PauseSound()
   {
      _systemSource.Pause();
   }

   public void ResumeSound()
   {
      _systemSource.UnPause();
   }
   
   public void PlayOneShot(AudioClip clip)
   {
      _systemSource.PlayOneShot(clip);
   }
   
   //Funções de gerenciamento de áudio 3D

   public void PlaySound(AudioClip clip, AudioSource source)
   {
      if(!_activeSources.Contains(source)) _activeSources.Add(source);
      source.Stop();
      source.clip = clip;
      source.Play();
   }
   
   public void StopSound(AudioSource source)
   {
      source.Stop();
      _activeSources.Remove(source);
   }
   
   public void PauseSound(AudioSource source)
   {
      source.Pause();
   }

   public void ResumeSound(AudioSource source)
   {
      source.UnPause();
   }

   public void PlayOneShot(AudioClip clip, AudioSource source)
   {
      source.PlayOneShot(clip);
   }
}
