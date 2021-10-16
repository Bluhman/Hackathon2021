using UnityEngine;

// The Audio Source component has an AudioClip option.  The audio
// played in this example comes from AudioClip and is called audioData.

[RequireComponent(typeof(AudioSource))]
public class PlaySound : MonoBehaviour
{
   AudioSource audioData;

   void OnEnable()
   {
      audioData = GetComponent<AudioSource>();
      audioData.Play(0);
   }
}