using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class VectorProperties : MonoBehaviour
{
    [HideInInspector]
    public bool isValidPlacement; //is the head or tail component colliding?

    [HideInInspector]
    public bool isForceKnown; //has the user selected/inputted a constant force value
    [HideInInspector] public int forceValue; //user-inputted force value

    private bool nameLabelHovered;
    [SerializeField] private MLInputController inputController;
    public GameObject keypad;

    #region Public Methods
    public void SetNameLabelHoverState(bool isHovered)
    { nameLabelHovered = isHovered; }

    [SerializeField] BeamPlacementM3_Original beamPlacement;

    #endregion
    
     void Start()
    {
        inputController = MLInput.GetController(MLInput.Hand.Right);
        if (!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnTriggerDown += OnTriggerDown;
    }

    private void OnTriggerDown(byte controllerId, float pressure)
    {
        if (nameLabelHovered)
        {
            isForceKnown = true;
            //TRIGGER NEW STATE (ENTRY KEYPAD)
            //origin, content root, content
            if (GLOBALS.stage == Stage.m3forcesel)
            {  //placed vectors, going into force keypad
                Debug.Log("hover detected");
                Debug.Log("trigger press dec vec prop on vector " + gameObject.name);
                keypad.GetComponentInParent<GameObject>().SetActive(true);

                keypad.GetComponent<KeypadPanel>().ReceiveVector(gameObject);
              //  string value = beamPlacement.keypad.GetComponent<KeypadPanel>().IFText.text;
                //float adjValue = float.Parse(value, System.Globalization.NumberStyles.Float);
                GLOBALS.stage++; //now in keypad
            }
               // case (Stage.m): //selecting a vector for components
               //     return;
                    //selecting a vector to give force val
            }
        }
    }

    
