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
}


public interface IForest {
	IForestItem FindItem (ForestItemEnum itmType);
}