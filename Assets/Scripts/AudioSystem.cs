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
    bool randomBeeee;
    public bool wpierdolB;
    public AudioClip beeee;
    public GameObject a;

 
	void Start () {

        DontDestroyOnLoad(a);
        randomBeeee = true; 
        StartCoroutine("PlayRandomBeee");
	}

    public void TransitionToGame()
    {
        MenuToGame.TransitionTo(0.5f);
        randomBeeee = false;
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

    public IEnumerator PlayRandomBeee()
    {
        while (randomBeeee) { 
            float beeecooldown = Random.Range(6f, 10f);
            yield return new WaitForSeconds(beeecooldown);
            sfx.volume = Random.Range(0.6f, 1f);
            sfx.pitch = Random.Range(0.65f, 1f);
            sfx.PlayOneShot(beeee);

        }

    }

 


    void Update()
    {
       // if (wpierdol && !oneShots.isPlaying)
       //     MenuToGame.TransitionTo(1f);
    }
}
