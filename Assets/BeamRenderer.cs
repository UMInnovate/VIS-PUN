using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamRenderer : MonoBehaviour
{
    GameObject BeamBody = null;
    [SerializeField] GameObject BeamBodyPrefab;
    string BeamBodyName = "BeamBody";
    public bool bEnabled = false;
    bool bOriginSet = false;
    bool bEndpointSet = false;

    [HideInInspector]
    public Vector3 OriginLocation;
    [HideInInspector]
    public Vector3 EndpointLocation; 

    public void SetPosition(int index, Vector3 position)
    {
        switch (index)
        {
            case 0:
                UpdateOriginLocation(position);
                break;
            case 1:
                UpdateEndpointLocation(position);
                break;
            default:
                Debug.Log("Wrong index called in BeamRenderer:SetPosition");
                break;
        }

    }

    private void UpdateOriginLocation(Vector3 position)
    {
        OriginLocation = position;
        bOriginSet = true;

        Enable();
    }

    private void UpdateEndpointLocation(Vector3 position) 
    {
        EndpointLocation = position; 
    }

    public void Enable()
    {
        if(bOriginSet && bEndpointSet)
        {
            DrawBeam();
        }
    }
}
