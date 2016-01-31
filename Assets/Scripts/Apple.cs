using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Apple : MonoBehaviour {

    public AudioSource sfx;
    public AudioClip pac;

    void Start()
    {
        if (sfx == null)
            sfx = GameObject.Find("Audio/AudioSFX").GetComponent<AudioSource>();
    }


    public void PlayPac()
    {
        if (sfx != null)
            sfx.PlayOneShot(pac);
        //sfx = GameObject.Find("Audio/AudioSFX").GetComponent<AudioSource>();
    }



}
