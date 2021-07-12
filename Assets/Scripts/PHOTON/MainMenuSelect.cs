using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSelect : MonoBehaviour
{
    // main menu panel and credits panel to be toggled off and on for credits viewing
    public GameObject menuPanel;
    public GameObject creditPanel;


    // module 1
    public void Module1Selec()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    // module 2
    public void Module2Selec()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }

    //module 3
    public void Module3Selec()
    {
        SceneManager.LoadScene(13, LoadSceneMode.Single);
    }

    // quit app
    public void QuitApp()
    {
        Application.Quit();
    }

    // credits
    public void EnterCredits()
    {
        menuPanel.SetActive(false);
        creditPanel.SetActive(true);
    }
    public void ExitCredits()
    {
        menuPanel.SetActive(true);
        creditPanel.SetActive(false);
    } 
}
