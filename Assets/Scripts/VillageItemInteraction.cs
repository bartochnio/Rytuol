using UnityEngine;
using System.Collections;

public class VillageItemInteraction : MonoBehaviour {

	void OnMouseDown() {
		IVillageItem item = (IVillageItem)transform.parent.GetComponent<VillageItem> ();
		if (item != null) {
			item.Select ();
		}
	}
}
