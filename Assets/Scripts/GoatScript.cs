using UnityEngine;
using System.Collections;
using UnityEngine.Audio; 

public class GoatScript : MonoBehaviour {

    public AudioClip beeeee;
    public AudioClip[] goatDeath;
    public AudioClip blood; 
    public AudioSource sfx;
    Animator anim;
    public int clicks;
    public bool coolBool;
    public float cd = 1.5f;
    public float time;
    Critter crit;
    GameObject smoke; 

    void Awake ()
    {
        if (sfx == null)
            sfx = GameObject.Find("Audio/AudioSFX").GetComponent<AudioSource>();
        crit = GetComponent<Critter>();

    }


    void Start () {
        sfx.volume = Random.Range(0.8f, 1f);
        sfx.pitch = Random.Range(0.75f, 1f);
        int c = Random.Range(0, 50);
        if (c > 25) 
            sfx.PlayOneShot(beeeee);
        smoke = Resources.Load("smoke") as GameObject;
        anim = GetComponent<Animator>();

    }

    void OnMouseDown()
    {
        clicks++;
        time = 0f; 
        coolBool = true; 
        if (clicks >= 5)
        {
            GameObject s = Instantiate(smoke, this.gameObject.transform.position, this.gameObject.transform.rotation) as GameObject;
            clicks = 0;
          //  anim.Play("goat_death");
        }
        
    }


    public void DestroyMe() { 
        Destroy(this.gameObject);
    }


	// Update is called once per frame
	void Update () {


        if (crit.velocity.x > 0 && crit.velocity.y > 0)
            anim.SetTrigger("U");
        if (crit.velocity.x < 0 && crit.velocity.y > 0)
            anim.SetTrigger("R");
        if (crit.velocity.x < 0 && crit.velocity.y < 0)
            anim.SetTrigger("D");
        if (crit.velocity.x > 0 && crit.velocity.y < 0)
            anim.SetTrigger("L");

        if (coolBool)
        {
            time += Time.deltaTime;
            if (time > cd)
            {
               clicks = 0;
                coolBool = false; 
            }
        }
	}
}
