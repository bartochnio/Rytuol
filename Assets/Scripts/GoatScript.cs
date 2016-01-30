﻿using UnityEngine;
using System.Collections;
using UnityEngine.Audio; 

public class GoatScript : MonoBehaviour {

    public AudioClip beeeee;
    public AudioClip[] goatDeath;
    public AudioClip blood; 
    public AudioSource sfx;
    Animator anim;
    public GameObject smoke;
    public int clicks;

    void Start () {
        sfx.volume = Random.RandomRange(0.8f, 1f);
        sfx.pitch = Random.RandomRange(0.75f, 1f);
        sfx.PlayOneShot(beeeee);
        anim = GetComponent<Animator>();
        GetComponent<ParticleSystem>().enableEmission = false;
    //    StartCoroutine("GoatDeathSounds");
    }

    void OnMouseDown()
    {
        clicks++;

        if (clicks >= 10)
        {
            smoke.GetComponent<SpriteRenderer>().enabled = true;
            smoke.GetComponent<Animator>().enabled = true;
            anim.Play("goat_death");
        }
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
        smoke.GetComponent<SpriteRenderer>().enabled = true;
        smoke.GetComponent<Animator>().enabled = true;
        anim.Play("goat_death");
        
    }
    public void DestroyMe() { 
        Destroy(this.gameObject);
    }


	// Update is called once per frame
	void Update () {
	   
	}
}
