using UnityEngine;
using System.Collections;

public class Temple : MonoBehaviour
{
	public enum ID {
		Left,
		Middle,
		Right
	}
	public ID templeId;
	
	
    void OnMouseDown()
    {
		SacrificeQueue.GetInstance ().DispatchToTheTemple (templeId, transform.position);
    }
}
