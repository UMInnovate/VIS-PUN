﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

/* MenuPanel.cs handles the main menu buttons for both module 1 and module 2
* Each button has a field for which function runs upon click - these are listed below
* Some buttons need to have their text update as well
*/

public class MenuPanel : MonoBehaviour
{
    // these text items live inside the Button of MainMenuPanel

    [SerializeField] private PhotonRoom room; 

    // module 2 only
    [SerializeField, Tooltip("Text field of button for Handedness selection")]
    Text handText = null;

    [SerializeField, Tooltip("Text field of button for Units selection")]
    Text unitsText = null;

    [SerializeField, Tooltip("Text field of button for Sound selection")]
    Text soundText = null;

    //Button:Hand.OnClick()
    public void ToggleHandedness()
    {
        GLOBALS.rightHanded = !GLOBALS.rightHanded;
        GLOBALS.flipZ = -GLOBALS.flipZ;
        GLOBALS.invertCross = true;
        if (GLOBALS.rightHanded)
            handText.text = "Handedness: Right";
        else
            handText.text = "Handedness: Left";
    }

    //Button:Units.OnClick()
    public void ToggleUnits()
    {
        GLOBALS.inFeet = !GLOBALS.inFeet;
        if (GLOBALS.inFeet)
            unitsText.text = "Units: Feet";
        else
            unitsText.text = "Units: Meters";
    }

    // Button:Sound.OnClick()
    public void ToggleSound()
    {
        GLOBALS.soundOn = !GLOBALS.soundOn;
        if (GLOBALS.soundOn)
            soundText.text = "Sound: On";
        else
            soundText.text = "Sound: Off";
    }



    //general scene reload
    public void ResetModClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    //Button:Reset.OnClick() in Module 1
    public void ResetMod1Clicked()
    {
        SceneManager.LoadScene(4, LoadSceneMode.Single);
    }

    //Button:Reset.OnClick() in Module 2
    public void ResetMod2Clicked()
    {
        SceneManager.LoadScene(6, LoadSceneMode.Single);
    }

    public void ResetMod3Clicked()
    {
        SceneManager.LoadScene(12, LoadSceneMode.Single);
    }


    //Button:BackToStart.OnClick()
    public void BackToStartClicked()
    {
        if (SceneManager.GetActiveScene().buildIndex == 9 || SceneManager.GetActiveScene().buildIndex == 9 || SceneManager.GetActiveScene().buildIndex == 14) //pun scenes are 9,11,14
            room.OnPlayerLeftRoom(PhotonNetwork.LocalPlayer); //clean up user's items
        else
            SceneManager.LoadScene(0, LoadSceneMode.Single);
     }

    //Button:Exit.OnClick()
    public void Exit()
    {
        Application.Quit();
    }

}
