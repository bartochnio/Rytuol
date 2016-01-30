using UnityEngine;
using System.Collections.Generic;

public class Forest : MonoBehaviour, IForest {
// global instance
	static private Forest globalInstance = null;
	static public Forest GetGlobalInstance() {
		return globalInstance;
	}


// public vars
	public GameObject savagePrefab;
	public GameObject fruitPrefab;
	public GameObject animalPrefab;

	public float spawnDelay = 1.0f;

	public GameObject forestArea;	
	public GameObject itemsArea;



// private vars
	public List<IForestItem> savages = new List<IForestItem> ();
	public List<IForestItem> fruits = new List<IForestItem> ();
	public List<IForestItem> animals = new List<IForestItem> ();

	float spawnWaitTime = 0.0f;


// MonoBehaviour
//
	void Awake () {
		Forest.globalInstance = this;
	}

	void Update () {
		spawnWaitTime += Time.deltaTime;
		if (spawnWaitTime > spawnDelay) {
			spawnWaitTime = 0.0f;

			int chance = Random.Range (0, 1000) % 4;
			switch (chance) {
			case 0: {
					if (savages.Count < 10) {
						GameObject GO = GameObject.Instantiate (savagePrefab, AnyLocation, Quaternion.identity) as GameObject;
						GO.transform.parent = itemsArea.transform;//Forest.GetGlobalInstance ().transform;

						IForestItem item = (IForestItem)GO.GetComponent<ForestItem>();
						if (item != null) {
							savages.Add (item);
						}
					}
				}
				break;

			case 1: {
					if (animals.Count < 10) {
						GameObject GO = GameObject.Instantiate (animalPrefab, AnyLocation, Quaternion.identity) as GameObject;
						GO.transform.parent = itemsArea.transform; //Forest.GetGlobalInstance ().transform;

						IForestItem item = (IForestItem)GO.GetComponent<ForestItem>();
						if (item != null) {
							animals.Add (item);
						}
					}
				}
				break;
			}
		}
	}



// IForest
//
	public IForestItem FindItem (ForestItemEnum itmType) {
		IForestItem returnItem = null;

		switch (itmType) {
		case ForestItemEnum.eSavage:
			if (savages.Count > 0) {
				int lastItemIndex = savages.Count - 1;
				returnItem = savages [lastItemIndex];
				savages.RemoveAt (lastItemIndex);
			}
			break;

		case ForestItemEnum.eFruit:
			if (savages.Count > 0) {
				int lastItemIndex = savages.Count - 1;
				returnItem = fruits [lastItemIndex];
				fruits.RemoveAt (lastItemIndex);
			}
			break;

		case ForestItemEnum.eAnimal:
			if (savages.Count > 0) {
				int lastItemIndex = savages.Count - 1;
				returnItem = animals [lastItemIndex];
				animals.RemoveAt (lastItemIndex);
			}
			break;
		}

		return returnItem;
	}

	public Vector3 AnyLocation {
		get {
			var renderer = forestArea.GetComponentInChildren<BoxCollider2D> ();
			var bounds = renderer.bounds;
			Vector3 minPt = bounds.min;
			Vector3 maxPt = bounds.max;
			return new Vector3( Random.Range(minPt.x, maxPt.x), Random.Range(minPt.y, maxPt.y), 0.0f  );
		}
	}
}
