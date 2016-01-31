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
	float spawnWaitTime = 0.0f;
	int savagesCount = 0;
	int animalsCount = 0;


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
					if (savagesCount < 10) {
						++savagesCount;

						GameObject GO = GameObject.Instantiate (savagePrefab, AnyLocation, Quaternion.identity) as GameObject;
						GO.transform.parent = itemsArea.transform;//Forest.GetGlobalInstance ().transform;
					}
				}
				break;

			case 1: {
					if (animalsCount < 10) {
						++animalsCount;

						GameObject GO = GameObject.Instantiate (animalPrefab, AnyLocation, Quaternion.identity) as GameObject;
						GO.transform.parent = itemsArea.transform; //Forest.GetGlobalInstance ().transform;
					}
				}
				break;
			}
		}
	}



// IForest
//
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
