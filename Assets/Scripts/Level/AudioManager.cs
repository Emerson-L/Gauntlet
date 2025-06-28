using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip Footstep;
    public AudioSource SourceFootstep;


    public void PlayFootstep(float vol)
    {
        SourceFootstep.PlayOneShot(Footstep, vol);
    }
}
