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
		//if (bSelected) {
		//	transform.Rotate (Vector3.up, 360.0f * Time.deltaTime);
		//}
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

        SetColor(Color.red);

        int queueSlot = SacrificeQueue.GetInstance().ReserveSlot();
        if (queueSlot < 0)
        {
            Unselect();
            return;
        }

		Village.GetGlobalInstance().OrderSacrificeItem(this,queueSlot);
    }

	public void Unselect() {
		bSelected = false;
		transform.rotation = Quaternion.identity;
        SetColor(Color.white);
    }

    void SetColor(Color c)
    {
        SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
        render.color = c;
    }
}
