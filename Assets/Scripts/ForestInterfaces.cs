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
}


public interface IForest {
	IForestItem FindItem (ForestItemEnum itmType);
}