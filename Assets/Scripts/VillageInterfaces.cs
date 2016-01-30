using UnityEngine;

public enum VillageItemEnum
{
	eSavage,
	eFruit,
	eAnimal
}

public interface IVillageItem {
	VillageItemEnum ItemType { get; }

	void Select();
	void Unselect();
}

public interface IPeon {
	void MoveToPeonsArea();

	void SeekForestItem(IForestItem item);
	void StoreForestItem(IForestItem item);

	void SeekVillageItem(IVillageItem item, int queueSlot);
	void RetrieveVillageItem(IVillageItem item);

	void Sacrifice (Temple.ID templeId, Vector3 templeLocation);

	VillageItemEnum ItemToSacrifice { get; }
}

public interface IStorageArea {
	VillageItemEnum ContainedItemType { get; }
	Vector3 AnyLocation { get; }
}

public interface IPeonsArea {
	Vector3 AnyLocation { get; }
}

public interface IVillage {
	IPeonsArea PeonsArea { get; }
	IStorageArea SavagesArea { get; }
	IStorageArea FruitsArea { get; }
	IStorageArea AnimalsArea { get; }

    void RegisterPeon(IPeon peon);

	void StoreItem(Vector2 pos, ForestItemEnum itmType);

	void OrderCaptureItem(IForestItem item);

    void OrderSacrificeItem(IVillageItem item, int queueSlot);
}

