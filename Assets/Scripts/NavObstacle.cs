using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class NavObstacle : MonoBehaviour
{
    private PolygonCollider2D polygon;

    public List<Vector2> GetVerts() 
    {
        List<Vector2> verts = new List<Vector2>(polygon.GetPath(0));
        //transform to world space
        for(int i = 0; i < verts.Count; ++i)
        {
            verts[i] = transform.TransformPoint((Vector3)verts[i]);
        }

        return verts; 
    }

    void OnEnable()
    {
        polygon = GetComponent<PolygonCollider2D>();
    }

	void LateUpdate () 
    {
        DrawOutline();
	}

    void DrawOutline()
    {
        List<Vector2> verts = GetVerts();
        for (int i = 0; i < verts.Count; ++i)
        {
            int s = i;
            int e = (i + 1) % verts.Count;

            //if (i == 0)
            //{
            //    Debug.DrawRay(vertices[s], Vector2.up*0.5f, Color.red);
            //    Debug.DrawRay(vertices[e], Vector2.up*0.5f, Color.magenta);
            //}

            Debug.DrawLine(verts[s], verts[e], Color.red);
        }
    }

    public bool Intersects(Vector2 a, Vector2 b)
    {
        List<Vector2> verts = GetVerts();
        for (int i = 0; i < verts.Count; ++i)
        {
            int next = (i + 1) % verts.Count;

            Vector2 p = verts[i];
            Vector2 r = verts[next] - verts[i];
            Vector2 q = a;
            Vector2 s = b - a;

            if (Utils.LineIntersect(p,r,q,s) > -1.0f)
            {
                return true;
            }
        }

        return false;
    }
}
