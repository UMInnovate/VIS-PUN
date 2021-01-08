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

    public void ReceiveVector(VectorProperties v)
    {
        vp = v;
        Debug.Log("vp = " + v.name);
    }
    public void CheckClicked()
    {
        //    panel.SetActive(false);
        //   Debug.Log("Check clicked");
        //go back to v selection
        check = true;
      //  int de = 0;
        string value = IFText.text;
        vp.forceValue = int.Parse(value); 
        Debug.Log("vp force val: " + vp.forceValue.ToString());
        GLOBALS.stage++;
    }

    public void ACClicked()
    {
        IFText.text = "";
    }


    private void OnTriggerUp(byte controllerId, float pressure)
    {
        if (check)
        {
            Debug.Log("check click");
            check = false;
            CheckClicked();
        }
        Debug.Log("in keypadpanel: triggerup");
    }

    public void NumberButtonClicked(int buttonValue)
    {
        IFText.text += buttonValue.ToString();
    }
}
