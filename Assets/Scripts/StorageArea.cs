using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StorageArea : MonoBehaviour, IStorageArea {

    public VillageItemEnum containedItemType;
    private List<VillageItem> _retriveList = new List<VillageItem>();
    private List<VillageItem> _itemsList = new List<VillageItem>();



    // IStorageArea
    public VillageItemEnum ContainedItemType {
        get { return containedItemType; }
    }

    public List<VillageItem> retriveList { get { return _retriveList; } }
    public List<VillageItem> itemsList { get { return _itemsList; } }

    public Vector3 AnyLocation {
        get {
            var renderer = GetComponentInChildren<BoxCollider2D>();
            var bounds = renderer.bounds;
            Vector3 minPt = bounds.min;
            Vector3 maxPt = bounds.max;

            Vector2 size = renderer.size;
            Vector2 randomPoint = new Vector2(Random.Range(-size.x / 2.0f, size.x / 2.0f), Random.Range(-size.y / 2.0f, size.y / 2.0f));
            randomPoint = transform.TransformPoint(randomPoint);

            return randomPoint;
        }
    }

    bool hasItem
    {
        get { return itemsList.Count > 0; }
    }

    public void ReturnItemToTheList ( VillageItem item )
    {
        if (_retriveList.Contains(item))
        {
            itemsList.Add(item);
            retriveList.Remove(item);
        }
    }

    VillageItem PopItemFromStorage()
    {
        VillageItem item = null;
        if (itemsList.Count > 0)
        {
            item = itemsList[0];
            retriveList.Add(item);
            itemsList.Remove(item);
        }
        return item;
    }
    void Update ()
    {

        if ( Input.GetKeyDown ( KeyCode.Alpha1))
        {
            if (hasItem)
            {
                PopItemFromStorage().Select(this);
            Debug.Log("Items List: " + itemsList.Count + " \r\n Retived Items: " + retriveList.Count);
            }
        }
    }
    void OnButtonDown()
    {
        if (hasItem)
        {
            PopItemFromStorage().Select(this);
        }
        Debug.Log("Storage Cliced");
    }
}
