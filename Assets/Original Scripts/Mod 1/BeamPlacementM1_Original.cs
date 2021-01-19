
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

/*  BeamPlacementM1.cs handles user input and the scene's state for Module 1
 *  This script handles placing the controller beam, opening/closing menus,
 *  and calling GiveInstructions for other functions
 */

public class BeamPlacementM1_Original : MonoBehaviour
{
    // Menu Panel 
    public GameObject menuPanel;
    // Content root
    private GameObject _root;
    // Origin of coordinate system
    [HideInInspector] public GameObject _origin;
    // Input controller
    private MLInput.Controller _controller = null;
    // LineRenderer from controller
    private LineRenderer _beamline = null;
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
    // True when vector head must follow the end of the beam.
    private bool placingHead = false;
    // display the instructions, stored in other script
    GiveInstructions _giveInstructions = null;

    [SerializeField, Tooltip("The vector")]
    public VectorControlM1_Original _vector;
    [SerializeField, Tooltip("The angles")]
    private AngleControl_Original _angles;

    void Start()
    {
        _root = GameObject.Find("Content Root");
        _origin = _root.transform.Find("Origin").gameObject;
        _beamline = GetComponent<LineRenderer>();
        _beamSphere = GameObject.Find("BeamSphere");
        _giveInstructions = GetComponent<GiveInstructions>();

        GLOBALS.stage = Stage.m1orig;
        if (!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnControllerButtonUp += OnButtonUp;
        MLInput.OnTriggerUp += OnTriggerUp;
        _controller = MLInput.GetController(MLInput.Hand.Left);

        _beamline.startWidth = 0.007f;
        _beamline.endWidth = 0.01f;
        _origin.SetActive(false);
        _vector.gameObject.SetActive(false);
        _vector.vecColor = GLOBALS.visMagenta;
        menuPanel.SetActive(false);
        placingHead = false;
        _giveInstructions.DisplayText();
        _angles.SetActive(false);
    }

    void Update()
    {
        HandleBeamPlacement();
        HandleTouchpadInput();

        // if placingHead, then have vector head follow beam
        if (placingHead && !menuPanel.activeSelf)
        {
            _vector._head.position = beamEnd;
            // if the origin needs vector updates, redraw origin
            if (GLOBALS.displayMode == DispMode.Components)
            {
                _origin.GetComponent<OriginControlM1_Original>().DisplayVectorComponents(_vector.GetVectorComponents());
            }
            else if (GLOBALS.displayMode == DispMode.Units)
            {
                _origin.GetComponent<OriginControlM1_Original>().DisplayUnitVectors(_vector.GetVectorComponents(), _origin.transform.position, _vector.GetMagnitude());
            }
        }
    }

    public void IncrementStage()
    {
        GLOBALS.stage++;
        _vector.SetEnabledLabels(GLOBALS.showingCoords);
        _giveInstructions.DisplayText();
    }

    private void HandleBeamPlacement()
    {
        if (_beamline.enabled)
        {
            beamEnd = _controller.Position + (transform.forward * beamLength);
            _beamline.SetPosition(0, _controller.Position);
            _beamline.SetPosition(1, beamEnd);
            _beamSphere.transform.position = beamEnd;
            _beamSphere.SetActive(!menuPanel.activeSelf && !placingHead);
        }
    }

    // grid snapping unused at the moment
    private void SnapToGrid()
    {
        beamEnd /= GLOBALS.gridSize;
        beamEnd = new Vector3(Mathf.Round(beamEnd.x), Mathf.Round(beamEnd.y), Mathf.Round(beamEnd.z));
        beamEnd *= GLOBALS.gridSize;
    }

    // listener for TRIGGER presses
    private void OnTriggerUp(byte controllerId, float pressure)
    {
        if (!menuPanel.activeSelf)
        {
            switch (GLOBALS.stage)
            {
                case Stage.m1orig:
                    _origin.SetActive(true);
                    _origin.transform.position = beamEnd;
                    _beamline.enabled = false;
                    IncrementStage();
                    break;
                case Stage.m1rotate:
                    _beamline.enabled = true;
                    placingHead = true;
                    GLOBALS.showingCoords = true;
                    _vector.gameObject.SetActive(true);
                    IncrementStage();
                    break;
                case Stage.m1vector:
                    _vector._head.position = beamEnd;
                    placingHead = false;
                    IncrementStage();
                    break;
                case Stage.m1view:
                    placingHead = !placingHead;
                    break;
                default:
                    break;
            }
        }
    }

    // listener for HOME and BUMPER presses
    private void OnButtonUp(byte controllerId, MLInput.Controller.Button button)
    {
        Debug.Log("button press");
        if (button == MLInput.Controller.Button.HomeTap)
        {
            Debug.Log("that button is the home button");
            // if opening up the menu, make sure there is a beam and no instructions
            if (!menuPanel.activeSelf)
                _beamline.enabled = true;
            // open the main menu
            menuPanel.SetActive(!menuPanel.activeSelf);
            // display instructions only when no menu
            _giveInstructions.EnableText(!menuPanel.activeSelf);
        }

        if (button == MLInput.Controller.Button.Bumper)
        {
            Debug.Log("that button is the bumper button");
            // change the display mode
            if (GLOBALS.stage != Stage.m1view)
                return;
            if (GLOBALS.displayMode == DispMode.Angles)
                GLOBALS.displayMode = DispMode.Vector;
            else
                GLOBALS.displayMode++;

            // ...and change the display
            switch(GLOBALS.displayMode)
            {
                case DispMode.Vector:
                    _origin.GetComponent<OriginControlM1_Original>().Reset();
                    _angles.SetActive(false);
                    break;
                case DispMode.Components:
                    _origin.GetComponent<OriginControlM1_Original>().DisplayVectorComponents(_vector.GetVectorComponents());
                    _angles.SetActive(false);
                    break;
                case DispMode.Units:
                    _origin.GetComponent<OriginControlM1_Original>().DisplayUnitVectors(_vector.GetVectorComponents(), _origin.transform.position, _vector.GetMagnitude()); 
                    _angles.SetActive(false);
                    break;
                case DispMode.Angles:
                    _origin.GetComponent<OriginControlM1_Original>().Reset();
                    _angles.SetActive(true);
                    break;
            }

            _giveInstructions.DisplayText();
        }
    }

    private void HandleTouchpadInput()
    {
        if (_controller.Touch1Active)
        {
            if (GLOBALS.stage == Stage.m1rotate)
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
            else if (GLOBALS.stage == Stage.m1view)
            {
                switch (_controller.CurrentTouchpadGesture.Type)
                {
                   // _controller.CurrentTouchpadGesture.Direction
                    case MLInput.Controller.TouchpadGesture.GestureType.Swipe: //go to next
                        
                        Debug.Log("swipe to the right in the view mode in m1");
                    if (GLOBALS.displayMode == DispMode.Angles)
                        GLOBALS.displayMode = DispMode.Vector;
                    else
                        GLOBALS.displayMode++;

                    switch (GLOBALS.displayMode)
                    {
                        case DispMode.Vector:
                            _origin.GetComponent<OriginControlM1_Original>().Reset();
                            _angles.SetActive(false);
                        break;
                        case DispMode.Components:
                        _origin.GetComponent<OriginControlM1_Original>().DisplayVectorComponents(_vector.GetVectorComponents());
                        _angles.SetActive(false);
                        break;
                        case DispMode.Units:
                        _origin.GetComponent<OriginControlM1_Original>().DisplayUnitVectors(_vector.GetVectorComponents(), _origin.transform.position, _vector.GetMagnitude());
                        _angles.SetActive(false);
                        break;
                    case DispMode.Angles:
                        _origin.GetComponent<OriginControlM1_Original>().Reset();
                        _angles.SetActive(true);
                        break;
                }
                break;
            
                        /* FOR LATER-- ADD A CASE WHERE THEY CAN GO BACKWARDS 
                         * */
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

    private void OnDisable()
    {
        MLInput.OnTriggerUp -= OnTriggerUp;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }
}

