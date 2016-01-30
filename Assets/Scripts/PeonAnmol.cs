using UnityEngine;
using System.Collections;

public class PeonAnmol : MonoBehaviour/*, IPeon*/ {

// private vars
	enum ActionState {
		eIdle,

		eMovingToPeonsArea,

		eCapturingSavage,
		eGatheringFruit,
		eTamingAnimal,

		eStoringSavage,
		eStoringFruit,
		eStoringAnimal,

		eRetrievingSavage,
		eRetrievingFruit,
		eRetrievingAnimal,

		eOfferingSavage,
		eOfferingFruit,
		eOfferingAnimal
	}
	ActionState actionState = ActionState.eIdle;


	bool bSelected = false;


	Vector3 startPt, endPt;
	float t;


// MonoBehaviour
//
	void Update () {
		if (actionState != ActionState.eIdle) {
			const float travelTime = 3.0f;

			t += Time.deltaTime;
			if (t >= travelTime) {
				t = travelTime;
				actionState = ActionState.eIdle;
			}
			transform.position = Vector3.Lerp (startPt, endPt, t / travelTime);
		}

		if (bSelected) {
			transform.Rotate (Vector3.up, 360.0f * Time.deltaTime, Space.World);
		}
	}

	void OnTriggerEnter(Collider other) {
	}


// private funcs
	void MoveTo(Vector3 point, ActionState newActionState) {
		startPt = transform.position;
		endPt = point;
		t = 0.0f;
		actionState = newActionState;
	}


// IPeon
//
	public void MoveToPeonsArea() {
		if (actionState != ActionState.eIdle)
			return;

		MoveTo (Village.GetGlobalInstance ().PeonsArea.AnyLocation, ActionState.eMovingToPeonsArea);
	}

	public void SeekVillageItem(IVillageItem item) {
		if (actionState != ActionState.eIdle)
			return;
	}

	public void SeekForestItem(IForestItem item) {
		if (actionState != ActionState.eIdle)
			return;
	}
}
