using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour {

    public AudioClip[] drumzC;
    public AudioClip[] ambientC;

    public AudioMixerSnapshot MenuToGame;
    public AudioMixerSnapshot GameToMenu;

    public AudioSource audioMiG;
    public AudioSource audioM;
    public AudioSource ambient;
    public AudioSource sfx; 

	void Start () {


        foreach (AudioSource a in GameObject.FindObjectsOfType<AudioSource>())
            DontDestroyOnLoad(a);

        GameToMenu.TransitionTo(0f);

	}

    public void TransitionToGame()
    {
        MenuToGame.TransitionTo(0.5f);
        
    }

    void Update()
    {
        if (!audioMiG.isPlaying)
            ambient.PlayOneShot(ambientC[1]);
    }
}
