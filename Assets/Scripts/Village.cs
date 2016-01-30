using UnityEngine;
using System.Collections.Generic;

public class Village : MonoBehaviour, IVillage {
// global instance
	static private Village globalInstance = null;
	static public Village GetGlobalInstance() {
		return globalInstance;
	}


// public vars
	public GameObject peonsAreaGO;
	public GameObject savagesAreaGO;
	public GameObject fruitsAreaGO;
	public GameObject animalsAreaGO;

	public float spawnDelay = 3.0f;
	public GameObject peonPrefab;
	public GameObject spawnPoint;


// private vars
	float spawnWaitTime = 3.0f;

	List<IPeon> peons = new List<IPeon>();
	//List<IVillageItem> selectedVillageItems = new List<IVillageItem>();
	//List<IVillageItem> selectedForestItems = new List<IForestItem>();



// MonoBehaviour
//
	void Awake () {
		Village.globalInstance = this;
	}

	void Update () {
		spawnWaitTime += Time.deltaTime;
		if (spawnWaitTime > spawnDelay) {
			spawnWaitTime = 0.0f;

			if (peons.Count < 3) {
				GameObject peonGO = GameObject.Instantiate (peonPrefab, spawnPoint.transform.position, Quaternion.identity) as GameObject;
				peonGO.transform.parent = Village.GetGlobalInstance ().transform;

				IPeon peonItem = peonGO.GetComponent<Peon> ();
				if (peonItem != null)
                {
					peonItem.MoveToPeonsArea();
				}
			}
		}
	}

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


    public void RegisterVillageSavageSelection(IVillageItem item) {
	}

	public void RegisterVillageFruitSelection(IVillageItem item) {
	}

	public void RegisterVillageAnimalSelection(IVillageItem item) {
	}


    // TODO: consider merging this 3 functions

	public void OrderCaptureSavage(IForestItem item)
    {
        IPeon p = PopPeon();

        if (p != null)
            p.SeekForestItem(item);
        else
            item.Unselect();
    }

	public void OrderGatheringFruit(IForestItem item)
    {
        IPeon p = PopPeon();

        if (p != null)
            p.SeekForestItem(item);
        else
            item.Unselect();
	}

	public void OrderHuntAnimal(IForestItem item)
    {
        IPeon p = PopPeon();

        if (p != null)
            p.SeekForestItem(item);
        else
            item.Unselect();
    }
}
