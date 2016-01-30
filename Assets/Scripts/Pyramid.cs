using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Pyramid : MonoBehaviour, IPointerDownHandler {

    // Multiplier for not prefered sacrifice
    public const float REDUCTED_SACRIFICE_VALUE = 0.5f;
    public enum SacrificeType
    {
        Human,
        Goat,
        Fruits
    }
    public SacrificeType preferedSacrifice;
    public SacrificeMeter sMeter;

    private TempResourcesSpawner spawner;
    public float sacrificeFullValue = 0.3f;

    public void ReciveSacrifice(SacrificeType type)
    {
        float sValue = sacrificeFullValue;
        if ( type != preferedSacrifice)
        {
            sValue *= REDUCTED_SACRIFICE_VALUE;
        }
        sMeter.BoostByValue(sValue);
    }

    public void OnPointerDown (PointerEventData data)
    {
        spawner.RequestSacrifice(this) ;
    }
	// Use this for initialization
	void Start () {

        if (sMeter == null) GetComponentInChildren<SacrificeMeter>();
        if (sMeter == null) Debug.LogWarning(" There is no Sacrifice Meter reference attached to: " + name);

        spawner = TempResourcesSpawner.instance;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
