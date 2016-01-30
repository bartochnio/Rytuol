using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TempResourcesSpawner : MonoBehaviour {

    public float timeToMoveToCollected = 3f;
    public float timeBetweenSacrifices = 1f;

    public float minimalSpawnInterval = 1f;
    public float maximalSpawnInterval = 3f;

    public int maxSpawnedItems = 5;

    public Object tempButtonPrefab;
    public RectTransform storageTransform;
    public RectTransform harvestTransform;

    private static TempResourcesSpawner _instance;
    public static TempResourcesSpawner instance
    {
        get { return _instance; }
    }

    public TempResourceButton currentResource;
    List<TempResourceButton> spawnedList = new List<TempResourceButton>();
    List<TempResourceButton> resourceList = new List<TempResourceButton>();

    public void RequestSacrifice  (Pyramid p )
    {
        if (p != null)
        {
            if (resourceList.Count > 0 )
            {
                feed = true;
                StartCoroutine(FeedSacrifice(p));
            }
        }
    }

    bool feed = false;

    IEnumerator FeedSacrifice(Pyramid p)
    {
        while (resourceList.Count > 0)
        {
            TempResourceButton b = resourceList[0];
            b.GetComponent<Button>().interactable = false;
            yield return new WaitForSeconds(timeBetweenSacrifices);
            p.ReciveSacrifice(b.sacType);
            resourceList.Remove(b);
            Destroy(b.gameObject);
        }

        feed = false;

    }

    void Awake()
    {
        if (_instance == null) _instance = this;
        else
        {
            Debug.LogWarning ( "Second instance of singleton type: " +  this.GetType() + " \r\n Destroying! ");
            Destroy(this);
            return;
        }
    }

    public IEnumerator SpawnRoutine ()
    {
        while (true)
        {
            float time = Random.Range(minimalSpawnInterval, maximalSpawnInterval);
            Pyramid.SacrificeType sacType = (Pyramid.SacrificeType)Random.Range(0, 3);
            yield return new WaitForSeconds(time);

            if (spawnedList.Count >= maxSpawnedItems) continue;
            GameObject o = Instantiate(tempButtonPrefab) as GameObject;

            o.transform.SetParent(harvestTransform);
            TempResourceButton b = o.GetComponent<TempResourceButton>();
            b.sacType = sacType;
            spawnedList.Add(b);


            Color c = Color.red;
            switch (sacType)
            {
                case Pyramid.SacrificeType.Fruits:
                    c = Color.blue;
                    break;
                case Pyramid.SacrificeType.Goat:
                    c = Color.green;
                    break;
            }

            b.GetComponent<Image>().color = c;

        }
    }
    public void HandleButtonClick ( TempResourceButton button )
    {
        if ( button.myState == TempResourceButton.ResourceState.Spawned)
        {
            button.GetComponent<Button>().interactable = false;
            spawnedList.Remove(button);
            StartCoroutine(SendToStorage(button));
        }
        else
        {
            currentResource = button;
        }
    }

    IEnumerator SendToStorage ( TempResourceButton button)
    {
        yield return new WaitForSeconds(timeToMoveToCollected);
        button.transform.SetParent ( storageTransform );
        resourceList.Add(button);
        button.GetComponent<Button>().interactable = true;
        button.myState = TempResourceButton.ResourceState.Collected;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(SpawnRoutine());
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
