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

            Vector2 size = renderer.size;
            Vector2 randomPoint = new Vector2(Random.Range(-size.x / 2.0f, size.x / 2.0f), Random.Range(-size.y / 2.0f, size.y / 2.0f));
            randomPoint = transform.TransformPoint(randomPoint);

            return randomPoint;
		}
	}

    public void RequestSelfSacrifice()
    {
        int queueNumber = SacrificeQueue.GetInstance().ReserveSlot();
        Village.GetGlobalInstance().OrderSelfSacrifice(queueNumber);
    }

    void OnMouseDown ()
    {
        Debug.Log("Self Sacrifice!");
        RequestSelfSacrifice();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space ))
        {
            RequestSelfSacrifice();
        }
    }
}
