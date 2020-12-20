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
    public int forceValue; //user-inputted force value

    private bool nameLabelHovered;
    [SerializeField] private MLInputController inputController;

    #region Public Methods
    public void SetNameLabelHoverState(bool isHovered)
    { nameLabelHovered = isHovered; }


    #endregion
    
     void Start()
    {
        inputController = MLInput.GetController(MLInput.Hand.Right);
        if (!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnTriggerUp += OnTriggerUp;
    }

    private void OnTriggerUp(byte controllerId, float pressure)
    {
        if (nameLabelHovered)
        {
            isForceKnown = true;
            //TRIGGER NEW STATE (ENTRY KEYPAD)
            //origin, content root, content
            switch (GLOBALS.stage)
            {
                case (Stage.m3v4p2): //placed vectors, going into force keypad
                    GLOBALS.stage++;
                    return;
               // case (Stage.m): //selecting a vector for components
               //     return;
                    //selecting a vector to give force val
            }
        }
    }

    }
