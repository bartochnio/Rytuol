using UnityEngine;
using System.Collections;

public class Chapel : MonoBehaviour
{
    public StorageArea savageArea;
    public float indoctrinationTime = 1.0f;

    private bool mIsIndoctrinating = false;
    private float counter = 0.0f;

    private static Chapel instance = null;
    public static Chapel GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        Chapel.instance = this;
    }

    void OnMouseDown()
    {
        Debug.Log("SAVAGE");
        VillageItem savage = savageArea.PopItemFromStorage();

        if (savage != null)
            Village.GetGlobalInstance().OrderConvertSavage(savage);
    }

    public void Indoctrinate()
    {
        Debug.Log("UOLOLOLOLO");
        mIsIndoctrinating = true;
    }

    public Vector2 GetEntrace()
    {
        Vector2 pos = transform.GetChild(0).position;
        return pos;
    }

    void Update()
    {
        if (mIsIndoctrinating)
        {
            counter += Time.deltaTime;
            if (counter >= indoctrinationTime)
            {
                counter = 0.0f;
                mIsIndoctrinating = false;
                Village.GetGlobalInstance().SpawnPeon(GetEntrace());
            }
        }
    }
}
