using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( Camera))]
[ExecuteInEditMode]
public class PixelPerfectCamera : MonoBehaviour {

    public int referencePPU = 100;
    public int referenceHeight = 768;
    public int referenceWidth = 1024;

    public float worldHeight;
    public float worldWidth;

    public float perfectOrthoSize;

    public float wStep;
    public float hStep;

    Camera c;

    void SetPerfect()
    {
        c = GetComponent<Camera>();
        perfectOrthoSize = (float)referenceHeight / ((float)referencePPU * 2f);
        c.orthographicSize = perfectOrthoSize;

        worldHeight = 2 * perfectOrthoSize;
        worldWidth = worldHeight / c.aspect;
        wStep = worldWidth / referenceWidth;
        hStep = worldHeight / referenceHeight;
        Debug.Log("PerfectOrtho: " + perfectOrthoSize + "\r\n H Step: " + hStep + " W Step: " + wStep);
    }
    //public Texture2D t;
    void Awake()
    {
        SetPerfect();
    }



    public Vector2 SnapToPixelPerfectPos ( Vector2 current )
    {
        int rX = Mathf.FloorToInt(current.x / wStep);
        int rY = Mathf.FloorToInt(current.y / hStep);

        return new Vector2(rX * wStep, rY * hStep);
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
        SetPerfect();
	}
}
