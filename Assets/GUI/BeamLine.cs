using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class BeamLine : MonoBehaviour
{
    private MLInputController controller = null;

    private LineRenderer beamLine; // lr
    public Vector3 beamEnd;
    private float beamLength = 1.5f; // depth of controller beam   

    void Start()
    {
        MLInput.Start();
        controller = MLInput.GetController(MLInput.Hand.Left);

        beamLine = GetComponent<LineRenderer>();
        beamLine.enabled = true;
        beamLine.startWidth = 0.007f;
        beamLine.endWidth = 0.007f;
    }

    void Update()
    {
        HandleBeamPlacement();
    }

    private void HandleBeamPlacement()
    {
        beamEnd = transform.position + (transform.forward * beamLength);
        beamLine.SetPosition(0, transform.position);
        beamLine.SetPosition(1, beamEnd);
    }
}
