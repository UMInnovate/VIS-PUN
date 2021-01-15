using UnityEngine;

/// <summary>
/// Event Listener for objects that interrupt the raycast of the Magic Leap Controller Beam (collidding with it)
/// </summary>
public class MLBeamCollissionTracker : MonoBehaviour
{
    public CollissionManager collissionManager;
    int _beamTriggeredFrames { get; set; } // TODO have global calculations of frames
    int _beamTotalFrames { get; set; } // TODO have global calculations of frames
    void OnTriggerStay(Collider other)
    {
        _beamTriggeredFrames++;
        if (other.gameObject != collissionManager.objectCurrentlyColliddingWithBeam)
        {
            collissionManager.OnBeamExitsObjectIntoSomething(other.gameObject);
        }
    }
    void FixedUpdate()
    {
        _beamTotalFrames++;
        if ((_beamTriggeredFrames + 10) < _beamTotalFrames && collissionManager.objectCurrentlyColliddingWithBeam)
        {
            collissionManager.OnBeamExitsObjectIntoNothing();
            _beamTriggeredFrames = _beamTotalFrames = 0;
        }
    }
}