using UnityEngine;
using System.Collections;

public class Payload : MonoBehaviour {
    public GameObject savage;
    public GameObject animal;
    public GameObject fruit;


    public void ShowPayload(VillageItemEnum payloadType)
    {
        switch (payloadType)
        {
            case VillageItemEnum.eSavage:
                savage.SetActive(true);
                break;

            case VillageItemEnum.eAnimal:
                animal.SetActive(true);
                break;

            case VillageItemEnum.eFruit:
                fruit.SetActive(true);
                break;
        };

        gameObject.SetActive(true);
    }

    public void HidePayload()
    {
        savage.SetActive(false);
        animal.SetActive(false);
        fruit.SetActive(false);
    }
}
