using System.Collections;
using UnityEngine;


public class CollissionManager : MonoBehaviour
{

    public struct ObjectColliddingWithBeam
    {
        public bool onBeamEnter, onBeamExit, onBeamClick;

        public void ResetStatuses(int i = 0)
        {
            onBeamEnter = onBeamExit = onBeamClick = false;
        }
    }


    [Header("Sends input to")]
    [SerializeField] StateManager _stateTaskManager = null;
    public bool trackingCollissions = true;

    [Header("Tacking collissions with")]
    [SerializeField] public GameObject pointerCursor;

    public ObjectColliddingWithBeam objectColliddingWithBeamStatuses = new ObjectColliddingWithBeam();
    public GameObject objectExittingCollissionWithBeam { get; set; }
    public GameObject objectCurrentlyColliddingWithBeam { get; set; }
    public GameObject objectClicked { get; set; }
    public bool colliddingWithSomething { get; set; }

    void Awake()
    {
        pointerCursor.AddComponent<MLBeamCollissionTracker>();
        pointerCursor.GetComponent<MLBeamCollissionTracker>().collissionManager = this;
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="_toggle"></param>
    public void IsTracking(bool _toggle)
    {
        if (trackingCollissions != _toggle)
        {
            objectColliddingWithBeamStatuses.ResetStatuses();
            trackingCollissions = _toggle;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="_collissionStatus"></param>
    /// <param name="_customLog"></param>
    public void CheckCollissionEvents(ref bool _collissionStatus, string _customLog = "")
    {
        if (trackingCollissions)
        {
            _collissionStatus = true;
           // _stateTaskManager.IntraStateCollissionEvents();
            if (_customLog != "") Debug.Log(_customLog);
            _collissionStatus = false;
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void OnBeamClick()
    {
        CheckCollissionEvents(ref objectColliddingWithBeamStatuses.onBeamClick, "On beam click object: " + objectClicked);
    }

    /// <summary>
    /// TODO
    /// </summary>
    public void OnBeamExitsObjectIntoNothing()
    {
        colliddingWithSomething = false;
        objectExittingCollissionWithBeam = objectCurrentlyColliddingWithBeam;
        objectCurrentlyColliddingWithBeam = null;
        objectClicked = null;
        if (objectExittingCollissionWithBeam)
            CheckCollissionEvents(ref objectColliddingWithBeamStatuses.onBeamExit, "On beam exit object: " + objectExittingCollissionWithBeam);
    }

    /// <summary>
    /// TODO
    /// </summary>
    /// <param name="_gObjectCurrentlyColliddingWithBeam"></param>
    public void OnBeamExitsObjectIntoSomething(GameObject _gObjectCurrentlyColliddingWithBeam)
    {
        colliddingWithSomething = true;
        objectExittingCollissionWithBeam = objectCurrentlyColliddingWithBeam;
        objectCurrentlyColliddingWithBeam = _gObjectCurrentlyColliddingWithBeam;
        objectClicked = objectCurrentlyColliddingWithBeam;
        if (objectExittingCollissionWithBeam)
            CheckCollissionEvents(ref objectColliddingWithBeamStatuses.onBeamExit, "On beam exit object: " + objectExittingCollissionWithBeam);
        CheckCollissionEvents(ref objectColliddingWithBeamStatuses.onBeamEnter, "On beam enter object: " + objectCurrentlyColliddingWithBeam);
    }
}