using UnityEngine;
using System.Collections;

public class SavageAn : MonoBehaviour {


    Animator anim;
    Critter crit;
    public RuntimeAnimatorController peon;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        crit = GetComponent<Critter>();
   //     peon = GameObject.Find("Audio").GetComponent<Animator>().runtimeAnimatorController;
  //      StartCoroutine("Conversion");
	}

    IEnumerator Conversion()
    {
        yield return new WaitForSeconds(5f);
        anim.runtimeAnimatorController = peon;
        this.gameObject.AddComponent<Peon>();
        this.gameObject.GetComponent<Peon>().MoveToPeonsArea();
        this.gameObject.AddComponent<Rigidbody2D>();
        this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
        enabled = false;
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
