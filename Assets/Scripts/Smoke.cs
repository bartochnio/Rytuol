using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Smoke : MonoBehaviour {

    public AudioSource sfx;
    public AudioClip puff;

    public void playPuff()
    {
        sfx.PlayOneShot(puff);

    }

}
