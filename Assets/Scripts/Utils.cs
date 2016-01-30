using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static class Utils
{
    public static float cross2d(Vector2 a, Vector2 b)
    {
        return a.x * b.y - b.x * a.y;
    }

    public static float rayLineIntersect(Vector2 p, Vector2 r, Vector2 q, Vector2 s)
    {
        float rs = cross2d(r, s);
        float t = cross2d((q - p), s) / rs;
        float u = cross2d((q - p), r) / rs;

        float res = -1.0f;
        if ((u >= 0.0f && u <= 1.0f) && t >= 0.0f)
            res = t;

        return res;
    }

    public static float LineIntersect(Vector2 p, Vector2 r, Vector2 q, Vector2 s)
    {
        float rs = cross2d(r, s);
        float t = cross2d((q - p), s) / rs;
        float u = cross2d((q - p), r) / rs;

        if (Mathf.Abs(rs) < float.Epsilon && Mathf.Abs(u) < float.Epsilon)
            return -1.0f;

        if (Mathf.Abs(rs) < float.Epsilon && u != 0.0f)
            return -1.0f;

        if (rs != 0.0f && (t > 0.0f && t < 1.0f) && (u >= 0.0f && u <= 1.0f))
        {
            return t;
        }

        return -1.0f;
    }

    public static float GetLineIntersect(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2)
    {
        Vector2 p = a1;
        Vector2 r = b1 - a1;

        Vector2 q = a2;
        Vector2 s = b2 - a2;

        float rs = cross2d(r, s);
        float t = cross2d((q - p), s) / rs;
        float u = cross2d((q - p), r) / rs;

        //if (Mathf.Abs(rs) < float.Epsilon && Mathf.Abs(u) < float.Epsilon)
        //    return -1.0f;

        //if (Mathf.Abs(rs) < float.Epsilon && u != 0.0f)
        //    return -1.0f;

        float res = -1.0f;
        if (Mathf.Abs(rs) > float.Epsilon && (t >= 0.0f && t <= 1.0f) && (u >= 0.0f && u <= 1.0f))
        {
            res = t;
        }

        return res;
    }

    public static bool PolyContainsPoint(List<Vector2> poly, Vector2 p)
    {
        for (int i = 0; i < poly.Count; ++i)
        {
            int next = (i + 1) % poly.Count;

            Vector2 v1 = poly[next] - poly[i];
            Vector2 v2 = p - poly[i];

            if (Utils.cross2d(v1, v2) > 0.0f)
            {
                return false;
            }
        }
        return true;
    }

    public static float sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    public static bool isConvex(Vector2 a, Vector2 b, Vector2 c)
    {
        Vector2 d1 = b - a;
        Vector2 d2 = c - b;

        //matrix determinant
        return d1.x * d2.y - d1.y * d2.x < 0.0f;
    }

    //TODO: check how it exacly works / find faster algo
    public static bool triContains(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        bool b1, b2, b3;

        b1 = Utils.sign(p, a, b) < 0.0f;
        b2 = Utils.sign(p, b, c) < 0.0f;
        b3 = Utils.sign(p, c, a) < 0.0f;

        return ((b1 == b2) && (b2 == b3));
    }

    public static float angle(Vector2 a, Vector2 b, Vector2 c)
    {
        float ax = c.x - b.x;
        float ay = c.y - b.y;
        float bx = a.x - b.x;
        float by = a.y - b.y;
        return Mathf.Atan2(ax * by - ay * bx, ax * bx + ay * by)*Mathf.Rad2Deg;
    }

    public static Vector2 projectPointToSegment(Vector2 p, Vector2 A, Vector2 B)
    {
        Vector2 AB = B - A;

        float ABsqr = Vector2.Dot(AB,AB);
        if (ABsqr == 0)
            return A;
        else
        {
            Vector2 Ap = p - A;
            float t = Vector2.Dot(Ap,AB) / ABsqr;
            if (t < 0.0)
                return A;
            else if (t > 1.0)
                return B;
            else
                return A + t * AB;
        }
    }
}

