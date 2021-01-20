using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using System;

public class KeypadPanel : MonoBehaviour
{
    [SerializeField] List<Button> ValueButtons;
    [SerializeField] Button CheckButton;
    //[SerializeField] Button ConfirmButton;
    public Text IFText;

    private GameObject panel;

    private VectorProperties vp;
    bool check;
    //PhotonView PV;

   // private int count;
    // Start is called before the first frame update
    void Start()
    {
        
        check = false;
        if(!MLInput.IsStarted) MLInput.Start();
        MLInput.OnTriggerUp += OnTriggerUp;
        panel = GetComponent<GameObject>();
        IFText.text = "";
        Debug.Log("panel initialized");
    }

    public void ReceiveVector(GameObject v)
    {
        Debug.Log("before vp");

        vp = v.GetComponent<VectorProperties>();
        Debug.Log("the item is: " + vp.gameObject.name);
        Debug.Log("after vp");
    }
    public void CheckClicked()
    {
        string value = IFText.text;
        vp.forceValue = int.Parse(value); 
        Debug.Log("vp force val: " + vp.forceValue.ToString());

        gameObject.SetActive(false);
        vp.SetForceVal(int.Parse(value));
        if(GLOBALS.count == 0) vp.BuildForceVector();
        ACClicked();
        GLOBALS.stage++;
    }

    public void ACClicked()
    {
        IFText.text = "";
    }


    private void OnTriggerUp(byte controllerId, float pressure)
    {
        if(GLOBALS.stage == Stage.m3keypad)
           Debug.Log("in keypadpanel: triggerup");
    }

    public void NumberButtonClicked(int buttonValue)
    {
        IFText.text += buttonValue.ToString();
    }
}
