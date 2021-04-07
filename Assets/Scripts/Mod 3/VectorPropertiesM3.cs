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
    public float correctForceValue; // DEPRECATED

    [HideInInspector]
    public Vector3 uVec;
    public float magnitude; 

    [HideInInspector]
    public bool isForceKnown; //has the user selected/inputted a constant force value

    [HideInInspector]
    public bool isGivenForceValue = false; //is this the known force value


    [HideInInspector] public int forceValue; //user-inputted force value

    [HideInInspector] public bool isHeadCollidingWithPOC = true;
    [HideInInspector] public Vector3 relativeVec; 

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

    public void BuildForceVector()
    {
        Console.WriteLine("entering build force vector with vector " + gameObject.name);
        float floatrelMag = relativeVec.magnitude;
        magnitude = floatrelMag;
        uVec = new Vector3(relativeVec.x / floatrelMag, relativeVec.y / floatrelMag, -1*relativeVec.z / floatrelMag);

        if (isGivenForceValue)
        {
            forceVec = forceValue * uVec;
            GLOBALS.forceVector = forceVec;
            Debug.Log("our given force vec: " + forceVec.ToString(GLOBALS.format));
        }
        else
        {
            //GLOBALS.unknownUVecs.RemoveAt(GLOBALS.unknownUVecs.Count);
            GLOBALS.unknownUVecs.Add(uVec); //unknownUVecs holds a list of unit vectors that dont have given force vals
            Console.WriteLine("vector " + gameObject.name + " added its uvec " + uVec.ToString("F2") + " to the unknownVecs list, list size is now: " + GLOBALS.unknownVecs.Count);
           // GLOBALS.unknownVecs.RemoveAt(GLOBALS.unknownVecs.Count);
            GLOBALS.unknownVecs.Add(gameObject);
            Console.WriteLine("vector " + gameObject.name + " added its gameobject to the unknown vecs list, size is now " + GLOBALS.unknownVecs.Count);

            Console.WriteLine("uvecs at v is " + GLOBALS.unknownUVecs[GLOBALS.unknownVecs.Count].ToString(GLOBALS.format) + " relativeVec: " + GLOBALS.unknownVecs[GLOBALS.unknownVecs.Count].GetComponent<VectorPropertiesM3>().relativeVec.ToString(GLOBALS.format));
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
