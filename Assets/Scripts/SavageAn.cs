using UnityEngine;
using System.Collections;

public class SavageAn : MonoBehaviour {


    Animator anim;
    Critter crit; 
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        crit = GetComponent<Critter>();
	}
	
	// Update is called once per frame
	void Update () {

        if (crit.velocity.x > 0 && crit.velocity.y > 0)
            anim.SetTrigger("UR");
        if (crit.velocity.x < 0 && crit.velocity.y > 0)
            anim.SetTrigger("UL");
        if (crit.velocity.x < 0 && crit.velocity.y < 0)
            anim.SetTrigger("DL");
        if (crit.velocity.x > 0 && crit.velocity.y < 0)
            anim.SetTrigger("DR");

    }
}
