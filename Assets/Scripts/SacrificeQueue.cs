using UnityEngine;
using System.Collections.Generic;

public class SacrificeQueue : MonoBehaviour
{
	private static SacrificeQueue instance = null;
	public static SacrificeQueue GetInstance()
	{
		return instance;
	}


    public int queueSize = 5;
    public Transform startingPos;
	public float offset = 0.5f;


    int[] slots;
    IPeon[] peons;

    

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



    public int ReserveSlot()
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

	public void FreeSlot(int idx) //he he he
	{
		slots[idx] = 0;
		peons [idx] = null;
	}

	public void ClaimSlot(IPeon p, int slotIdx)
	{
		peons[slotIdx] = p;
	}


	public void DispatchToTheTemple(Temple temple, Vector2 templePos)
    {
        for(int i = 0; i < queueSize; ++i)
        {
            if (peons[i] != null)
            {
				peons [i].Sacrifice (temple, templePos);

				FreeSlot (i);
            }
        }
    }


	public void Reset()
    {
        for (int i = 0; i < queueSize; ++i)
        {
            slots[i] = 0;
            peons[i] = null;
        }
    }


    public Vector2 GetSlotPos(int idx)
    {
		Vector2 pos = (Vector2)startingPos.position + ((Vector2)startingPos.right)*offset*idx;

        return pos;
    }

}
