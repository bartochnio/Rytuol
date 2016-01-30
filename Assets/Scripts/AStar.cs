using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

//[ExecuteInEditMode]
public class AStar : MonoBehaviour
{
    private NavMesh2D navMesh;
    //public Transform player;

    class Node : IComparable<Node>
    {
        public float Gcost;
        public float Hcost;
        public int NavNodeIdx;
        public Node parent;

        public int CompareTo(Node other)
        {
            float cost = Gcost + Hcost;
            float otherCost = other.Gcost + Hcost;

            return cost.CompareTo(otherCost);
        }
    }

    static private AStar instance = null;
    public static AStar GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        AStar.instance = this;
    }

    void OnEnable()
    {
        navMesh = GetComponent<NavMesh2D>();
    }

	void Start ()
    {
	
	}

    void Update ()
    {
        //List<Vector2> path = FindPath(Vector2.zero, player.position);

        ////debug draw
        //for (int i = 0; i < path.Count - 1; ++i)
        //{
        //    Debug.DrawLine(path[i], path[i + 1], Color.red);
        //}
    }

    public List<Vector2> FindPath(Vector2 pos, Vector2 target)
    {
        List<Vector2> path = new List<Vector2>();
        int startIdx = navMesh.FindContainingNode(pos);
        int endIdx = navMesh.FindContainingNode(target);

        if (startIdx < 0 || endIdx < 0)
        {
            Debug.Log("Could not find containing nodes!");
            return path;
        }

        if (startIdx == endIdx)
        {
            path.Add(pos);
            path.Add(target);
            return path;
        }

        //NavMesh2D.Node startNode = navMesh.GetNode(startIdx);

        Node startNode = new Node();
        startNode.Gcost = 0.0f;
        startNode.Hcost = 0.0f;
        startNode.NavNodeIdx = startIdx;
        startNode.parent = null;
        List<Node> openList = new List<Node>();
        openList.Add(startNode);

        List<Node> closedList = new List<Node>();

        bool foundPath = false;

        //int count = 10;
        while (openList.Count > 0 || !foundPath)
        {
            openList.Sort();
            Node node = openList[0];
            openList.RemoveAt(0);
            closedList.Add(node);

            NavMesh2D.Node navNode = navMesh.GetNode(node.NavNodeIdx);

            if (node.NavNodeIdx == endIdx)
            {
                foundPath = true;

                path.Add(target);

                Node n = node;
                Vector3 pNext = target;
                while(n.parent != null)
                {
                    NavMesh2D.Node nav = navMesh.GetNode(n.NavNodeIdx);

                    if (n.parent != null)
                    {
                        int parentIdx = n.parent.NavNodeIdx;
                        NavMesh2D.Edge edge = nav.GetEdge(parentIdx);

                        Vector3 vertA = edge.e1;
                        Vector3 vertB = edge.e2;

                        Vector3 deltaBA = vertB - vertA;
                        Vector3 deltaNS = pNext - (Vector3)pos;
                        Vector3 P = new Vector3(-deltaBA.z, deltaBA.y, deltaBA.x);

                        float h = (Vector2.Dot(vertA - (Vector3)pos,P)) / Vector2.Dot(deltaNS,P);
                        Vector3 p;
                        if (h < 0.0f) p = vertA;
                        else if (h > 1.0f) p = vertB;
                        else p = (Vector3)pos + deltaNS * h;

                        Vector3 res = Utils.projectPointToSegment(p, pos, pNext);
                        Vector3 pathPoint = Utils.projectPointToSegment(res, vertA, vertB);
                        path.Add(pathPoint);
                        pNext = pathPoint;
                    }

                    n = n.parent;
                }

                path.Add(pos);
                break;
            }

            //get neighbours    
            List<int> neighbours = navNode.GetNeighbours();
            
            foreach(int idx in neighbours)
            {
                //skip the ones already checked
                if (IsInList(closedList, idx))
                    continue;

                NavMesh2D.Node navChild = navMesh.GetNode(idx);

                //check if it is on openList
                if (!IsInList(openList,idx))
                {
                    //compute cost
                    Node child = new Node();
                    child.Gcost = Vector2.Distance(navNode.center, navChild.center);
                    child.Hcost = Vector2.Distance(navChild.center, target);
                    child.parent = node;
                    child.NavNodeIdx = idx;
                    openList.Add(child);
                }
                else
                {
                    //Node child = openList.First(w => w.NavNodeIdx == idx);
                    //float G = Vector2.Distance(navNode.center, navChild.center);

                    //if (child.Gcost > G)
                    //{
                    //    child.Gcost = G;
                    //    child.parent = node;
                    //}
                }
            }
        }

        path.Reverse();
        return path;
    }

    bool IsInList(List<Node> list, int idx)
    {
        foreach(Node node in list)
        {
            if (node.NavNodeIdx == idx)
                return true;
        }

        return false;
    }
}
