using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Waypoint
{
    public Vector2 e1;
    public Vector2 e2;
    public Vector2 pos;

    public Waypoint(Vector2 a, Vector2 b, Vector2 p)
    {
        e1 = a;
        e2 = b;
        pos = p;
    }
}

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
        public Vector2 pos;
        public Vector2 e1;
        public Vector2 e2;

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

    public List<Waypoint> FindPath(Vector2 pos, Vector2 target)
    {
        List<Waypoint> path = new List<Waypoint>();
        int startIdx = navMesh.FindContainingNode(pos);
        int endIdx = navMesh.FindContainingNode(target);

        if (startIdx < 0 || endIdx < 0)
        {
            Debug.Log("Could not find containing nodes!");
            return path;
        }

        if (startIdx == endIdx)
        {
            path.Add(new Waypoint(Vector2.zero, Vector2.zero,pos));
            path.Add(new Waypoint(Vector2.zero, Vector2.zero, target));
            return path;
        }

        //NavMesh2D.Node startNode = navMesh.GetNode(startIdx);

        Node startNode = new Node();
        startNode.Gcost = 0.0f;
        startNode.Hcost = 0.0f;
        startNode.NavNodeIdx = startIdx;
        startNode.parent = null;
        startNode.pos = pos;
        List<Node> openList = new List<Node>();
        openList.Add(startNode);

        List<Node> closedList = new List<Node>();

        bool foundPath = false;

        //int count = 10;
        Node node = startNode;
        while (openList.Count > 0 || !foundPath)
        {
            openList.Sort();
            node = openList[0];
            openList.RemoveAt(0);
            closedList.Add(node);

            NavMesh2D.Node navNode = navMesh.GetNode(node.NavNodeIdx);

            if (node.NavNodeIdx == endIdx)
            {
                foundPath = true;
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

                    NavMesh2D.Edge edge = navNode.GetEdge(idx);
                    Vector2 childPos = ((edge.e2 - edge.e1) / 2.0f) + edge.e1;

                    child.Gcost = Vector2.Distance(node.pos, childPos);
                    child.Hcost = Vector2.Distance(childPos, target);
                    child.parent = node;
                    child.pos = childPos;
                    child.NavNodeIdx = idx;
                    child.e1 = edge.e1;
                    child.e2 = edge.e2;
                    openList.Add(child);
                }
                else
                {
                    Node child = openList.First(w => w.NavNodeIdx == idx);
                    float G = Vector2.Distance(node.pos, child.pos) + node.Gcost;

                    if (child.Gcost > G)
                    {
                        child.Gcost = G;
                        child.parent = node;
                    }
                }
            }
        }

        //construct path
        if (foundPath)
        {
            //path.Add(target);

            Node n = node;
            n.pos = target;
            Vector3 pNext = target;
            while (n.parent != null)
            {
                NavMesh2D.Node nav = navMesh.GetNode(n.NavNodeIdx);

                //if (n.parent != null)
                //{

                //}

                path.Add(new Waypoint(n.e1,n.e2,n.pos));
                n = n.parent;
            }

            //path.Add(pos);
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
