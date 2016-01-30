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

			if (peons.Count < 10) {
				GameObject peonGO = GameObject.Instantiate (peonPrefab, spawnPoint.transform.position, Quaternion.identity) as GameObject;
				peonGO.transform.parent = Village.GetGlobalInstance ().transform;

				IPeon peonItem = (IPeon)peonGO.GetComponent<PeonAnmol> ();
				if (peonItem != null) {
					peonItem.MoveToPeonsArea ();
				}
			}
		}
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


	public void RegisterVillageSavageSelection(IVillageItem item) {
	}

	public void RegisterVillageFruitSelection(IVillageItem item) {
	}

	public void RegisterVillageAnimalSelection(IVillageItem item) {
	}


	public void RegisterForestSavageSelection(IForestItem item) {
	}

	public void RegisterForestFruitSelection(IForestItem item) {
	}

	public void RegisterForestAnimalSelection(IForestItem item) {
	}
}
