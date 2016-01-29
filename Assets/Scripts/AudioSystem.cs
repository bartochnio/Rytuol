using UnityEngine;
using System.Collections;

public class AudioSystem : MonoBehaviour {

    AudioSource audio;
    
    

	// Use this for initialization
	void Start () {
        foreach (AudioSource a in GameObject.FindObjectsOfType<AudioSource>())
            DontDestroyOnLoad(a);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
