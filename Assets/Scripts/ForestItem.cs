using UnityEngine;
using System.Collections;

public class ForestItem : MonoBehaviour, IForestItem {
// public vars
	public ForestItemEnum itemType;


// private vars
	bool bSelected;
	bool bAppleExists = false;

    Apple apple;

// MonoBehaviour
//
	void OnMouseDown()
    {
		if (!bAppleExists)
			return;
		
        Select();
    }


// public functions
//

    //called from animation
	public void AppleSpawned() {
		bAppleExists = true;
	}

	public void ApplePicked()
    {
        apple = GetComponentInChildren<Apple>();
        Animator animator = apple.GetComponent<Animator>();
        bAppleExists = false;
        animator.Play("apple_in");
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

		Village.GetGlobalInstance ().OrderCaptureItem (this);
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

	public bool IsSafeToKill {
		get {
			return !bSelected;
		}
	}

	public void Kill() {
		GameObject.Destroy (gameObject);
	}

    public void Start()
    {

    }

    public void Update()
    {

    }
}
