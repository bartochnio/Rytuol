using UnityEngine;

public enum VillageItemEnum
{
	eSavage,
	eFruit,
	eAnimal
}

public enum PeonCommands
{
	eCaptureSavage,
	eGatherFruit,
	eTameAnimal,

	eStoreSavage,
	eStoreFruit,
	eStoreAnimal,

	eRetrieveSavage,
	eRetrieveFruit,
	eRetrieveAnimal,

	eOfferSavage,
	eOfferFruit,
	eOfferAnimal
}

public interface IVillageItem {
	VillageItemEnum ItemType { get; }

	void Select();
	void Unselect();
}

public interface IPeon {
	void MoveToPeonsArea();
    void StoreForestItem(IForestItem item);
	void SeekVillageItem(IVillageItem item);
	void SeekForestItem(IForestItem item);
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

	void StoreSavage(Vector2 pos);
	void StoreFruit(Vector2 pos);
	void StoreAnimal(Vector2 pos);

	void OrderCaptureSavage(IForestItem item);
	void OrderGatheringFruit(IForestItem item);
	void OrderHuntAnimal(IForestItem item);
}

