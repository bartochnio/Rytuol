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
    public Image[] alphas;

    void Start()
    {
        Camera c = GameObject.FindObjectOfType<Camera>();
        tutorial.enabled = false; 
        DontDestroyOnLoad(c);
        HideTutorials();
        ShowTutorial();
    }

    void ShowTutorial()
    {
        texts[currentTut].enabled = true;
        alphas[currentTut].enabled = true; 
    }

    void HideTutorials()
    {
        foreach (Image i in alphas)
            i.enabled = false;
        foreach (Text t in texts)
            t.enabled = false;
    }


    public void StartButton()
    {
        Application.LoadLevel("VilageProto_v2");
    }

    public void YesTut()
    {
        tutorial.enabled = true;
        main.enabled = false;
    }

    public void NoTut()
    {
        tutorialStartup.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void NextTutorial()
    {
        currentTut++;
        if (currentTut >= texts.Length )
            currentTut = 0;
        HideTutorials();
        ShowTutorial();
    }

    public void PreviousTutorial()
    {
        currentTut--;
        if (currentTut < 0)
            currentTut = texts.Length -1;
        HideTutorials();
        ShowTutorial();
    }

    public void QuitTutorial()
    {
        tutorialStartup.SetActive(false);
        tutorial.enabled = false;
        main.enabled = true;
    }

}
