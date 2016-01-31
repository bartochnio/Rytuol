using UnityEngine;

public enum ForestItemEnum
{
	eSavage,
	eFruit,
	eAnimal
}


public interface IForestItem {
	ForestItemEnum ItemType { get; }

	Vector3 Location { get; }

	void Select();
	void Unselect();

	bool IsSafeToKill { get; }
	void Kill();

    void ApplePicked();
}


public interface IForest {
	Vector3 AnyLocation { get; }
	Vector3 AnyLocation2 { get; }

	void OnAnimalTamed();
	void OnSavageCaptured();
}