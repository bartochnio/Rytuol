using UnityEngine;
using System.Collections;

public class PeonsArea : MonoBehaviour, IPeonsArea {
	// IPeonsArea
	public Vector3 AnyLocation {
		get {
			var renderer = GetComponentInChildren<BoxCollider2D> ();
			var bounds = renderer.bounds;
			Vector3 minPt = bounds.min;
			Vector3 maxPt = bounds.max;
			return new Vector3( Random.Range(minPt.x, maxPt.x), Random.Range(minPt.y, maxPt.y) , 0.0f );
		}
	}
}
