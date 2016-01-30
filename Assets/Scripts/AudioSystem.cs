using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour {

    public AudioClip[] drumz;
    public AudioClip[] ambient;

    public AudioMixerSnapshot MenuToGame;
    public AudioMixerSnapshot GameToMenu;


	void Start () {


        foreach (AudioSource a in GameObject.FindObjectsOfType<AudioSource>())
            DontDestroyOnLoad(a);

        GameToMenu.TransitionTo(0f);

	}

    public void TransitionToGame()
    {
        MenuToGame.TransitionTo(0.5f);
    }

	
}
