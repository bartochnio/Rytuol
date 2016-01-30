using UnityEngine;
using System.Collections;

public class ForestItem : MonoBehaviour, IForestItem {
// public vars
	public ForestItemEnum itemType;


// private vars
	bool bSelected;


// MonoBehaviour
//
	void OnMouseDown()
    {
        Select();
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

	public void Select()
    {
		if (bSelected)
			return;
		bSelected = true;

        SetColor(Color.red);

        switch (itemType) {
		case ForestItemEnum.eFruit:
			Village.GetGlobalInstance ().OrderGatheringFruit (this);
			break;
		}
	}

	public void Unselect()
    {
        SetColor(Color.white);
        bSelected = false;
		transform.rotation = Quaternion.identity;
	}

    void SetColor(Color c)
    {
        SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
        render.color = c;
    }
}
