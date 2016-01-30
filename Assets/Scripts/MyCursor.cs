using UnityEngine;
using System.Collections;

public class MyCursor : MonoBehaviour {

    public Texture2D texture;
	// Use this for initialization
	void Start ()
    {
        if (texture != null) Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
