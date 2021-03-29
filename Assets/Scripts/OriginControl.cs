using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

/*  OriginControl keeps the origin displaying correctly for Module 2
 */

[ExecuteInEditMode]     // can disable if problematic
public class OriginControl : MonoBehaviour
{
    [SerializeField, Tooltip("Magic Leap Main Camera")]
    private Camera _camera;

    [SerializeField, Tooltip("The 6 LineRenderers that display the axes by default.")]
    private List<PhotonLineRenderer> origin_axes; //***PUN
    [SerializeField, Tooltip("The default beam material that colors are applied onto")]
    private Material beamMaterial;
    const float axes_length = 1f;

    [SerializeField] public TextMeshPro xAxisText; //***PUN made public
    [SerializeField] public TextMeshPro yAxisText; //***PUN
    [SerializeField] public TextMeshPro zAxisText; //***PUN
    const float labelTextScale = 0.008f;

    //***PUN
    public RPCReceiver rpcReceiverReference;
   
    void Start()
    {
        InitializeText();
        //InitializeAxes();
    }
    
    void Update()
    {
        // always rotate the labels to the camera
        RotateTextTowardUser();
        if(transform.hasChanged) 
        {
            // transform might have changed due to user placement or rotation
            SetAxesPositions();
            if (SceneManager.GetActiveScene().buildIndex > 1 && SceneManager.GetActiveScene().buildIndex < 13)
                rpcReceiverReference.SetUp_UpdateOriginLabel_RPC(); //***PUN
            
        }
    }

    private void RotateTextTowardUser()
    {
        Quaternion Xrotation = Quaternion.LookRotation(xAxisText.transform.position - _camera.transform.position);
        Quaternion Yrotation = Quaternion.LookRotation(yAxisText.transform.position - _camera.transform.position);
        Quaternion Zrotation = Quaternion.LookRotation(zAxisText.transform.position - _camera.transform.position);
        xAxisText.transform.rotation = Quaternion.Slerp(xAxisText.transform.rotation, Xrotation, 1.5f);
        yAxisText.transform.rotation = Quaternion.Slerp(yAxisText.transform.rotation, Yrotation, 1.5f);
        zAxisText.transform.rotation = Quaternion.Slerp(zAxisText.transform.rotation, Zrotation, 1.5f);
    }

    public void SetAxesPositions() //***PUN made public
    {
        foreach(PhotonLineRenderer linerenderer in origin_axes)
        {
            linerenderer.SetPosition(0, transform.position);
        }

        origin_axes[0].SetPosition(1, transform.position + transform.right * axes_length);
        origin_axes[1].SetPosition(1, transform.position + transform.up * axes_length);
        origin_axes[2].SetPosition(1, transform.position + transform.forward * axes_length * GLOBALS.flipZ);
        origin_axes[3].SetPosition(1, transform.position - transform.right * axes_length);
        origin_axes[4].SetPosition(1, transform.position - transform.up * axes_length);
        origin_axes[5].SetPosition(1, transform.position - transform.forward * axes_length * GLOBALS.flipZ);

        xAxisText.transform.position = transform.position + transform.right * axes_length * 0.3f;
        yAxisText.transform.position = transform.position + transform.up * axes_length * 0.3f;
        zAxisText.transform.position = transform.position + transform.forward * axes_length * 0.3f * GLOBALS.flipZ;
    }

    private void InitializeText()
    {
        xAxisText.text = "X";
        yAxisText.text = "Y";
        zAxisText.text = "Z";
        Vector3 scale = new Vector3(labelTextScale, labelTextScale, labelTextScale);
        xAxisText.transform.localScale = scale;
        yAxisText.transform.localScale = scale;
        zAxisText.transform.localScale = scale;

        //***PUN
        if (SceneManager.GetActiveScene().buildIndex > 1 && SceneManager.GetActiveScene().buildIndex < 13)
        {
            rpcReceiverReference.SetUp_OriginLabel_RPC();
            rpcReceiverReference.SpawnOriginSphere(transform.position);
        }
    }

    //private void InitializeAxes()
    //{
    //    foreach (PhotonLineRenderer linerenderer in origin_axes)
    //    {
    //        linerenderer.startWidth = 0.01f;
    //        linerenderer.endWidth = 0.01f;
    //        linerenderer.material = beamMaterial;
    //    }
    //    float startAlpha = 0.2f;

    //    origin_axes[0].startColor = new Color(1, 0, 0, startAlpha);
    //    origin_axes[0].endColor = new Color(1, 0, 0, 0);
    //    origin_axes[1].startColor = new Color(0, 1, 0, startAlpha);
    //    origin_axes[1].endColor = new Color(0, 1, 0, 0);
    //    origin_axes[2].startColor = new Color(0, 0, 1, startAlpha);
    //    origin_axes[2].endColor = new Color(0, 0, 1, 0);

    //    origin_axes[3].startColor = new Color(1, 1, 1, startAlpha);
    //    origin_axes[3].endColor = new Color(1, 1, 1, 0);
    //    origin_axes[4].startColor = new Color(1, 1, 1, startAlpha);
    //    origin_axes[4].endColor = new Color(1, 1, 1, 0);
    //    origin_axes[5].startColor = new Color(1, 1, 1, startAlpha);
    //    origin_axes[5].endColor = new Color(1, 1, 1, 0);

    //}    
}
