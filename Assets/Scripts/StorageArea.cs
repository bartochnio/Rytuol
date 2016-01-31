using UnityEngine;
using System.Collections;

public class StorageArea : MonoBehaviour, IStorageArea {
	public VillageItemEnum containedItemType;


// IStorageArea
	public VillageItemEnum ContainedItemType {
		get { return containedItemType; }
	}

	public Vector3 AnyLocation {
		get {
			var renderer = GetComponentInChildren<BoxCollider2D> ();
			var bounds = renderer.bounds;
			Vector3 minPt = bounds.min;
			Vector3 maxPt = bounds.max;

            Vector2 size = renderer.size;
            Vector2 randomPoint = new Vector2(Random.Range(-size.x / 2.0f, size.x / 2.0f), Random.Range(-size.y / 2.0f, size.y / 2.0f));
            randomPoint = transform.TransformPoint(randomPoint);

            return randomPoint;
		}
	}
}
