using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PhotonLineRenderer : MonoBehaviour
{
    GameObject BeamBody = null;
    [SerializeField] GameObject BeamBodyPrefab;
    string BeamBodyName = "BeamBody";
    public bool bEnabled = false;
    bool bOriginSet = false;
    bool bEndpointSet = false;

    [HideInInspector]
    public Vector3 OriginLocation; //***PUN made public 
    [HideInInspector]
    public Vector3 EndpointLocation; //***PUN made public 

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
                Debug.Log("Could not find index");
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
        bEndpointSet = true;

        Enable();
    }

    private void DrawBeam()
    {
        // if our beam has not been enabled (spawned) then spawn it first
        if (!bEnabled && PhotonNetwork.InRoom)
        {
            BeamBodyName = BeamBodyPrefab.name;
            BeamBody = PhotonNetwork.Instantiate(BeamBodyName, OriginLocation, Quaternion.identity);
            bEnabled = true;
        }

        RotateVector();
        ScaleVector();
    }

    private void RotateVector()
    {
        if (BeamBody == null) { return; }

        BeamBody.transform.position = OriginLocation;
        BeamBody.transform.LookAt(EndpointLocation);
    }

    private void ScaleVector()
    {
        if (BeamBody == null) { return; }

        float DistanceToTarget = GetDistance(BeamBody.transform.position, EndpointLocation);
        BeamBody.transform.localScale = new Vector3(0.01f, 0.01f, DistanceToTarget / 2f);
    }

    public void Enable()
    {
        if (bOriginSet && bEndpointSet)
        {
            DrawBeam();
        }
    }

    public void Disable()
    {
        if (bEnabled)
        {
            bEnabled = false;
            PhotonNetwork.Destroy(BeamBody);
        }
    }

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        return Vector3.Distance(position1, position2);
    }

    public Vector3 GetPosition(int position)
    {
        switch (position)
        {
            case 0:
                return OriginLocation;
            case 1:
                return EndpointLocation;
            default:
                return OriginLocation;
        }
    }
}
