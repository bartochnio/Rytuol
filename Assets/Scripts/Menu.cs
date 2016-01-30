using UnityEngine;
using System; 
using System.Collections;

public class Menu : MonoBehaviour {

    void Start()
    {
        Camera c = GameObject.FindObjectOfType<Camera>();
        DontDestroyOnLoad(c);
    }


    public void StartButton()
    {
        Application.LoadLevel("Main");
    }


    public void Quit()
    {
        Application.Quit();
    }

}
