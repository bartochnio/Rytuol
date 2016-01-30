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

	void RegisterVillageSavageSelection(IVillageItem item);
	void RegisterVillageFruitSelection(IVillageItem item);
	void RegisterVillageAnimalSelection(IVillageItem item);

	void RegisterForestSavageSelection(IForestItem item);
	void RegisterForestFruitSelection(IForestItem item);
	void RegisterForestAnimalSelection(IForestItem item);
}

