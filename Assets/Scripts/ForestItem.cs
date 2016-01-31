using UnityEngine;
using System.Collections;

public class ForestItem : MonoBehaviour, IForestItem {
// public vars
	public ForestItemEnum itemType;

    public float minRespawn = 0.5f;
    public float maxRespawn = 5.0f;
    float counter = 0.0f;
    bool spawnApple = true;

// private vars
	bool bSelected;
	bool bAppleExists = false;
    float respawn = 0.0f;

    Animator appleAni;

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
        bAppleExists = false;
        spawnApple = true;
        appleAni.speed = 0.0f;
        appleAni.Play("apple_in");
        respawn = Random.Range(minRespawn, maxRespawn);
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

        if ( ItemType == ForestItemEnum.eFruit)
        {
            SetColor(Color.red, transform.parent.gameObject);
        }
        else 
            SetColor(Color.red);

		Village.GetGlobalInstance ().OrderCaptureItem (this);
	}

	public void Unselect()
    {
        if ( ItemType == ForestItemEnum.eFruit)
        {
            SetColor(Color.white, transform.parent.gameObject);
        }
        else 
            SetColor(Color.white);
        bSelected = false;
		transform.rotation = Quaternion.identity;
	}

    void SetColor(Color c)
    {
        SetColor(c, null);
    }
    void SetColor(Color c, GameObject obj)
    {

        SpriteRenderer render = GetComponentInChildren<SpriteRenderer>();
        if ( obj != null)
        {
            SpriteRenderer pRend = transform.parent.GetComponent<SpriteRenderer>();
            if ( pRend != null)
            {
                Debug.Log(" Color change " + pRend.name);
                render = pRend;
            }
        }
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
        Apple apple = GetComponentInChildren<Apple>();
        appleAni = apple.GetComponent<Animator>();
        appleAni.speed = 0.0f;
        respawn = Random.Range(minRespawn, maxRespawn);
    }

    public void Update()
    {
        if(spawnApple)
        {
            counter += Time.deltaTime;
            if (counter >= respawn)
            {
                counter = 0.0f;
                appleAni.speed = 1.0f;
                spawnApple = false;
            }
        }
    }
}
