using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TempResourceButton : MonoBehaviour {

    public enum ResourceState
    {
        Spawned,
        Collected
    }
    TempResourcesSpawner spawner;
    public ResourceState myState;

    public Pyramid.SacrificeType sacType;

    public void ButtonUsed ()
    {
        spawner.HandleButtonClick(this);
    }
	// Use this for initialization
	void Start () {

        spawner = TempResourcesSpawner.instance;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
