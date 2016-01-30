using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SpriteSorter : MonoBehaviour {

	void Update ()
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.sortingOrder = (int)(transform.position.y * -10);
    }
}
