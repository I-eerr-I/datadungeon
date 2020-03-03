using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
   public AudioClip startBG;
   public AudioClip loopBG;

   AudioSource audioSource;

   void Start()
   {
       audioSource = GetComponent<AudioSource>();
       audioSource.clip = startBG;
       audioSource.Play();
   } 

   void Update()
   {
       if(!audioSource.isPlaying)
       {
           audioSource.clip = loopBG;
           audioSource.loop = true;
           audioSource.Play();
       }
   }
}
