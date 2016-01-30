using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioSystem : MonoBehaviour {

    public AudioClip[] drumzC;
    public AudioClip[] ambientC;
    public AudioClip wpierdol; 

    public AudioMixerSnapshot MenuToGame;
    public AudioMixerSnapshot GameToMenu;
    public AudioMixerSnapshot ToWpierdol;

    public AudioSource audioMiG;
    public AudioSource audioM;
    public AudioSource ambient;
    public AudioSource sfx;
    public AudioSource oneShots;

    public bool wpierdolB; 

	void Start () {


        foreach (AudioSource a in GameObject.FindObjectsOfType<AudioSource>())
            DontDestroyOnLoad(a);

        GameToMenu.TransitionTo(0f);

	}

    public void TransitionToGame()
    {
        GameToMenu.TransitionTo(0.5f);
        
    }


    public void TransitionToWpierdol()
    {
        wpierdolB = !wpierdolB;
        if (wpierdolB)
        {
            oneShots.PlayOneShot(wpierdol);
            ToWpierdol.TransitionTo(1f);
        }
        if (!wpierdolB)
            MenuToGame.TransitionTo(1f);
    }

    void Update()
    {
        if (wpierdol && !oneShots.isPlaying)
            MenuToGame.TransitionTo(1f);
    }
}
