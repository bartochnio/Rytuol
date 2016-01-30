using UnityEngine;
using System.Collections.Generic;

public class ThunderStorm : MonoBehaviour, IThunderStorm {
// global instance
	static private ThunderStorm globalInstance = null;
	static public ThunderStorm GlobalInstance() {
		return globalInstance;
	}

// public vars
	public GameObject peonsContainer;
	public GameObject forestItemsContainer;
	public GameObject villageSavagesContainer;
	public GameObject villageFruitsContainer;
	public GameObject villageAnimalsContainer;


// MonoBehaviour
//
	void Awake() {
		ThunderStorm.globalInstance = this;
	}

	void LateUpdate() {
		// kill things here
	}


// IThunderInterface
//
	public int KillPeons(int count) {
		int nPeonsKilled = 0;

		var peons = peonsContainer.GetComponentsInChildren<Peon> ();
		int nPeons = peons.Length;

		int nAttempts = nPeons * 2;

		while (nPeons != 0) {
			int idx = Random.Range (0, nPeons);
			var peon = peons [idx];

			if (peon.IsSafeToKill) {
				++nPeonsKilled;

				peon.Kill ();

				//swap delete
				peons[idx] = peons[nPeons - 1];
				--nPeons;
			}

			if (nPeonsKilled == count)
				break;

			--nAttempts;
			if (nAttempts == 0)
				break;
		}

		return nPeonsKilled;
	}


	public int KillForestItems(int count) {
		int nItemsKilled = 0;

		var items = forestItemsContainer.GetComponentsInChildren<IForestItem> ();
		int nItems = items.Length;

		int nAttempts = nItems * 2;

		while (nItems != 0) {
			int idx = Random.Range (0, nItems);

			bool isSafe = false;
			GameObject go = null;
			{
				Critter critter = items [idx] as Critter;
				if (critter != null) {
					go = critter.gameObject;
					isSafe = critter.IsSafeToKill;
				}
				else {
					ForestItem fi = items [idx] as ForestItem;
					if (fi != null) {
						go = fi.gameObject;
						isSafe = fi.IsSafeToKill;
					}
				}
			}

			if (go != null && isSafe) {
				++nItemsKilled;

				items [idx].Kill ();

				//swap delete
				items[idx] = items[nItems - 1];
				--nItems;
			}

			if (nItemsKilled == count)
				break;

			--nAttempts;
			if (nAttempts == 0)
				break;
		}

		return nItemsKilled;
	}


	public int KillVillageItems(int count) {
		int nItemsKilled = 0;

		VillageItem[] items = villageSavagesContainer.transform.parent.GetComponentsInChildren<VillageItem> ();

		int nItems = items.Length;

		int nAttempts = nItems * 2;

		while (nItems != 0) {
			int idx = Random.Range (0, nItems);
			var item = items [idx];

			if (item.IsSafeToKill) {
				++nItemsKilled;

				item.Kill ();

				//swap delete
				items[idx] = items[nItems - 1];
				--nItems;
			}

			if (nItemsKilled == count)
				break;

			--nAttempts;
			if (nAttempts == 0)
				break;
		}

		return nItemsKilled;
	}


	public int KillForestItemsOfType(ForestItemEnum itmType, int count) {
		int nItemsKilled = 0;

		IForestItem[] allItems = forestItemsContainer.GetComponentsInChildren<IForestItem> ();

		List<IForestItem> items = new List<IForestItem> ();
		foreach (var itm in allItems) {
			if (itm.ItemType == itmType)
				items.Add (itm);
		}

		int nItems = items.Count;

		int nAttempts = nItems * 2;

		while (nItems != 0) {
			int idx = Random.Range (0, nItems);
			bool isSafe = false;
			GameObject go = null;
			{
				Critter critter = items [idx] as Critter;
				if (critter != null) {
					go = critter.gameObject;
					isSafe = critter.IsSafeToKill;
				}
				else {
					ForestItem fi = items [idx] as ForestItem;
					if (fi != null) {
						go = fi.gameObject;
						isSafe = fi.IsSafeToKill;
					}
				}
			}

			if (go != null && isSafe) {
				++nItemsKilled;

				items [idx].Kill ();

				//swap delete
				items[idx] = items[nItems - 1];
				items.RemoveAt(nItems - 1);
				--nItems;
			}

			if (nItemsKilled == count)
				break;

			--nAttempts;
			if (nAttempts == 0)
				break;
		}

		return nItemsKilled;
	}


	public int KillVillageItemsOfType(VillageItemEnum itmType, int count) {
		int nItemsKilled = 0;

		VillageItem[] items = null;
		switch (itmType) {
		case VillageItemEnum.eSavage:
			items = villageSavagesContainer.GetComponentsInChildren<VillageItem> ();
			break;
		case VillageItemEnum.eFruit:
			items = villageFruitsContainer.GetComponentsInChildren<VillageItem> ();
			break;
		case VillageItemEnum.eAnimal:
			items = villageAnimalsContainer.GetComponentsInChildren<VillageItem> ();
			break;
		}

		int nItems = items.Length;

		int nAttempts = nItems * 2;

		while (nItems != 0) {
			int idx = Random.Range (0, nItems);
			var item = items [idx];

			if (item.IsSafeToKill) {
				++nItemsKilled;

				item.Kill();

				//swap delete
				items[idx] = items[nItems - 1];
				--nItems;
			}

			if (nItemsKilled == count)
				break;

			--nAttempts;
			if (nAttempts == 0)
			break;
		}

		return nItemsKilled;
	}
	
	
	public void DamagePeonSpawner() {
	}
	
	
	public void DamageForestItemSpawner() {
	}
}
