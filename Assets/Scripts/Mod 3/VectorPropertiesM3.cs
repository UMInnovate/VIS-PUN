using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class 
    VectorPropertiesM3 : MonoBehaviour
{
    [HideInInspector]
    public bool isValidPlacement; //is the head or tail component colliding?

    [HideInInspector]
    public Vector3 forceVec;

    [HideInInspector]
    public float correctForceValue;

    [HideInInspector]
    public Vector3 uVec;

    [HideInInspector]
    public bool isForceKnown; //has the user selected/inputted a constant force value

    [HideInInspector]
    public bool isGivenForceValue; //is this the known force value


    [HideInInspector] public int forceValue; //user-inputted force value

    private bool nameLabelHovered;
    [SerializeField] private MLInput.Controller inputController;
    public GameObject keypad;

    [SerializeField] public BeamPlacementM3 beamPlacement;

    #region Public Methods
    public void SetNameLabelHoverState(bool isHovered)
    { nameLabelHovered = isHovered; }

    //set force value of vector from keypad panel
    public void SetForceVal(int fval)
    {
        forceValue = fval;

        //REGEX \b([A]|[B]|[C]|[D])
        //SpaceVector A
        string subA = gameObject.name.Substring(12);


        // Debug.Log("subA = " + subA);
        if (GLOBALS.inFeet) gameObject.GetComponent<VectorControlM3>().SetName(subA + " = " + fval.ToString() + " lbs");
        else gameObject.GetComponent<VectorControlM3>().SetName(subA + " = " + fval.ToString() + " N");
    }

    public void ViewMode(DispMode disp)
    {
        if (disp == DispMode.Components)
        {
            gameObject.GetComponent<VectorControlM3>().GetVectorComponents();
        }
    }
    #endregion

    void Start()
    {
        keypad.SetActive(false);
        inputController = MLInput.GetController(MLInput.Hand.Right);
        if (!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnTriggerDown += OnTriggerDown;
    }

    public void BuildForceVector()
    {
        Vector3 relVec = GetComponent<VectorControlM3>()._head.position - GetComponent<VectorControlM3>()._tail.position;
        Debug.Log(relVec.ToString(GLOBALS.format));
        float floatrelMag = relVec.magnitude;
        uVec = new Vector3(relVec.x / floatrelMag, relVec.y / floatrelMag, relVec.z / floatrelMag);
        if (isGivenForceValue)
        {
            forceVec = forceValue * uVec;
            GLOBALS.forceVector = forceVec;
            Debug.Log("our given force vec: " + forceVec.ToString(GLOBALS.format));
        }
        else
        {
            GLOBALS.unknownUVecs.Add(uVec); //unknownUVecs holds a list of unit vectors that dont have given force vals
            GLOBALS.unknownVecs.Add(gameObject);
        }
    }

    private void OnTriggerDown(byte controllerId, float pressure)
    {
        if (nameLabelHovered)
        {
            isForceKnown = true;
            //TRIGGER NEW STATE (ENTRY KEYPAD)
            //origin, content root, content
            GLOBALS.SelectedVec = gameObject;
            if (GLOBALS.stage == Stage.m3forcesel)
            {  //placed vectors, going into force keypad
                //Debug.Log("hover detected");
                //Debug.Log("trigger press dec vec prop on vector " + gameObject.name);
                keypad.SetActive(true);
                keypad.GetComponent<KeypadPanel>().ReceiveVector(gameObject);
                if (GLOBALS.count == 0)
                {
                    isGivenForceValue = true;
                    GLOBALS.GivenForceVec = gameObject;
                }
                else
                    isGivenForceValue = false;
                GLOBALS.count++;
                GLOBALS.stage++; //now in keypad
            }

        }
    }

}
