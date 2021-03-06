﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Village : MonoBehaviour, IVillage {
// global instance
	static private Village globalInstance = null;
	static public Village GetGlobalInstance() {
		return globalInstance;
	}


// public vars
    public GameObject savageResourcePrefab;
    public GameObject animalResourcePrefab;
    public GameObject fruitResourcePrefab;

    public GameObject peonsAreaGO;
	public GameObject savagesAreaGO;
	public GameObject fruitsAreaGO;
	public GameObject animalsAreaGO;

	public float spawnDelay = 3.0f;
	public GameObject peonPrefab;
	public GameObject spawnPoint;

    public int startingPeonCount = 10;

// private vars
	float spawnWaitTime = 3.0f;
    

	List<IPeon> peons = new List<IPeon>();


// MonoBehaviour
//
    void Awake () {
		Village.globalInstance = this;
	}

    void Start()
    {
        for(int i = 0; i < startingPeonCount; ++i)
        {
            SpawnPeon(spawnPoint.transform.position);
        }
    }

	void Update () {
		spawnWaitTime += Time.deltaTime;
		if (spawnWaitTime > spawnDelay) {
			spawnWaitTime = 0.0f;

			if (peons.Count < 3) {
                SpawnPeon(spawnPoint.transform.position);
			}
		}
	}


    public void SpawnPeon(Vector2 pos)
    {
        GameObject peonGO = GameObject.Instantiate(peonPrefab, pos, Quaternion.identity) as GameObject;
        peonGO.transform.parent = spawnPoint.transform;//Village.GetGlobalInstance ().transform;

        IPeon peonItem = peonGO.GetComponent<Peon>();
        if (peonItem != null)
        {
            peonItem.MoveToPeonsArea();
        }
    }

// public functions
    IPeon PopPeon()
    {
        if (peons.Count == 0)
            return null;

        int idx = peons.Count - 1;
        IPeon p = peons[idx];
        peons.RemoveAt(idx);

        return p;
    }


// IVillage
//
	public IPeonsArea PeonsArea {
		get { return (IPeonsArea)peonsAreaGO.GetComponent<PeonsArea> (); }
	}

	public IStorageArea SavagesArea {
		get { return (IStorageArea)savagesAreaGO.GetComponent<IStorageArea> (); }
	}

	public IStorageArea FruitsArea {
		get { return (IStorageArea)fruitsAreaGO.GetComponent<IStorageArea> (); }
	}

	public IStorageArea AnimalsArea {
		get { return (IStorageArea)animalsAreaGO.GetComponent<IStorageArea> (); }
	}

    public void RegisterPeon(IPeon peon)
    {
        peons.Add(peon);
    }

	public bool IsPeonRegistered(IPeon peon) {
		return (peons.Find (x => x == peon) != null);
	}

	public void UnregisterPeon (IPeon peon) {
		peons.Remove (peon);
	}


	public void StoreItem(Vector2 pos, ForestItemEnum itmType)
    {
        StorageArea area;
		switch(itmType)
		{
		case ForestItemEnum.eSavage:
			{
				GameObject go = GameObject.Instantiate (savageResourcePrefab, pos, Quaternion.identity) as GameObject;
                area = SavagesArea as StorageArea;
                    area.itemsList.Add(go.GetComponent<VillageItem>());
				go.transform.parent = (SavagesArea as StorageArea).transform;
			}
			break;

		case ForestItemEnum.eAnimal:
			{
				GameObject go = GameObject.Instantiate (animalResourcePrefab, pos, Quaternion.identity) as GameObject;
                area = AnimalsArea as StorageArea;
                    area.itemsList.Add(go.GetComponent<VillageItem>());
				go.transform.parent = (AnimalsArea as StorageArea).transform;
			}
			break;

		case ForestItemEnum.eFruit:
			{
				GameObject go = GameObject.Instantiate (fruitResourcePrefab, pos, Quaternion.identity) as GameObject;
                area = FruitsArea as StorageArea;
                    area.itemsList.Add(go.GetComponent<VillageItem>());
				go.transform.parent = (FruitsArea as StorageArea).transform;
			}
			break;
		}
    }


	public void OrderCaptureItem(IForestItem item)
    {
        IPeon p = PopPeon();

        if (p != null)
            p.SeekForestItem(item);
        else
            item.Unselect();
    }


    public void OrderSacrificeItem(IVillageItem item, int queueSlot)
    {
        IPeon p = PopPeon();

        if (p != null)
            p.SeekVillageItem(item, queueSlot);
        else
        {
            item.Unselect();
            SacrificeQueue.GetInstance().FreeSlot(queueSlot);
        }
    }

    public void OrderConvertSavage(IVillageItem item)
    {
        IPeon p = PopPeon();

        if (p != null)
            p.SeekSavageToConvert(item);
    }

    public void OrderSelfSacrifice (int queueSlot)
    {
        IPeon p = PopPeon();
        if ( p != null )
        {
            p.SelfSacrifice( queueSlot );
        }
        else
        {
            SacrificeQueue.GetInstance().FreeSlot(queueSlot);
        }
    }
}
