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

	public float savageSpawnDelay = 1.0f;
	public int savageMaxCount = 20;
	public int savageInitialCount = 5;

	public float animalSpawnDelay = 1.0f;
	public int animalMaxCount = 20;
	public int animalInitialCount = 5;

	public GameObject forestArea;
	public GameObject[] additionalForestAreas;
	public GameObject itemsArea;



// private vars
	float savageSpawnWaitTime = 0.0f;
	float animalSpawnWaitTime = 0.0f;

	int savageCount = 0;
	int animalCount = 0;


// MonoBehaviour
//
	void Awake () {
		Forest.globalInstance = this;

		savageSpawnWaitTime = Random.Range (0.0f, savageSpawnDelay);
		animalSpawnWaitTime = Random.Range (0.0f, animalSpawnDelay);
	}

	void Start() {
		for(int i = 0; i < savageInitialCount; ++i)
		{
			SpawnSavage ();
		}

		for(int i = 0; i < animalInitialCount; ++i)
		{
			SpawnAnimal ();
		}
	}

	void Update () {
		savageSpawnWaitTime += Time.deltaTime;
		if (savageSpawnWaitTime > savageSpawnDelay) {
			savageSpawnWaitTime = 0.0f;

			if (savageCount < savageMaxCount) {

				int chance = Random.Range (0, 1000) % 2;
				if (chance == 0) {
					SpawnSavage ();
				}
			}
		}

		animalSpawnWaitTime += Time.deltaTime;
		if (animalSpawnWaitTime > animalSpawnDelay) {
			animalSpawnWaitTime = 0.0f;

			if (animalCount < animalMaxCount) {

				int chance = Random.Range (0, 1000) % 2;
				if (chance == 0) {
					SpawnAnimal ();
				}
			}
		}
	}


// private function
	void SpawnAnimal() {
		GameObject GO = GameObject.Instantiate (animalPrefab, AnyLocation, Quaternion.identity) as GameObject;
		GO.transform.parent = itemsArea.transform; //Forest.GetGlobalInstance ().transform;

		++animalCount;
	}

	void SpawnSavage() {
		GameObject GO = GameObject.Instantiate (savagePrefab, AnyLocation, Quaternion.identity) as GameObject;
		GO.transform.parent = itemsArea.transform;//Forest.GetGlobalInstance ().transform;

		++savageCount;
	}


// IForest
//
	public void OnAnimalTamed() {
		--animalCount;
	}

	public void OnSavageCaptured() {
		--savageCount;
	}

	public Vector3 AnyLocation {
		get {
			var collider = forestArea.GetComponentInChildren<BoxCollider2D> ();
			Vector2 size = collider.size;
			Vector2 randomPoint = new Vector2(Random.Range(-size.x / 2.0f, size.x / 2.0f), Random.Range(-size.y / 2.0f, size.y / 2.0f));
			randomPoint = forestArea.transform.TransformPoint(randomPoint);

			return randomPoint;
		}
	}

	public Vector3 AnyLocation2 {
		get {
			if (additionalForestAreas == null)
				return AnyLocation;
			if (additionalForestAreas.Length == 0)
				return AnyLocation;
			
			int idx = Random.Range(0, 100) % additionalForestAreas.Length;
			var collider = additionalForestAreas [idx].GetComponentInChildren<BoxCollider2D> ();

			Vector2 size = collider.size;
			Vector2 randomPoint = new Vector2(Random.Range(-size.x / 2.0f, size.x / 2.0f), Random.Range(-size.y / 2.0f, size.y / 2.0f));
			randomPoint = additionalForestAreas[idx].transform.TransformPoint(randomPoint);

			return randomPoint;
		}
	}
}
