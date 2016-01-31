using UnityEngine;
using System.Collections.Generic;

public enum VillageItemEnum
{
	eSavage,
	eFruit,
	eAnimal,
    eSelfSacrifice
}

public interface IVillageItem {
	VillageItemEnum ItemType { get; }

	void Select();
	void Unselect();

	bool IsSafeToKill { get; }
	void Kill();
}

public interface IPeon {
	void MoveToPeonsArea();

	void SeekForestItem(IForestItem item);
	void StoreForestItem(IForestItem item);

	void SeekVillageItem(IVillageItem item, int queueSlot);
	void RetrieveVillageItem(IVillageItem item);

	void Sacrifice (Temple temple, Vector3 templeLocation);
    void SelfSacrifice(int queueSlot);

	VillageItemEnum ItemToSacrifice { get; }

	bool IsSafeToKill { get; }
	void Kill();
}

public interface IStorageArea {
	VillageItemEnum ContainedItemType { get; }
	Vector3 AnyLocation { get; }
    List<VillageItem> itemsList { get; }
}

public interface IPeonsArea {
	Vector3 AnyLocation { get; }
    void RequestSelfSacrifice();
}


public interface IVillage {
	IPeonsArea PeonsArea { get; }
	IStorageArea SavagesArea { get; }
	IStorageArea FruitsArea { get; }
	IStorageArea AnimalsArea { get; }

    void RegisterPeon(IPeon peon);
	bool IsPeonRegistered (IPeon peon);

	void StoreItem(Vector2 pos, ForestItemEnum itmType);

	void OrderCaptureItem(IForestItem item);

    void OrderSacrificeItem(IVillageItem item, int queueSlot);
}

