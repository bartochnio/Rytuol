using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class NavMesh2D : MonoBehaviour 
{
    //for debugging
    public Transform player;

    public bool debugDrawOutline = false;
    public bool debugDrawPolys = false;
    public bool generateOnUpdate = false;
    public bool drawEdges = true;

    private static List<Edge> edges = new List<Edge>();
    private static List<Node> nodes = new List<Node>();

    public class Edge : IEquatable<Edge>
    {
        public Vector2 e1;
        public Vector2 e2;

        public int[] polys = new int[2];

        public Edge()
        {
            e1 = Vector2.zero;
            e2 = Vector2.zero;
            polys[0] = polys[1] = -1;
        }

        public Vector2 GetCenter()
        {
            return (e1 + e2) / 2.0f;
        }

        public bool Equals(Edge other)
        {
            return (Vector2.Distance(e1, other.e1) < 0.0001f && Vector2.Distance(e2, other.e2) < 0.0001f)
                || (Vector2.Distance(e1, other.e2) < 0.0001f && Vector2.Distance(e2, other.e1) < 0.0001f);
        }
    }

    public class Node
    {
        //this is inefficient but we need to store verts for containment test
        //since edges are not guaranteed to be CW
        public List<Vector2> verts; 
        public List<int> edgeInds = new List<int>();
        public int id;
        public Vector2 center;

        public float Gcost;
        public float Hcost;

        public bool Contains(Vector2 p)
        {
            return Utils.PolyContainsPoint(verts, p);
        }

        public Edge GetEdge(int nidx)
        {
            foreach(int idx in edgeInds)
            {
                if (edges[idx].polys[0] == nidx || edges[idx].polys[1] == nidx)
                    return edges[idx];
            }

            return null;
        }

        public List<int> GetNeighbours()
        {
            List<int> result = new List<int>();
            foreach(int idx in edgeInds)
            {
                for (int i = 0; i < 2; ++i)
                {
                    if (edges[idx].polys[i] == id || edges[idx].polys[i] < 0)
                        continue;

                    result.Add(edges[idx].polys[i]);
                }
            }

            return result;
        }

        //Debug stuff
        public void DebugDraw(Color color)
        {
            foreach (int idx in edgeInds)
            {
                Debug.DrawLine(edges[idx].e1, edges[idx].e2, color);
            }
        }

        public void DrawNeighbours()
        {
            foreach (int idx in edgeInds)
            {
                for(int i = 0; i < 2; ++i)
                {
                    if (edges[idx].polys[i] == id || edges[idx].polys[i] < 0)
                        continue;

                    nodes[edges[idx].polys[i]].DebugDraw(Color.yellow);
                    //Debug.DrawLine(center, nodes[edges[idx].polys[i]].center, Color.yellow);
                }
            }

            DebugDraw(Color.blue);
        }
    }

    private PolygonCollider2D polygon;
    
    
    public void AddObstacle()
    {
        GameObject obstacle = new GameObject();
        obstacle.transform.parent = transform;
        obstacle.AddComponent<PolygonCollider2D>();
        obstacle.AddComponent<NavObstacle>();
        obstacle.name = "NavObstacle";
    }

    void OnEnable()
    {
        polygon = GetComponent<PolygonCollider2D>();
    }

	void Start () 
    {
        if (!generateOnUpdate)
            Generate();
	}

	void Update ()
    {
        if (generateOnUpdate)
            Generate();

        //debug draw
        if (debugDrawPolys)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                nodes[i].DebugDraw(Color.cyan);
            }
        }

        //debug draw
        if (debugDrawOutline)
            DrawOutline();


        int nodeIdx = FindContainingNode(player.position);
        if (nodeIdx > -1)
        {
            nodes[nodeIdx].DrawNeighbours();
        }
    }

    public Node GetNode(int idx)
    {
        return nodes[idx];
    }

    public int FindContainingNode(Vector2 p)
    {
        for(int i = 0; i < nodes.Count; ++i)
        {
            if (nodes[i].Contains(p))
                return i;
        }

        return -1;
    }

    int Next(int idx, int size)
    {
        return (idx + 1) % size;
    }

    int Prev(int idx, int size)
    {
        return idx == 0 ? size - 1 : idx - 1;
    }

    public void Generate()
    {
        Decompose();
    }

    public void Decompose()
    {
        nodes.Clear();
        edges.Clear();

        //get holes
        List<NavObstacle> obstacles = new List<NavObstacle>();
        GetComponentsInChildren<NavObstacle>(obstacles);

        List<int> L = new List<int>();
        List<Vector2> verts = new List<Vector2>(polygon.GetPath(0));
        
        //transform to world space
        for (int i = 0; i < verts.Count; ++i)
        {
            verts[i] = transform.TransformPoint((Vector2)verts[i]);
        }

        L.Add(0); //v1
        L.Add(1); //v2

        int counter = 100;
        while(verts.Count > 0 && counter > 0)
        {
            List<Vector2> poly = CreateConvex(verts, L);

            int first = L[0];
            int last = L[L.Count - 1];
            if (poly != null)
            {
                //check holes
                //TODO: REFACTOR

                Vector2 diagA = Vector2.zero;
                Vector2 diagB = Vector2.zero;
                
                //check if diagonal intersects a hole
                bool intersecting = false;
                int obIdx = 0;
                for (; obIdx < obstacles.Count; ++obIdx)
                {
                    if (obstacles[obIdx].Intersects(verts[first], verts[last]))
                    {
                        intersecting = true;
                        diagA = verts[first];
                        diagB = verts[last];

                        break;
                    }
                }

                //check if new poly cointains a hole
                bool containing = false;
                int closestHoleIdx = 0;
                int closestInnerIdx = 0;
                float closest = float.MaxValue;
                if (!intersecting)
                {
                    int closestOuterIdx = first;
                    
                    for (int i = 0; i < obstacles.Count; ++i)
                    {
                        List<Vector2> obsVerts = obstacles[i].GetVerts();
                        for (int j = 0; j < obsVerts.Count; ++j)
                        {
                            if (Utils.PolyContainsPoint(poly, obsVerts[j]))
                            {
                                float d = Vector2.Distance(verts[closestOuterIdx], obsVerts[j]);
                                if (d <= closest)
                                {
                                    closestHoleIdx = i;
                                    closestInnerIdx = j;
                                    containing = true;
                                }
                            }
                        }
                    }

                    if (containing)
                    {
                        diagA = verts[closestOuterIdx];
                        diagB = obstacles[closestHoleIdx].GetVerts()[closestInnerIdx];
                    }
                }

                if (intersecting || containing)
                {
                    int holeIdx = closestHoleIdx;
                    int es = closestInnerIdx;
                    //Debug.DrawLine(diagA, diagB, Color.gray);
                    GetTrueDiagonal(diagA, diagB, obstacles, verts, poly, out holeIdx, out es);

                    if (holeIdx < 0 || es < 0)
                    {
                        holeIdx = closestHoleIdx;
                        es = closestInnerIdx;
                    }

                    //Debug.DrawLine(diagA, obstacles[holeIdx].GetVerts()[es], Color.magenta);

                    List<Vector2> vertsToAdd = new List<Vector2>();

                    //vertsToAdd.Add(verts[first]);
                    List<Vector2> obsVerts = obstacles[holeIdx].GetVerts();
                    int idx = es;
                    for (int i = 0; i <= obsVerts.Count; ++i)
                    {
                        vertsToAdd.Add(obsVerts[idx]);
                        idx = Prev(idx, obsVerts.Count);
                    }
                    vertsToAdd.Add(verts[first]);

                    verts.InsertRange(first+1, vertsToAdd);

                    //remove obstacles[holeIdx]
                    obstacles.RemoveAt(holeIdx);

                    //clear L
                    L.Clear();
                    L.Add(first);
                    L.Add(Next(first, verts.Count));
                }
                else
                {
                    SaveNode(poly);

                    //all verts have been added
                    if (L.Count == verts.Count)
                    {
                        verts.Clear();
                        break;
                    }
                    else
                    {
                        //cut from P
                        List<Vector2> newVerts = new List<Vector2>();
                        for (int i = 0; i < verts.Count; ++i)
                        {
                            if (!L.Contains(i) || i == L[0] || i == L[L.Count - 1])
                            {
                                newVerts.Add(verts[i]);
                            }
                            else
                            {
                                if (i < L[0])
                                    --first;
                                if (i < L[L.Count - 1])
                                    --last;
                            }
                        }
                        verts = newVerts;
                    }

                    L.Clear();
                    L.Add(last);
                    L.Add((last + 1) % verts.Count);
                }                
            }
            else
            {
                --counter;
                L.Clear();
                L.Add(last);
                L.Add((last + 1) % verts.Count);
            }
        }

        if (counter <= 0)
        {
            Debug.Log("Failed to create polygon!");
        }
    }

    List<Vector2> CreateConvex(List<Vector2> P, List<int> L)
    {
        //try to expand in CW
        if (Expand(P, L, true) < P.Count)
        {
            CheckIntersections(P, L);

            //Try to expand CCW
            if (Expand(P, L, false) < P.Count)
            {
                //check intersections CCW
                CheckIntersectionsCCW(P, L);
            }
            else //we finished add last one and break
            {
                return CreatePoly(P, L);
            }

            int first = L[0];
            int last = L[L.Count - 1];
            //check if we created a convex
            if (L.Count > 2 && (IsReflex(P, first) || IsReflex(P, last)))
            {
                return CreatePoly(P, L);
            }
            else //no polygon generated
            {
                return null;
            }
        }
        else //we finished add last one and break
        {
            return CreatePoly(P, L);
        }
    }

    void GetTrueDiagonal(Vector2 dA, Vector2 dB, List<NavObstacle> obstacles, List<Vector2> P, List<Vector2> C, out int holeIdx, out int es)
    {
        holeIdx = -1;
        es = -1;

        //int closestHoleIdx = 0;
        float closest = float.MaxValue;
        int e1Idx = 0;
        int e2Idx = 0;

        Vector2 e1 = Vector2.zero;
        Vector2 e2 = Vector2.zero;

        Vector2 pc = Vector2.zero;

        bool intersected = true;
        while (intersected)
        {
            intersected = false;
            for (int i = 0; i < obstacles.Count; ++i)
            {
                List<Vector2> obsVerts = obstacles[i].GetVerts();

                for (int j = 0; j < obsVerts.Count; ++j)
                {
                    Vector2 p = dA;
                    Vector2 r = dB - dA;
                    Vector2 q = obsVerts[j];
                    Vector2 s = obsVerts[Next(j, obsVerts.Count)] - obsVerts[j];

                    float d = Utils.rayLineIntersect(p, r, q, s);
                    if (d > -1.0f && d <= closest)
                    {
                        closest = d;
                        e1Idx = j;
                        e2Idx = Next(j, obsVerts.Count);

                        e1 = obsVerts[j];
                        e2 = obsVerts[Next(j, obsVerts.Count)];

                        holeIdx = i;
                        intersected = true;

                        pc = p + r * d;

                        if (Vector2.Distance(pc, dB) < 0.0001f)
                            intersected = false;
                    }
                }
            }


            if (intersected)
            {
                //Debug.DrawLine(dA, pc, Color.blue);
                //Debug.DrawLine(e1, e2, Color.yellow);

                if (!Utils.PolyContainsPoint(C, e1))
                {
                    dB = e2;
                    es = e2Idx;
                }
                else if (!Utils.PolyContainsPoint(C, e2))
                {
                    dB = e1;
                    es = e1Idx;
                }
                else
                {
                    float d1 = Vector2.Distance(dA, e1);
                    float d2 = Vector2.Distance(dA, e2);

                    dB = d1 < d2 ? e1 : e2;
                    es = d1 < d2 ? e1Idx : e2Idx;
                }

                //Debug.DrawLine(dA, dB, Color.magenta);
            }
        }

        Debug.DrawLine(dA, dB, Color.yellow);
    }

    int Expand(List<Vector2> P, List<int> L, bool cw)
    {
        int vi = 0;

        int counter = L.Count;
        if (cw)
        {
            int last = Next(L[L.Count - 1],P.Count);
            L.Add(last);

            vi = last;            
            while (counter < P.Count && L.Count > 2)
            {
                if (AngleCheck(P, L))
                {
                    //add next
                    vi = Next(vi,P.Count);
                    L.Add(vi);
                    
                    ++counter;
                }
                else
                {
                    L.Remove(vi);
                    break;
                }

            }
        }
        else //ccw
        {
            int first = Prev(L[0],P.Count);
            L.Insert(0, first);

            vi = first;
            while (counter < P.Count && L.Count > 2)
            {
                if (AngleCheckCCW(P, L))
                {
                    vi = Prev(vi, P.Count);
                    L.Insert(0,vi);

                    ++counter;
                }
                else
                {
                    L.Remove(vi);
                    break;
                }
            }
        }

        if (counter == P.Count)
            L.RemoveAt(L.Count - 1);

        return counter;
    }

    List<Vector2> CreatePoly(List<Vector2> P, List<int> L)
    {
        List<Vector2> poly = new List<Vector2>();
        for (int i = 0; i < L.Count; ++i)
        {
            poly.Add(P[L[i]]);
        }

        return poly;
    }

    void SaveNode(List<Vector2> C)
    {
        Node node = new Node();
        node.id = nodes.Count;
        node.verts = C;
        nodes.Add(node);

        node.center = Vector2.zero;
        for (int i = 0; i < C.Count; ++i)
        {
            int idx = i;
            int next = Next(i, C.Count);

            node.center += C[idx];

            Edge edge = new Edge();
            edge.e1 = C[idx];
            edge.e2 = C[next];

            if (!edges.Contains(edge))
            {
                edge.polys[0] = node.id;
                edges.Add(edge);
                node.edgeInds.Add(edges.Count - 1);
            }
            else
            {
                int edgeIdx = edges.FindIndex(edge.Equals);
                node.edgeInds.Add(edgeIdx);

                if (edges[edgeIdx].polys[1] > -1)
                {
                    Debug.Log("Edge already contains 2 neighbours");
                }

                edges[edgeIdx].polys[1] = node.id;
            }
        }
        node.center /= C.Count;
    }

    bool IsReflex(List<Vector2> P, int idx)
    {
        int prev = idx == 0 ? P.Count - 1 : idx - 1;
        int next = (idx + 1) % P.Count;

        return (!Utils.isConvex(P[prev], P[idx], P[next]));
    }

    List<int> GetReflexVerts(List<Vector2> P)
    {
        List<int> reflex = new List<int>();

        for (int i = 0; i < P.Count; ++i)
        {
            if (IsReflex(P, i))
                reflex.Add(i);
        }

        return reflex;
    }

    void CheckIntersections(List<Vector2> P, List<int> L)
    {
        List<int> notches = GetReflexVerts(P);
        for (int i = 0; i < notches.Count; ++i)
        {
            if (L.Count > 2 && (L[L.Count - 1] == notches[i] || L[0] == notches[i]))
                continue;

            if (L.Count > 2 && Contains(P, L, P[notches[i]]))
            {
                //Debug.DrawRay(P[notches[i]], Vector2.up, Color.red);

                //get vk sign of v1 -> vi
                Vector2 vk = P[L[L.Count-1]];
                Vector2 v1 = P[L[0]];
                Vector2 vi = P[notches[i]];

                bool sign = Utils.cross2d(vk - v1, vi - v1) > 0.0f;
                
                //remove vk
                L.RemoveAt(L.Count - 1);

                //check sign of the rest v1->vi
                List<int> toRemove = new List<int>();
                for (int j = 2; j < L.Count; ++j)
                {
                    bool s = Utils.cross2d(P[L[j]] - v1, vi - v1) > 0.0f;
                    if (s == sign)
                        toRemove.Add(L[j]);
                }

                foreach (int idx in toRemove)
                    L.Remove(idx);
            }
        }
    }

    void CheckIntersectionsCCW(List<Vector2> P, List<int> L)
    {
        List<int> notches = GetReflexVerts(P);
        for (int i = 0; i < notches.Count; ++i)
        {
            if (L.Count > 2 && (L[L.Count - 1] == notches[i] || L[0] == notches[i]))
                continue;

            if (L.Count > 2 && Contains(P, L, P[notches[i]]))
            {
                //Debug.DrawRay(P[notches[i]], Vector2.up, Color.red);

                //get vk sign of v1 -> vi
                Vector2 vk = P[L[0]];
                Vector2 v1 = P[L[L.Count-1]];
                Vector2 vi = P[notches[i]];

                bool sign = Utils.cross2d(vk - v1, vi - v1) > 0.0f;

                //remove vk
                L.RemoveAt(0);

                //check sign of the rest v1->vi
                List<int> toRemove = new List<int>();
                for (int j = 0; j < L.Count-2; ++j)
                {
                    bool s = Utils.cross2d(P[L[j]] - v1, vi - v1) > 0.0f;
                    if (s == sign)
                        toRemove.Add(L[j]);
                }

                foreach (int idx in toRemove)
                    L.Remove(idx);
            }
        }
    }

    bool Contains(List<Vector2> P, List<int> L, Vector2 p)
    {
        for (int j = 0; j < L.Count; ++j)
        {
            int next = Next(j,L.Count);

            if (Vector2.Distance(P[L[j]], p) < 0.0001f || Vector2.Distance(P[L[next]], p) < 0.0001f)
                return false;

            Vector2 v1 = P[L[next]]- P[L[j]];
            Vector2 v2 = p - P[L[j]];
            if (Utils.cross2d(v1, v2) > 0.0f)
            {
                return false;
            }
        }

        return true;
    }

    bool AngleCheck(List<Vector2> P, List<int> L)
    {
        int next = L.Count - 1;
        int cur = L.Count - 2;
        int prev = L.Count - 3;

        return Utils.isConvex(P[L[prev]], P[L[cur]], P[L[next]]) &&
               Utils.isConvex(P[L[cur]], P[L[next]], P[L[0]]) &&
               Utils.isConvex(P[L[next]], P[L[0]], P[L[1]]);
    }

    bool AngleCheckCCW(List<Vector2> P, List<int> L)
    {
        int next = 0;
        int cur = 1;
        int prev = 2;

        int last = L.Count - 1;
        int ll  = last - 1;

        return Utils.isConvex(P[L[next]], P[L[cur]], P[L[prev]]) &&
                Utils.isConvex(P[L[last]], P[L[next]], P[L[cur]]) &&
                Utils.isConvex(P[L[ll]], P[L[last]], P[L[next]]);
    }

    //DebugDraw stuff
    void DrawOutline()
    {
        List<Vector2> verts = new List<Vector2>(polygon.GetPath(0));
        //transform to world space
        for (int i = 0; i < verts.Count; ++i)
        {
            verts[i] = transform.TransformPoint((Vector2)verts[i]);
        }

        for (int i = 0; i < verts.Count; ++i)
        {
            int s = i;
            int e = Next(i, verts.Count);

            //if (i == 0)
            //{
            //    Debug.DrawRay(polyVerts[s], Vector2.up * 0.5f, Color.red);
            //    Debug.DrawRay(polyVerts[e], Vector2.up * 0.5f, Color.magenta);
            //}

            Debug.DrawLine(verts[s], verts[e], Color.green);
        }
    }
}
