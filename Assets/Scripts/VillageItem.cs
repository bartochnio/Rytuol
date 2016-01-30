using UnityEngine;
using System.Collections;

public class VillageItem : MonoBehaviour, IVillageItem {
// public vars
	public VillageItemEnum itemType;


// private vars
	bool bSelected = false;


// MonoBehaviour
//
	void Update() {
		if (bSelected) {
			transform.Rotate (Vector3.up, 360.0f * Time.deltaTime);
		}
	}

	void OnMouseDown() {
		Select ();
	}



// IVillageItem
//
	public VillageItemEnum ItemType {
		get { return itemType; }
	}

	public void Select() {
		if (bSelected)
			return;
		bSelected = true;

		switch (itemType) {
		case VillageItemEnum.eSavage:
				Village.GetGlobalInstance ().RegisterVillageSavageSelection (this);
				break;

		case VillageItemEnum.eFruit:
				Village.GetGlobalInstance ().RegisterVillageFruitSelection (this);
				break;

		case VillageItemEnum.eAnimal:
				Village.GetGlobalInstance ().RegisterVillageAnimalSelection (this);
				break;
		}
	}

	public void Unselect() {
		bSelected = false;
		transform.rotation = Quaternion.identity;
	}
}
