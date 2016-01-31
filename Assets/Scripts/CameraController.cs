using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    Vector3 defaultPos = new Vector3(0.0f, 0.0f, -10.0f);

    void OnEnable()
    {
        Messenger.AddListener("thunder", CameraShake);
        Messenger.AddListener("speedBoost", CameraShake);
    }

    void OnDisable()
    {
        Messenger.RemoveListener("thunder", CameraShake);
        Messenger.RemoveListener("speedBoost", CameraShake);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = Vector3.Lerp(transform.position, defaultPos, Time.deltaTime * 20.0f);
	}

    IEnumerator ShakeEffect()
    {
        for (float i = 0; i < 0.5f; i+=Time.deltaTime)
        {
            
            transform.position += (Vector3)(Random.insideUnitCircle * 0.5f);
            yield return new WaitForEndOfFrame();
        }
    }

    void CameraShake()
    {
        StartCoroutine("ShakeEffect");
    }
}
