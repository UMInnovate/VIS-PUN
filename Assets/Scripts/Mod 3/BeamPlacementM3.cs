using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.EventSystems;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using System;



public class BeamPlacementM3 : MonoBehaviour
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
    [HideInInspector] public GameObject _origin;
    //Point of concurrency of system
    private GameObject _poc;
    // Input controller
    private MLInput.Controller _controller = null;
    // LineRenderer from controller
    private PhotonLineRenderer _beamline = null; //***PUN
    // VectorMath handles vector positioning
    private VectorMathM3 _vectorMath = null;
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
    [HideInInspector] public Vector3 storedBeamEnd; //* PUN STORED POS OF BEAMEND
    public bool bOriginPlaced, bIsViewer; ///* Origin Placed by someone

    [HideInInspector]
    public bool bCanPlaceVec1Labels, bCanPlaceVec2Labels, bCanPlaceVec3Labels, bCanPlaceVec4Labels, bCanPlacePOC, bCalcPanel, bCalc1, bCalcSoE, bLinearSys, bValidate = false;

    public bool forceSystemText = false;

    [HideInInspector] public Vector3 adjPOCPos; //*PUN POC
    #endregion

    void Start()
    {
        bIsViewer = false; 
        GLOBALS.displayMode = DispMode.Vector;
        _root = GameObject.Find("Content Root");
        _origin = _root.transform.Find("Origin").gameObject;
        _poc = _origin.transform.Find("POC").gameObject;
        pocPos = _poc.transform.position;
        _controller = MLInput.GetController(MLInput.Hand.Left);
        _beamline = GetComponent<PhotonLineRenderer>();
        _beamSphere = GameObject.Find("BeamSphere");
        _vectorMath = GetComponent<VectorMathM3>();
        _giveInstructions = GetComponent<GiveInstructions>();
        vec = 0;
        GLOBALS.stage = Stage.m3orig;
        if (!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnControllerButtonUp += OnButtonUp;
        MLInput.OnTriggerUp += OnTriggerUp;
        _beamline.SetPosition(0, _controller.Position); //***PUN
        _beamline.SetPosition(1, beamEnd); //***PUN


        _origin.SetActive(false);

        menuPanel.SetActive(false);
        keypad.SetActive(false);
        operationsPanel.SetActive(false);
        calcPanel.SetActive(false);
        placingHead = false;
        _giveInstructions.DisplayText();
        debugCount = 0;
        GLOBALS.count = 0;
        bOriginPlaced = false;
    }

    void Update()
    {
        if (!bIsViewer)
        {
            _giveInstructions.DisplayText();
        }
        else
        {
            _giveInstructions.text.text = "You are a viewer. Your controller is disabled.";
        }

        aMenuIsActive = (operationsPanel.activeSelf || menuPanel.activeSelf);
        if (GLOBALS.gridOn)
            SnapToGrid();
        HandleBeamPlacement();
        HandleTouchpadInput();

        pocPos = _poc.transform.position;
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
            _beamSphere.SetActive(!aMenuIsActive);
        }
    }

    #region MLInputController listeners
    // Trigger click functionality will depend on the stage the app is in
    // Each time, IncrementStage() will handle updating the instructions and moving to the new stage
    // Note - the trigger click handles what happens upon EXITING the stage given in the switch statement.
    private void OnTriggerUp(byte controllerId, float pressure)
    {
        if (!aMenuIsActive && !bIsViewer)
        {

            switch (GLOBALS.stage)
            {
                case Stage.m3orig:
                    _origin.SetActive(true);
                    _origin.transform.position = beamEnd;
                    _beamline.enabled = false;
                    bOriginPlaced = true;
                    GLOBALS.isHost = true; 
                    IncrementStage();
                    break;
                case Stage.m3rotate:
                    _beamline.enabled = true;
                    IncrementStage();
                    break;
                case Stage.m3poc:
                    _poc.SetActive(true);
                    PhotonNetwork.Instantiate("POC", beamEnd, Quaternion.identity);
                    _poc.transform.position = beamEnd;
                    GLOBALS.pocPos = _poc.transform.position; //save to global for use in calc canv
                    bCanPlacePOC = true; 
                   // Console.WriteLine("\n GLOBAL POC \n" + _poc.transform.position.ToString());
                    IncrementStage();
                    placingHead = true;
                    break;
                case Stage.m3v1p1:
                    storedBeamEnd = beamEnd;
                    _vectorMath.PlaceVector3Point(vec, storedBeamEnd);
                    IncrementStage();
                    break;
                case Stage.m3v1p2:
                    bCanPlaceVec1Labels = true; //***PUN
                    vec++;
                    IncrementStage();
                    break;
                case Stage.m3v2p1:
                    storedBeamEnd = beamEnd;
                    _vectorMath.PlaceVector3Point(vec, storedBeamEnd);
                    IncrementStage();
                    break;
                case Stage.m3v2p2:
                    bCanPlaceVec2Labels = true; //***PUN
                    vec++;
                    IncrementStage();
                    break;
                case Stage.m3v3p1:
                    storedBeamEnd = beamEnd;
                    _vectorMath.PlaceVector3Point(vec, storedBeamEnd);
                    IncrementStage();
                    break;
                case Stage.m3v3p2:
                    bCanPlaceVec3Labels = true; //***PUN
                    vec++;
                    IncrementStage();
                    break;
                case Stage.m3v4p1:
                    storedBeamEnd = beamEnd;
                    _vectorMath.PlaceVector3Point(vec, storedBeamEnd);
                    IncrementStage();
                    break;
                case Stage.m3v4p2:
                    bCanPlaceVec4Labels = true; //***PUN
                   // storedBeamEnd = beamEnd;
                    vec++;
                    bCalcPanel = true;
                    IncrementStage();
                    break;
                case Stage.m3forcesel:
                    calcPanel.GetComponent<CalculationsPanelM3>().StartCalculationsSequence();
             //       calcPanel.SetActive(true);
                    break;
                case Stage.m3keypad:
                    calcPanel.SetActive(true);
                    bCalc1 = true;
                    calcPanel.GetComponent<CalculationsPanelM3>().ComponentCalcs();
                    calcPanel.GetComponent<CalculationsPanelM3>().MagCalcs();
                    break;
                case Stage.m3view:
                    Console.WriteLine("going into setbuild stuff - our chosen int is " + GLOBALS.chosenVecInt);
                    switch (GLOBALS.chosenVecInt)
                    {
                        case 0: //a is chosen
                          //  SetBuild(1); SetBuild(2); SetBuild(3);
                            _vectorMath.vectors[1].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[2].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[3].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            break;
                        case 1:
                            // SetBuild(0); SetBuild(2); SetBuild(3);
                            _vectorMath.vectors[0].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[2].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[3].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            break;
                        case 2:
                         //   SetBuild(0); SetBuild(1); SetBuild(3);
                            _vectorMath.vectors[0].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[1].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[3].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            break;
                        case 3:
                          //  SetBuild(0); SetBuild(1); SetBuild(2);
                            _vectorMath.vectors[0].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[1].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            _vectorMath.vectors[2].GetComponent<VectorPropertiesM3>().BuildForceVector();
                            break;
                        default:
                           
                            Console.WriteLine("default case in m3view, setbuild");
                            break;
                    }
                    GetComponent<VectorMathM3>().SolveSystemOfEquations();
                    GLOBALS.stage++;
                    break;
                case Stage.m3forceview:
                    //calcPanel.GetComponent<CalculationsPanel>().SystemOfEqs();
                    bCalcSoE = true;
                    //   GetComponent<VectorMathM3>().ValidateForceSystem();
                    Console.WriteLine("ENTERING STAGE M3FORCEVIEW");
                    calcPanel.GetComponent<CalculationsPanelM3>().SystemOfEqs();
                    //  calcPanel.GetComponent<CalculationsPanel>().SystemOfEqs();
                    //calcPanel.GetComponent<CalculationsPanel>().ShowCorrectFVecs();
                    GLOBALS.stage++;
                    break;
                case Stage.m3forcesys:
                    bLinearSys = true; 
                    calcPanel.GetComponent<CalculationsPanelM3>().LinearCalc();
                    GLOBALS.stage++;
                    break;
                case Stage.m3validateview:
                    bValidate = true; 
                    calcPanel.GetComponent<CalculationsPanelM3>().isValid();
                    break;
                default:
                    return;
            }
        }

      //  Debug.Log("Current stage: " + GLOBALS.stage);
    }

    public void SetBuild(int v)
    { 
        Console.WriteLine("in set build for " + v);
        _vectorMath.vectors[v].GetComponent<VectorPropertiesM3>().isGivenForceValue = false;
        _vectorMath.vectors[v].GetComponent<VectorPropertiesM3>().BuildForceVector();
        Console.WriteLine("for v = " + v + " uvec is " + _vectorMath.vectors[v].GetComponent<VectorPropertiesM3>().uVec.ToString(GLOBALS.format));
        Console.WriteLine("uvecs at v is " + GLOBALS.unknownUVecs[v].ToString(GLOBALS.format) + " relativeVec: " + GLOBALS.unknownVecs[v].GetComponent<VectorPropertiesM3>().relativeVec.ToString(GLOBALS.format));
    }

    // Home or Bumper click handled here
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
            if(GLOBALS.stage == Stage.m3v1p2 || GLOBALS.stage == Stage.m3v2p2 || GLOBALS.stage == Stage.m3v3p2 || GLOBALS.stage == Stage.m3v4p2)
            {
              //  Debug.Log("bumper, can place head = " + _vectorMath.vectors[vec].canPlaceHead.ToString());
                _vectorMath.vectors[vec].canPlaceHead = !_vectorMath.vectors[vec].canPlaceHead;

                Console.WriteLine("isheadcol was " + _vectorMath.vectors[vec].GetComponent<VectorPropertiesM3>().isHeadCollidingWithPOC + " is now " + !_vectorMath.vectors[vec].GetComponent<VectorPropertiesM3>().isHeadCollidingWithPOC);
                _vectorMath.vectors[vec].GetComponent<VectorPropertiesM3>().isHeadCollidingWithPOC = !_vectorMath.vectors[vec].GetComponent<VectorPropertiesM3>().isHeadCollidingWithPOC;
                _vectorMath.PlaceVector3Point(vec, storedBeamEnd);
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