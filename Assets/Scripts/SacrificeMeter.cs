using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SacrificeMeter : MonoBehaviour 
{
    public Image fillerImage;
    public float currentValue = 0;

    public float startValue = 0.75f;

    public float decreaseRate = 0.1f;
    public float increaseRate = 0.3f;

    public float desiredValue;
    bool warning = false; 
    bool boost = false;
    public Color bloodColor;
    void Start ()
    {
        currentValue = startValue;
        fillerImage.fillAmount = startValue;
        fillerImage.color = Color.grey;
    }
    // Uses Percents
    public void BoostByValue(float value)
    {
        value = Mathf.Clamp(value, 0, 1);
        if ( boost )
        {
            desiredValue += value;
        }
        else
        {
            desiredValue = currentValue + value;
        }
        desiredValue = Mathf.Clamp(desiredValue, 0f, 1f);
        boost = true;
    }

    void SetSprite(float value)
    {
        fillerImage.fillAmount = value;
    }

    /*
    public void OnPointerDown (PointerEventData data)
    {
        Debug.Log("Cliked");
        BoostByValue(0.3f);
    }
    */

    public IEnumerator Warning()
    {
        while (warning) {
            fillerImage.color = bloodColor;
            yield return new WaitForSeconds(0.20f);
            fillerImage.color = Color.white;
            yield return new WaitForSeconds(0.20f);
        }
    }

	// Update is called once per frame
	void Update () 
    {
        
        if ( boost )
        {
            if (!Mathf.Approximately(desiredValue, currentValue))
            {
                currentValue = Mathf.MoveTowards(currentValue, desiredValue, increaseRate * Time.deltaTime);
            }
            else
            {
                currentValue = desiredValue;
                boost = false;
            }
        }
        else
        {
            currentValue = Mathf.MoveTowards(currentValue, 0, decreaseRate * Time.deltaTime); 
        }
        if (currentValue < 0.3f && !warning)
        {
            warning = true;
            StartCoroutine("Warning");
            

        }

        if (currentValue > 0.3f)
        {
            warning = false;
            StopCoroutine("Warning");
            fillerImage.color = bloodColor;
        }

        SetSprite(currentValue);
	}
}
