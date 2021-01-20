using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

/*  BeamPlacementM2.cs handles user input and the scene's state for Module 2
 *  This script handles placing the controller beam, opening/closing menus,
 *  and calling VectorMath and GiveInstructions for other functions
 */

public class BeamPlacementM3_Original : MonoBehaviour
{
    #region Public Panels
    // Menu Panel 
    public GameObject menuPanel;
    // Operations Panel
    public GameObject operationsPanel;
    //Keypad
    public GameObject keypad;
    //Calculations Panel
    public GameObject calcPanel;
    public Text inputText;
    #endregion


    #region Private member variables
    [HideInInspector] public Vector3 pocPos;
    // Content root
    private GameObject _root;
    // Origin of coordinate system
    private GameObject _origin;
    //Point of concurrency of system
    private GameObject _poc; 
    // Input controller
    private MLInput.Controller _controller = null;
    // LineRenderer from controller
    private LineRenderer _beamline = null;
    // VectorMath handles vector positioning
    private VectorMathM3_Original _vectorMath = null;
    // Position of end of the beam
    private Vector3 beamEnd;
    // Sphere object at end of beam
    private GameObject _beamSphere;
    // Rate of extending/retracting the controller beam
    private const float pushRate = 0.05f;
    // Depth of controller beam
    private float beamLength = 1f;
    // Latest touchpad Y value
    private float lastY = 0f;
    // Which vector is being placed
    private int vec;
    // True when vector head must follow the end of the beam.
    private bool placingHead;
    // True when the vector placed is valid
    private bool isValidVector;
    // display the instructions, stored in other script
    GiveInstructions _giveInstructions = null;
    // Is a menu being displayed?
    private bool aMenuIsActive;
    private int debugCount;


    private bool firstTimeInFSel;
    #endregion

    void Start()
    {
        GLOBALS.displayMode = DispMode.Vector;
        _root = GameObject.Find("Content Root");
        _origin = _root.transform.Find("Origin").gameObject;
        _poc = _origin.transform.Find("POC").gameObject;
        pocPos = _poc.transform.position;
        _controller = MLInput.GetController(MLInput.Hand.Left);
        _beamline = GetComponent<LineRenderer>();
        _beamSphere = GameObject.Find("BeamSphere");
        _vectorMath = GetComponent<VectorMathM3_Original>();
        _giveInstructions = GetComponent<GiveInstructions>();
        vec = 0;
        GLOBALS.stage = Stage.m3orig;
        if (!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnControllerButtonUp += OnButtonUp;
        MLInput.OnTriggerUp += OnTriggerUp;
        _beamline.startWidth = 0.007f;
        _beamline.endWidth = 0.01f;
        _origin.SetActive(false);

        menuPanel.SetActive(false);
        keypad.SetActive(false);
        operationsPanel.SetActive(false);
        calcPanel.SetActive(false);
        placingHead = false;
        _giveInstructions.DisplayText();
        debugCount = 0;
        firstTimeInFSel = true;
    }

    void Update()
    {
       // Debug.Log("STAGE: " + GLOBALS.stage);
        aMenuIsActive = (operationsPanel.activeSelf || menuPanel.activeSelf);
        if (GLOBALS.gridOn)
            SnapToGrid();
        HandleBeamPlacement();
        HandleTouchpadInput();

        pocPos = _poc.transform.position;
       // Debug.Log("POC POS: " + pocPos);
        // if placingHead, have vector head follow beam
        if (placingHead && !aMenuIsActive)
        {
            _vectorMath.PlaceVector3(vec, true, beamEnd);
        }
        
        // Debug.Log("Vector is valid: " + GLOBALS.isCorrectVectorPlacement);
    }

    public void IncrementStage()
    {
        // increment our stage variable
        GLOBALS.stage++;
        // update instructions text
        _giveInstructions.DisplayText();
        // play voiceover
        if (GLOBALS.soundOn)
            _giveInstructions.PlayAudio();
    }

    public void DecrementStage()
    {
            GLOBALS.stage--;

    }


    private void HandleBeamPlacement()
    {
        // handle the beam out of the controller
        if (_beamline.enabled)
        {
            beamEnd = _controller.Position + (transform.forward * beamLength);
            _beamline.SetPosition(0, _controller.Position);
            _beamline.SetPosition(1, beamEnd);
            _beamSphere.transform.position = beamEnd;

            // the beam sphere is only active for some moments
            _beamSphere.SetActive(!aMenuIsActive && !placingHead);
        }
    }

    #region MLInputController listeners
    // Trigger click functionality will depend on the stage the app is in
    // Each time, IncrementStage() will handle updating the instructions and moving to the new stage
    // Note - the trigger click handles what happens upon EXITING the stage given in the switch statement.
    private void OnTriggerUp(byte controllerId, float pressure)
    {
        if (!aMenuIsActive)
        {
           
            switch (GLOBALS.stage)
            {
                case Stage.m3orig:
                    _origin.SetActive(true);
                    _origin.transform.position = beamEnd;
                    _beamline.enabled = false;
                    IncrementStage();
                    break;
                case Stage.m3rotate:
                    _beamline.enabled = true;
                    IncrementStage();
                    break;
                case Stage.m3poc:
                    _poc.SetActive(true);
                    _poc.transform.position = beamEnd;
                    GLOBALS.pocPos = _poc.transform.position; //save to global for use in calc canv
                    IncrementStage();
                    placingHead = true;
                    break;
                case Stage.m3v1p1:
                    GLOBALS.displayMode = DispMode.Vector;
                    _vectorMath.PlaceVector3(vec, false, beamEnd);
                    GLOBALS.tailPos = beamEnd;
                    placingHead = true;
                    IncrementStage();
                    break;
                case Stage.m3v1p2:
                    placingHead = false;
                    vec++;
                    IncrementStage();
                    break;
                case Stage.m3v2p1:
                    _vectorMath.PlaceVector3(vec, false, beamEnd);
                    GLOBALS.tailPos = beamEnd;
                    placingHead = true;
                    IncrementStage();
                    break;
                case Stage.m3v2p2:
                    placingHead = false;
                    vec++;
                    IncrementStage();
                    break;
                case Stage.m3v3p1:
                    _vectorMath.PlaceVector3(vec, false, beamEnd);
                    GLOBALS.tailPos = beamEnd;
                    placingHead = true;
                    IncrementStage();
                    break;
                case Stage.m3v3p2:
                    placingHead = false;
                    vec++;
                    IncrementStage();
                    break;
                case Stage.m3v4p1:
                    _vectorMath.PlaceVector3(vec, false, beamEnd);
                    GLOBALS.tailPos = beamEnd;
                    placingHead = true;
                    IncrementStage();
                    break;
                case Stage.m3v4p2:
                    placingHead = false;
                    vec++;
                    IncrementStage();
                    break;
                case Stage.m3forcesel:
                   keypad.SetActive(true);

                    break;
                case Stage.m3keypad:
                    break;
                case Stage.m3view:
                    //  Debug.Log("trigg in m3view");
                    //GLOBALS.firstVec = false;
                    calcPanel.SetActive(true);
                    calcPanel.GetComponent<CalculationsPanel>().StartCalculationsSequence();
                    //GLOBALS.stage++;
                    //Summary: Checks how many vectors have been given forces. If there is one unknown force left
                    //increment stage, otherwise repeat force selection by decrementing stage
                    int temp = 0; //Checks how many vectors have been given forces
                    for (int i = 0; i < GetComponent<VectorMathM3_Original>().vectors.Count; i++)
                        if (GetComponent<VectorMathM3_Original>().vectors[i].GetComponent<VectorProperties>().isForceKnown)
                            temp++;

                    Debug.Log("in m3view- our given force vector is: " + GLOBALS.GivenForceVec.gameObject.name);
                    if (temp < 4)
                    {
                        DecrementStage();
                        DecrementStage();
                    }
                    else
                        GLOBALS.stage++;
                    break;
                case Stage.m3highlight:
                  //  Debug.Log("in m3highlight");
                    if (!GetComponent<VectorMathM3_Original>().vectors[vec].GetComponent<VectorProperties>().isForceKnown)
                            GetComponent<VectorMathM3_Original>().vectors[vec].GetComponent<VectorControlM3_Original>().vecColor = Color.white;
                        else
                            vec++;
                    break;
                /*case Stage.m3forcesel2:
                    Debug.Log("Current vector: " + vec);
                    if (!GetComponent<VectorMathM3_Original>().vectors[vec].GetComponent<VectorProperties>().isForceKnown)
                    {
                        keypad.SetActive(true);
                        
                    }
                    break;*/
                default:
                    return;
            }
        }

        Debug.Log("Current stage: " + GLOBALS.stage);
    }



    // Home or Bumper clicks handled here
    private void OnButtonUp(byte controllerId, MLInput.Controller.Button button)
    {
        if (button == MLInput.Controller.Button.HomeTap)
        {
            // if opening up the menu, make sure there is a beam and no instructions
            if (!menuPanel.activeSelf)
                _beamline.enabled = true;
            // close the ops menu if needed
            if (operationsPanel.activeSelf)
                operationsPanel.SetActive(false);
            // open the main menu
            menuPanel.SetActive(!menuPanel.activeSelf);
            // display instructions only when no menu
            _giveInstructions.EnableText(!menuPanel.activeSelf);
        }
        else if (button == MLInput.Controller.Button.Bumper)
        {
            // If we're viewing the completed operation, bumper will toggle the labels we are viewing
            if (GLOBALS.stage == Stage.m3view)
            {
                switch (GLOBALS.displayMode)
                {
                    case DispMode.Vector:
                        GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().SetUnitVec(false);
                        GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().SetEnabledLabels(false, false, true, true);
                        Debug.Log("in comp mode for vector " + GLOBALS.SelectedVec.name + " bumper press rec");
                        GLOBALS.displayMode = DispMode.Components;
                        break;
                    case DispMode.Components:
                        GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().SetUnitVec(true);
                        GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().SetEnabledLabels(false, false, true, true);
                        Debug.Log("in unit mode for vector " + GLOBALS.SelectedVec.name + " bumper press rec");
                        GLOBALS.displayMode = DispMode.Units;
                        break;
                    case DispMode.Units:
                        GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().SetUnitVec(false);
                        GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().SetEnabledLabels(false, false, false, false);
                        Debug.Log("in unit mode for vector " + GLOBALS.SelectedVec.name + " bumper press rec");
                        GLOBALS.displayMode = DispMode.Vector;
                        break;
                }
            }
        }
    }

    // The touch pad handles either origin rotation or changing the beam length
    private void HandleTouchpadInput()
    {
        if (_controller.Touch1Active)
        {
            // Touchpad handles rotating the origin
            if (GLOBALS.stage == Stage.m3rotate)
            {
                switch (_controller.CurrentTouchpadGesture.Direction)
                {
                    case MLInput.Controller.TouchpadGesture.GestureDirection.Clockwise:
                        _root.transform.RotateAround(_origin.transform.position, Vector3.up, 80f * Time.deltaTime);
                        break;
                    case MLInput.Controller.TouchpadGesture.GestureDirection.CounterClockwise:
                        _root.transform.RotateAround(_origin.transform.position, Vector3.up, -80f * Time.deltaTime);
                        break;
                }
            }
            else
            {
                //swipe up or down to adjust beam length
                if (_controller.Touch1PosAndForce.y - lastY < -0.001)
                    beamLength -= pushRate;
                else if (_controller.Touch1PosAndForce.y - lastY > 0.001)
                    beamLength += pushRate;
                lastY = _controller.Touch1PosAndForce.y;
                if (beamLength < 0.01f)
                    beamLength = 0.01f;
            }
        }
    }
    #endregion


    #region Unused Methods
    // prototype for grid snapped functionality - currently unused
    // This version is not very user-friendly!
    private void SnapToGrid()
    {
        beamEnd /= GLOBALS.gridSize;
        beamEnd = new Vector3(Mathf.Round(beamEnd.x), Mathf.Round(beamEnd.y), Mathf.Round(beamEnd.z));
        beamEnd *= GLOBALS.gridSize;
    }
    #endregion

    private void OnDisable()
    {
        GLOBALS.stage = Stage.m3orig;
        GLOBALS.opSelected = VecOp.none;
        GLOBALS.didCross = false;
        vec = 0;
        MLInput.OnTriggerUp -= OnTriggerUp;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }
}