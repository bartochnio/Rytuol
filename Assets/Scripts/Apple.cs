using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Apple : MonoBehaviour {

    public AudioSource sfx;
    public AudioClip pac;




    public void PlayPac()
    {
        sfx.PlayOneShot(pac);
    }

}
