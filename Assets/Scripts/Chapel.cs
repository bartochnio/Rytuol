using UnityEngine;
using System.Collections;

public class Chapel : MonoBehaviour
{
    public StorageArea savageArea;

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
}
