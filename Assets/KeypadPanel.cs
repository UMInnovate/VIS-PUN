using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using Photon;
using Photon.Realtime;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;

public class KeypadPanel : MonoBehaviour
{
    [SerializeField] List<Button> ValueButtons;
    [SerializeField] Button CheckButton;

    [HideInInspector] public bool given;
    //[SerializeField] Button ConfirmButton;
    public Text IFText;

    private GameObject panel;

    private VectorProperties vp;
    private VectorPropertiesM3 vp3;
    bool check;
    //PhotonView PV;

    // private int count;
    // Start is called before the first frame update
    void Start()
    {

        check = false;
        if (!MLInput.IsStarted) MLInput.Start();
        MLInput.OnTriggerUp += OnTriggerUp;
        panel = GetComponent<GameObject>();
        IFText.text = "";
        Debug.Log("panel initialized");
    }

    public void ReceiveVector(GameObject v)
    {
        Debug.Log("before vp");

        vp = v.GetComponent<VectorProperties>();
        vp3 = v.GetComponent<VectorPropertiesM3>();
        //    vp.isGivenForceValue = true;
        //  GLOBALS.GivenForceVec = vp.gameObject;
        //  Debug.Log("the given force vector is (in m3forcesel): " + vp.gameObject.name);

        // Debug.Log("this vector is not a give force vector (in m3forcesel): " + vp.gameObject.name);

        Debug.Log("after vp");
    }

<<<<<<< Updated upstream

=======
>>>>>>> Stashed changes
    public void CheckClicked()
    {
        if (SceneManager.GetActiveScene().buildIndex == 12)
        {
            
            string value = IFText.text;
            vp.forceValue = int.Parse(value);
            

            gameObject.SetActive(false);
            vp.SetForceVal(int.Parse(value));
            vp.BuildForceVector();
            ACClicked();
            Debug.Log("vp force val: " + vp.forceValue.ToString());
            GLOBALS.stage++;
        }
        else
        {
            Debug.Log("vp3 force val: " + vp3.forceValue.ToString());
            string value = IFText.text;
            vp3.forceValue = int.Parse(value);
            

            gameObject.SetActive(false);
            vp3.SetForceVal(int.Parse(value));
            vp3.BuildForceVector();
            ACClicked();
            GLOBALS.stage++;
        }
    }

    public void ACClicked()
    {
        IFText.text = "";
    }


    private void OnTriggerUp(byte controllerId, float pressure)
    {
        if (GLOBALS.stage == Stage.m3keypad)
            Debug.Log("in keypadpanel: triggerup");
    }

    public void NumberButtonClicked(int buttonValue)
    {
        IFText.text += buttonValue.ToString();
    }
}