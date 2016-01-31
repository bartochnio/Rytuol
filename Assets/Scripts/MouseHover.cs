using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( OutLine))]
public class MouseHover : MonoBehaviour {

    OutLine outL;
	// Use this for initialization
	void Start () {

        outL = GetComponent<OutLine>();
	
	}

    void OnMouseEnter()
    {
        outL.drawOutline = true;
    }

    void OnMouseExit()
    {
        outL.drawOutline = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
