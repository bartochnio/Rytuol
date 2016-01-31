using UnityEngine;
using System.Collections;

[RequireComponent ( typeof ( SpriteRenderer))]
public class OutLine : MonoBehaviour {

    public bool drawOutline = false;
    public float outlineScale = 2.2f;
    public float zOffset = 1;
    public Color outlineColor = Color.black;
    private SpriteRenderer srcRend;
    private SpriteRenderer outRend;

    SpriteRenderer CreateOutlinRend()
    {

        srcRend = GetComponent<SpriteRenderer>();

        GameObject obj = new GameObject("Outline", typeof(SpriteRenderer));
        obj.transform.SetParent(this.transform);
        obj.transform.localPosition = Vector3.zero + Vector3.forward * zOffset;
        SpriteRenderer outRend = obj.GetComponent<SpriteRenderer>();
        outRend.material = srcRend.material;
        outRend.sprite = srcRend.sprite;

        return outRend;

    }

    void ModifyOutline(SpriteRenderer rend)
    {
        rend.material.color = outlineColor;
        Vector3 scale = srcRend.transform.localScale;
        rend.transform.localScale = scale * outlineScale;
        outRend.sortingOrder = srcRend.sortingOrder - 1;
    }
	// Use this for initialization
	void Start () {

        outRend = CreateOutlinRend();
        ModifyOutline(outRend);
	
	}
	
	// Update is called once per frame
	void Update () {

        outRend.enabled = drawOutline;
        if (drawOutline)
        {
            ModifyOutline(outRend);
        }
	
	}
}
