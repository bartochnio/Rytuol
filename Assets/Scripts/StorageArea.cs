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
			var renderer = GetComponentInChildren<MeshRenderer> ();
			var bounds = renderer.bounds;
			Vector3 minPt = bounds.min;
			Vector3 maxPt = bounds.max;
			return new Vector3( Random.Range(minPt.x, maxPt.x), 0.0f, Random.Range(minPt.z, maxPt.z) );
		}
	}
}
