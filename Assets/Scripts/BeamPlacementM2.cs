﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

/*  BeamPlacementM2.cs handles user input and the scene's state for Module 2
 *  This script handles placing the controller beam, opening/closing menus,
 *  and calling VectorMath and GiveInstructions for other functions
 */

public class BeamPlacementM2 : MonoBehaviour
{
    #region Public Panels
    // Menu Panel 
    public GameObject menuPanel;
    // Operations Panel
    public GameObject operationsPanel;
    #endregion
    #region Private member variables
    // Content root
    private GameObject _root;
    // Origin of coordinate system
    private GameObject _origin;
    // Input controller
    private MLInputController _controller = null;
    // LineRenderer from controller
    private PhotonLineRenderer _beamline = null; //***PUN
    // VectorMath handles vector positioning
    private VectorMath _vectorMath = null;
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
    // display the instructions, stored in other script
    GiveInstructions _giveInstructions = null;
    // Is a menu being displayed?
    private bool aMenuIsActive;

    //***PUN
    private string _beamSphereName = "BeamSphere";
    public GameObject particles;
    private PhotonView PV;
    [HideInInspector]
    public bool bCanPlaceVec1 = false;
    [HideInInspector]
    public bool bCanPlaceVec2 = false;
    [HideInInspector]
    public bool bCanPlaceVec1Labels, bCanPlaceV1ComponentLabel, bCanPlaceVec2Labels = false;
    [HideInInspector]
    public bool bCanHideV1Labels = false;


    #endregion

    void Start()
    {
        PV = GetComponent<PhotonView>(); //***PUN

        _root = GameObject.Find("Content Root");
        _origin = _root.transform.Find("Origin").gameObject;
        _controller = MLInput.GetController(MLInput.Hand.Left);
        _beamline = GetComponent<PhotonLineRenderer>(); //***PUN
        _beamSphere = GameObject.Find("BeamSphere");
        //_beamSphere = PhotonNetwork.Instantiate(_beamSphereName, Vector3.zero, Quaternion.identity);
        _vectorMath = GetComponent<VectorMath>();
        _giveInstructions = GetComponent<GiveInstructions>();

        GLOBALS.stage = Stage.m2orig;
        vec = 0;
        if(!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnControllerButtonUp += OnButtonUp;
        MLInput.OnTriggerUp += OnTriggerUp;
        //_beamline.startWidth = 0.007f ;
        //_beamline.endWidth = 0.01f;
        _origin.SetActive(false);
        menuPanel.SetActive(false);
        operationsPanel.SetActive(false);
        placingHead = false;
        _giveInstructions.DisplayText();

        _beamline.SetPosition(0, _controller.Position); //***PUN
        _beamline.SetPosition(1, beamEnd); //***PUN

    }

    void Update()
    {
        aMenuIsActive = (operationsPanel.activeSelf || menuPanel.activeSelf);
        if (GLOBALS.gridOn)
            SnapToGrid();
        HandleBeamPlacement();
        HandleTouchpadInput();
        // if placingHead, have vector head follow beam
        if (placingHead && !aMenuIsActive)
        {
            _vectorMath.PlaceVectorPoint(vec, true, beamEnd);
        }
    }

    public void IncrementStage()
    {
        // increment our stage variable
        GLOBALS.stage++;
        // update instructions textbCanPlaceV1ComponentLabel
        _giveInstructions.DisplayText();
        // play voiceover
        if (GLOBALS.soundOn)
            _giveInstructions.PlayAudio();
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
                case Stage.m2orig:
                    _origin.SetActive(true);
                    _origin.transform.position = beamEnd;
                    //_beamline.enabled = false;

                    _beamline.Disable(); //***PUN

                    IncrementStage();
                    break;

                case Stage.m2rotate:
                    _beamline.enabled = true;                                                         
                    IncrementStage();
                   
                    break;

                case Stage.v1p1:

                    //***PUN
                    if (PV)
                    {
                        PV.RPC("InstantiateParticles", RpcTarget.All, beamEnd);
                    }

                    _vectorMath.PlaceVectorPoint(vec, false, beamEnd);
                    placingHead = true;
                    IncrementStage();
                    break;

                case Stage.v1p2:

                    //***PUN
                    if (PV)
                    {
                        PV.RPC("InstantiateParticles", RpcTarget.All, beamEnd);
                    }

                    placingHead = false;
                    vec++;

                    bCanPlaceVec1 = true; //***PUN
                    bCanPlaceVec1Labels = true; //***PUN

                    IncrementStage();
                    break;

                case Stage.v1calc:
                        // the user must click to launch the animation of the component calculations
                    StartCoroutine(ComponentCalculation(0));
                    // this Corroutine animates the display of calculations
                    // it increments the stage AFTER completion            
                    break;

                case Stage.v2p1:

                    // these two are fine here, for now (but ideally, i want to place them right after comp calc has finished)
                    bCanHideV1Labels = true; //***PUN hide labels 
                    bCanPlaceV1ComponentLabel = true; //***PUN

                    //***PUN
                    if (PV)
                    {
                        PV.RPC("InstantiateParticles", RpcTarget.All, beamEnd);
                    }

                    _vectorMath.SetVectorLabels(0, false, false, true, false);
                    _vectorMath.PlaceVectorPoint(vec, false, beamEnd);
                    placingHead = true;
                    IncrementStage();
                    break;

                case Stage.v2p2:

                    //***PUN
                    if (PV)
                    {
                        PV.RPC("InstantiateParticles", RpcTarget.All, beamEnd);
                    }

                    placingHead = false;

                    bCanPlaceVec2 = true; //***PUN
                    bCanPlaceVec2Labels = true; //***PUN 

                    IncrementStage();
                    break;
                case Stage.v2calc:
                    StartCoroutine(ComponentCalculation(1));
                    // this case it should make the operations panel launch upon completion
                    break;
                case Stage.opSel:
                    _vectorMath.SetVectorLabels(1, false, false, true, false);
                    break;
                case Stage.opView:
                    break;
                default:
                    return;
            }
        }
    }

    // Home or Bumper clicks handled here
    private void OnButtonUp(byte controllerId, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.HomeTap)
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
        else if (button == MLInputControllerButton.Bumper)
        {
            // If we're viewing the completed operation, bumper will toggle the labels we are viewing
            if(GLOBALS.stage == Stage.opView)
            {
                GLOBALS.showingCoords = !GLOBALS.showingCoords;
                if(GLOBALS.showingCoords)
                {
                    switch(GLOBALS.opSelected)
                    {
                        case VecOp.none:
                            break;
                        case VecOp.Addition:
                            _vectorMath.SetVectorLabels(0, false, false, false, false);
                            _vectorMath.SetVectorLabels(1, true, false, false, false);
                            _vectorMath.SetVectorLabels(2, true, true, false, false);
                            break;
                        case VecOp.Subtraction:
                            _vectorMath.SetVectorLabels(0, false, false, false, false);
                            _vectorMath.SetVectorLabels(1, true, false, false, false);
                            _vectorMath.SetVectorLabels(2, true, true, false, false);
                            break;
                        case VecOp.Dot:
                            _vectorMath.SetVectorLabels(0, false, false, false, true);
                            _vectorMath.SetVectorLabels(1, true, false, false, true);
                            break;
                        case VecOp.Cross:
                            _vectorMath.SetVectorLabels(0, false, true, false, false);
                            _vectorMath.SetVectorLabels(1, false, true, false, false);
                            _vectorMath.SetVectorLabels(2, true, true, false, false);
                            break;
                    }
                }
                else
                {
                    _vectorMath.SetVectorLabels(0, false, false, true, false);
                    _vectorMath.SetVectorLabels(1, false, false, true, false);
                    _vectorMath.SetVectorLabels(2, false, false, true, false);
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
            if(GLOBALS.stage == Stage.m2rotate)
            {
                switch (_controller.TouchpadGesture.Direction)
                {
                    case MLInputControllerTouchpadGestureDirection.Clockwise:
                        _root.transform.RotateAround(_origin.transform.position, Vector3.up, 80f * Time.deltaTime);

                        _origin.GetComponent<OriginControl>().SetAxesPositions(); //***PUN
                        break;
                    case MLInputControllerTouchpadGestureDirection.CounterClockwise:
                        _root.transform.RotateAround(_origin.transform.position, Vector3.up, -80f * Time.deltaTime);

                        _origin.GetComponent<OriginControl>().SetAxesPositions(); //***PUN
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

    // this happens whenever the vector head has been placed
    private IEnumerator ComponentCalculation(int v)
    {
        yield return StartCoroutine(_vectorMath.ComponentCalc(v));
        _vectorMath.SetVectorLabels(v, false, false, true, false);

        if(GLOBALS.stage == Stage.v2calc)
        {
            // *** POTENTIAL BUG? ***
            operationsPanel.SetActive(true);           
        }

        IncrementStage();
    }

    #region Operation Panel Buttons

    // Button: Add.onClick()
    public void DoAddition()
    {
        operationsPanel.SetActive(false);
        GLOBALS.opSelected = VecOp.Addition;
        IncrementStage();
        StartCoroutine(_vectorMath.DoVectorAddition());
        _vectorMath.SetVectorLabels(2, false, false, true, false);

      
    }

    // Button: Subtract.onClick()
    public void DoSubtraction()
    {
        operationsPanel.SetActive(false);
        GLOBALS.opSelected = VecOp.Subtraction;
        IncrementStage();
        StartCoroutine(_vectorMath.DoVectorSubtraction());
        _vectorMath.SetVectorLabels(2, false, false, true, false);

        
    }

    // Button: Dot.onClick()
    public void DoDotProduct()
    {
        operationsPanel.SetActive(false);
        GLOBALS.opSelected = VecOp.Dot;
        IncrementStage();
        StartCoroutine(_vectorMath.DoVectorDot());
    }

    // Button: Cross.onClick()
    public void DoCrossProduct()
    {
        operationsPanel.SetActive(false);
        GLOBALS.opSelected = VecOp.Cross;
        GLOBALS.didCross = true;
        IncrementStage();
        StartCoroutine(_vectorMath.DoVectorCross());
        _vectorMath.SetVectorLabels(2, false, false, true, false);

       
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
        GLOBALS.stage = Stage.m2orig;
        GLOBALS.opSelected = VecOp.none;
        GLOBALS.didCross = false;
        vec = 0;
        MLInput.OnTriggerUp -= OnTriggerUp;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

    //***PUN
    [PunRPC]
    public void InstantiateParticles(Vector3 _pos)
    {
        GameObject _particles = Instantiate(particles, _pos, Quaternion.identity);
        _particles.GetComponent<ParticleSystem>().Play();
    }
}
