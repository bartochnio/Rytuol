using UnityEngine;
using System.Collections;

public class Temple : MonoBehaviour
{
    public float reduceSacrificeValue = 0.5f;
    public float sacrificeFullValue = 0.3f;

    public string bonusEvent;
    public string punishmentEvent;

    public float belowStartVal = 0.0f;
    public float aboveStartVal = 1.0f;

    public float eventDelayTime = 10.0f;
    public float valDuration = 5.0f;

    private float counter = 0;
    private float timeSinceLastRandomEvent = 0.0f;

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

    public float GetSacrificeValue()
    {
        return sMeter.GetCurrentValue();
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

    void Update()
    {
        if ((Time.time - timeSinceLastRandomEvent) > eventDelayTime || Mathf.Abs(timeSinceLastRandomEvent) < 0.0001f)
        {
            float meterVal = sMeter.GetCurrentValue();
            if (meterVal < belowStartVal)
            {
                //SetColor(Color.red);

                counter += Time.deltaTime;
                if (counter >= valDuration)
                {
                    Messenger.Invoke(punishmentEvent);
                    timeSinceLastRandomEvent = Time.time;
                }
            }
            else if(meterVal > aboveStartVal)
            {
                //SetColor(Color.green);

                counter += Time.deltaTime;
                if (counter >= valDuration)
                {
                    Messenger.Invoke(bonusEvent);
                    timeSinceLastRandomEvent = Time.time;
                }
            }
            else
            {
                counter = 0;
            }
        }
    }

    void SetColor(Color c)
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.color = c;
    }
}
