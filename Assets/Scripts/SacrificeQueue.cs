using UnityEngine;
using System.Collections.Generic;

public class SacrificeQueue : MonoBehaviour
{
    public int queueSize = 5;
    public Transform startingPos;
    public Vector2 offset;

    int nextIdx = 0;

    int[] slots;
    IPeon[] peons;

    private static SacrificeQueue instance = null;
    void Awake()
    {
        SacrificeQueue.instance = this;
    }

    void Start()
    {
        slots = new int[queueSize];
        peons = new IPeon[queueSize];
        for (int i = 0; i < queueSize; ++i)
            slots[i] = 0;
    }

    public static SacrificeQueue GetInstance()
    {
        return instance;
    }

    public int Reserve()
    {
        for (int i = 0; i < queueSize; ++i)
        {
            if (slots[i] == 0)
            {
                slots[i] = 1;
                return i;
            }
        }

        return -1;
    }

    void SendToTheTemple(Vector2 pos)
    {
        for(int i = 0; i < queueSize; ++i)
        {
            if (peons[i] != null)
            {
                //peons[i].Move
            }
        }
    }

    public void RegisterPeon(IPeon p, int slotIdx)
    {
        peons[slotIdx] = p;
    }

    public void Reset()
    {
        for (int i = 0; i < queueSize; ++i)
        {
            slots[i] = 0;
            peons[i] = null;
        }
    }

    public void Free(int idx) //he he he
    {
        slots[idx] = 0;
    }

    public Vector2 GetSlotPos(int idx)
    {
        Vector2 pos = (Vector2)startingPos.position + offset*idx;

        return pos;
    }

}
