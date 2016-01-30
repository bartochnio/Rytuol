using UnityEngine;
using System.Collections;
using UnityEngine.Audio; 

public class GoatScript : MonoBehaviour {

    public AudioClip beeeee;
    public AudioClip[] goatDeath;
    public AudioClip blood; 
    public AudioSource sfx;
    Animator anim; 

	void Start () {
        sfx.volume = Random.RandomRange(0.8f, 1f);
        sfx.pitch = Random.RandomRange(0.75f, 1f);
        sfx.PlayOneShot(beeeee);
        anim = GetComponent<Animator>();
        GetComponent<ParticleSystem>().enableEmission = false;
        StartCoroutine("GoatDeathSounds");
    }

    public void GoatDeath()
    {
        
    }

    IEnumerator GoatDeathSounds()
    {

        yield return new WaitForSeconds(2f);
        Debug.Log("DIE!");
        sfx.PlayOneShot(blood);
        GetComponent<ParticleSystem>().enableEmission = true;
        yield return new WaitForSeconds(0.2f);
        sfx.PlayOneShot(goatDeath[Random.RandomRange(0, goatDeath.Length)]);

        while (sfx.isPlaying)
            yield return new WaitForEndOfFrame();
        anim.Play("goat_death");
        GetComponent<ParticleSystem>().enableEmission = false;
    }
    public void DestroyMe() { 
        Destroy(this.gameObject);
    }


	// Update is called once per frame
	void Update () {
	
	}
}
