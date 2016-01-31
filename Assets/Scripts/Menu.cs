using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour {

    public GameObject tutorialStartup;
    public Canvas tutorial;
    public Canvas main;
    public  int currentTut;
    public Text[] texts;
    public AudioSource sfx;
    public AudioClip click;
    public Text tutcounter; 

    void Start()
    {
     //   Camera c = GameObject.FindObjectOfType<Camera>();
        tutorial.enabled = false; 
     //   DontDestroyOnLoad(c);
        HideTutorials();
        ShowTutorial();
    }

    void ShowTutorial()
    {
        texts[currentTut].enabled = true;
        tutcounter.text = "" + (currentTut+1).ToString() + "/4";
    }

    void HideTutorials()
    {
   
        foreach (Text t in texts)
            t.enabled = false;
    }


    public void StartButton()
    {
        Application.LoadLevel("VilageProto_v2");
        sfx.PlayOneShot(click);
    }

    public void YesTut()
    {
        tutorial.enabled = true;
        main.enabled = false;
        sfx.PlayOneShot(click);
    }

    public void NoTut()
    {
        tutorialStartup.SetActive(false);
        sfx.PlayOneShot(click);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextTutorial()
    {
        currentTut++;
        
        sfx.PlayOneShot(click);
        if (currentTut >= texts.Length )
            currentTut = 0;
        HideTutorials();
        ShowTutorial();
    }

    public void PreviousTutorial()
    {
        currentTut--;
       
        sfx.PlayOneShot(click);
        if (currentTut < 0)
            currentTut = texts.Length -1;
        HideTutorials();
        ShowTutorial();
    }

    public void QuitTutorial()
    {
        sfx.PlayOneShot(click);
        tutorialStartup.SetActive(false);
        tutorial.enabled = false;
        main.enabled = true;
    }

}
