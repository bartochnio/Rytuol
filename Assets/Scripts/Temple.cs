using UnityEngine;
using System.Collections;

public class Temple : MonoBehaviour
{
    public float reduceSacrificeValue = 0.5f;
    public float sacrificeFullValue = 0.3f;

    public float belowValDuration = 5.0f;
    public float eventStartVal = 0.0f;
    public float eventDelayTime = 10.0f;
    public string randomEvent;
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
        if (randomEvent.Length > 0 && (Time.time - timeSinceLastRandomEvent) > eventDelayTime)
        {
            if (sMeter.GetCurrentValue() < eventStartVal)
            {
                SetColor(Color.red);

                counter += Time.deltaTime;
                if (counter >= belowValDuration)
                {
                    Messenger.Invoke(randomEvent);
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
