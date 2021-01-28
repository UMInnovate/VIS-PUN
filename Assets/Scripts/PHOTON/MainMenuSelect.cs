using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSelect : MonoBehaviour
{
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
}
