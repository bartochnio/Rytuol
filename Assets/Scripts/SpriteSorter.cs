using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteSorter : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.sortingOrder = (int)(transform.position.y * -10);
    }
}
