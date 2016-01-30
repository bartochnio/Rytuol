using UnityEngine;
using System.Collections;

public interface IThunderStorm {

	int KillPeons (int count);


	int KillForestItems (int count);


	int KillVillageItems (int count);


	int KillForestItemsOfType (ForestItemEnum itmType, int count);


	int KillVillageItemsOfType (VillageItemEnum itmType, int count);


	void DamagePeonSpawner ();


	void DamageForestItemSpawner ();
}
