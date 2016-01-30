using UnityEngine;
using System.Collections;

public class Temple : MonoBehaviour
{
    public float reduceSacrificeValue = 0.5f;
    public float sacrificeFullValue = 0.3f;

    public VillageItemEnum preferedSacrifice;
    public SacrificeMeter sMeter;

    public enum ID {
		Left,
		Middle,
		Right
	}
	public ID templeId;

    public void ReciveSacrifice(VillageItemEnum type)
    {
        float sValue = sacrificeFullValue;
        if (type != preferedSacrifice)
        {
            sValue *= reduceSacrificeValue;
        }
        sMeter.BoostByValue(sValue);
    }

    void OnMouseDown()
    {
		SacrificeQueue.GetInstance ().DispatchToTheTemple (this, transform.position);
    }

    void Start()
    {
        if (sMeter == null) GetComponentInChildren<SacrificeMeter>();
        if (sMeter == null) Debug.LogWarning(" There is no Sacrifice Meter reference attached to: " + name);
    }
}
