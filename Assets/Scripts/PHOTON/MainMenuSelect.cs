using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSelect : MonoBehaviour
{
    // singleplayer
    public void SinglePlayerModSelec()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }

    // multiplayer
    public void MultiPlayerModSelec()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Single);
    }

    // quit app
    public void QuitApp()
    {
        Application.Quit();
    }
}
