using UnityEngine;
using System.Collections;

public class ForestItem : MonoBehaviour, IForestItem {
// public vars
	public ForestItemEnum itemType;


// private vars
	bool bSelected;


// MonoBehaviour
//
	void OnMouseDown() {
		switch (itemType) {
		case ForestItemEnum.eSavage:
			break;

		case ForestItemEnum.eFruit:
			break;

		case ForestItemEnum.eAnimal:
			break;
		}
	}


// IForestItem
	public ForestItemEnum ItemType {
		get { return itemType; }
	}

	public Vector3 Location {
		get {
			Vector3 pos = transform.position;
			return new Vector3 (pos.x, 0.0f, pos.z);
		}
	}

	public void Select() {
		if (bSelected)
			return;
		bSelected = true;

		switch (itemType) {
		case ForestItemEnum.eSavage:
			Village.GetGlobalInstance ().RegisterForestSavageSelection (this);
			break;

		case ForestItemEnum.eFruit:
			Village.GetGlobalInstance ().RegisterForestFruitSelection (this);
			break;

		case ForestItemEnum.eAnimal:
			Village.GetGlobalInstance ().RegisterForestAnimalSelection (this);
			break;
		}
	}

	public void Unselect() {
		bSelected = false;
		transform.rotation = Quaternion.identity;
	}
}
