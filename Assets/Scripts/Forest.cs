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
	public int savagesTargetPopulation = 20;
	public int animalTargetPopulation = 20;

	public GameObject[] forestArea;	
	public GameObject itemsArea;



// private vars
	public List<IForestItem> savages = new List<IForestItem> ();
	public List<IForestItem> fruits = new List<IForestItem> ();
	public List<IForestItem> animals = new List<IForestItem> ();

	float spawnWaitTime = 0.0f;
	int nSavages = 0;
	int nAnimals = 0;


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
					if (nSavages < savagesTargetPopulation) {
						++nSavages;

						GameObject GO = GameObject.Instantiate (savagePrefab, AnyLocation, Quaternion.identity) as GameObject;
						GO.transform.parent = itemsArea.transform;//Forest.GetGlobalInstance ().transform;
					}
				}
				break;

			case 1: {
					if (nAnimals < animalTargetPopulation) {
						++nAnimals;

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
			var renderer = forestArea[0].GetComponentInChildren<BoxCollider2D> ();

			var bounds = renderer.bounds;
			Vector3 minPt = bounds.min;
			Vector3 maxPt = bounds.max;

			Vector2 size = renderer.size;
			Vector2 randomPoint = new Vector2(Random.Range(-size.x / 2.0f, size.x / 2.0f), Random.Range(-size.y / 2.0f, size.y / 2.0f));
			randomPoint = forestArea[0].transform.TransformPoint(randomPoint);

			return randomPoint;
		}
	}

	public Vector3 AnyLocation2 {
		get {
			int idx = Random.Range (0, 100) % forestArea.Length;
			var renderer = forestArea[idx].GetComponentInChildren<BoxCollider2D> ();

			var bounds = renderer.bounds;
			Vector3 minPt = bounds.min;
			Vector3 maxPt = bounds.max;

			Vector2 size = renderer.size;
			Vector2 randomPoint = new Vector2(Random.Range(-size.x / 2.0f, size.x / 2.0f), Random.Range(-size.y / 2.0f, size.y / 2.0f));
			randomPoint = forestArea[idx].transform.TransformPoint(randomPoint);

			return randomPoint;
		}
	}
}
