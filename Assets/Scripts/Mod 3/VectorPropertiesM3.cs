using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class 
    VectorPropertiesM3 : MonoBehaviour
{
    public Vector3 relativeVec;
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

        switch(subA) {
            case "A":
                GLOBALS.chosenVecInt = 0;
                break;
            case "B":
              GLOBALS.chosenVecInt = 1;
              break;
            case "C":
                GLOBALS.chosenVecInt = 2;
                break;
            case "D":
                GLOBALS.chosenVecInt = 3;
                break;
            default:
                GLOBALS.chosenVecInt = -1;
                break; 
        }

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

    //relative vec is HEAD - TAIL

    public void BuildForceVector()
    {

        if (GetComponent<VectorControlM3>().canPlaceHead) //head was placed, tail is POC, so we do reltailpos - adjpocpos
        {
            if (beamPlacement.bIsViewer)
            {
                relativeVec = GetComponent<VectorControlM3>().photonPos - beamPlacement.adjPOCPos;
            }
            else
            {
                relativeVec = (GetComponent<VectorControlM3>()._head.position - GLOBALS.pocPos) - beamPlacement.adjPOCPos;
                Console.WriteLine("my relhead= " + (GetComponent<VectorControlM3>()._head.position - GLOBALS.pocPos).ToString(GLOBALS.format));
                Console.WriteLine("RELHEAD FROM VC  " + GetComponent<VectorControlM3>().relHeadPos.ToString("F2"));

                //Console.WriteLine("relative vec is " + GetComponent<VectorControlM3>().relHeadPos.ToString(GLOBALS.format) + " - " + beamPlacement.adjPOCPos.ToString("F2"));
            }
        }
        else
        {
            if (beamPlacement.bIsViewer)
            {
                relativeVec = beamPlacement.adjPOCPos - GetComponent<VectorControlM3>().photonPos;
            }
            else
            {
                relativeVec = beamPlacement.adjPOCPos - GetComponent<VectorControlM3>().relTailPos;
                Console.WriteLine("relative vec is " + beamPlacement.adjPOCPos.ToString(GLOBALS.format) + " - " + GetComponent<VectorControlM3>().relTailPos.ToString(GLOBALS.format));
            }
        }

        Console.WriteLine("relativevec: " + relativeVec.ToString(GLOBALS.format));
        float floatrelMag = relativeVec.magnitude;
        uVec = new Vector3(relativeVec.x / floatrelMag, relativeVec.y / floatrelMag, -1 * relativeVec.z / floatrelMag);

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
