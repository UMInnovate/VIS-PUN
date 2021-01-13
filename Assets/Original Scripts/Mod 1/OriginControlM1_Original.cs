using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*  OriginControlM1 keeps the origin displaying correctly for Module 1 
 */

[ExecuteInEditMode]
public class OriginControlM1_Original : MonoBehaviour
{
    const float axes_length = 1f;
    const float startAlpha = 0.2f;
    const float labelTextScale = 0.008f;

    [SerializeField, Tooltip("Magic Leap Main Camera")]
    private Camera _camera;
    [SerializeField, Tooltip("The 6 LineRenderers that display the axes by default.")]
    private List<LineRenderer> origin_axes;
    [SerializeField, Tooltip("The default beam material that colors are applied onto")]
    private Material beamMaterial;

    // labels on x, y, z axes
    [SerializeField] private TextMeshPro xAxisText;
    [SerializeField] private TextMeshPro yAxisText;
    [SerializeField] private TextMeshPro zAxisText;

    private void Start()
    {
        Reset();
    }

    private void Update()
    {
        // always rotate labels
        RotateLabelsTowardUser();

        // handle origin rotation
        if (GLOBALS.stage == Stage.m1rotate && transform.hasChanged)
        {
            SetAxesPositions();
        }
    }

    // called by Start()
    // called by BeamPlacementM1.cs for changing display modes
    public void Reset()
    {
        InitializeText();
        InitializeAxes();
    }

    // Displays the vector components
    public void DisplayVectorComponents(Vector3 vectorComps)
    {
        origin_axes[0].endColor = new Color(1, 0, 0, startAlpha); // red component color for positive x
        origin_axes[1].endColor = new Color(0, 1, 0, startAlpha); // green component color for positive y
        origin_axes[2].endColor = new Color(0, 0, 1, startAlpha); // blue component color for positive z

        origin_axes[3].enabled = false;  // only use 3 line renderers to display code
        origin_axes[4].enabled = false;
        origin_axes[5].enabled = false;

        origin_axes[0].SetPosition(1, transform.position + transform.right * vectorComps.x);  // set position(1=endpoint position) of origin location of x
        origin_axes[1].SetPosition(1, transform.position + transform.up * vectorComps.y); // set position(1=endpoint position) of origin location of y
        origin_axes[2].SetPosition(1, transform.position + transform.forward * vectorComps.z); // set position(1=endpoint position) of origin location of z

        // update text labels for default vector values
        if (GLOBALS.inFeet)
        {
            xAxisText.text = (vectorComps.x * GLOBALS.m2ft).ToString(GLOBALS.format);
            yAxisText.text = (vectorComps.y * GLOBALS.m2ft).ToString(GLOBALS.format);
            zAxisText.text = ((-1* vectorComps.z) * GLOBALS.m2ft).ToString(GLOBALS.format);
        }
        else
        {
            xAxisText.text = vectorComps.x.ToString(GLOBALS.format);
            yAxisText.text = vectorComps.y.ToString(GLOBALS.format);
            zAxisText.text = (-1 * vectorComps.z).ToString(GLOBALS.format);
        }
    }

    /*  Display unit vectors
     *  displays the X, Y, and Z line renderers at 1 ft length
     */
    public void DisplayUnitVectors(Vector3 vectorComps, float mag)
    {
        origin_axes[0].endColor = new Color(1, 0, 0, startAlpha); // red component color for positive x
        origin_axes[1].endColor = new Color(0, 1, 0, startAlpha); // green component color for positive y
        origin_axes[2].endColor = new Color(0, 0, 1, startAlpha); // blue component color for positive z

        origin_axes[3].enabled = false;  // only use 3 line renderers to display code
        origin_axes[4].enabled = false;
        origin_axes[5].enabled = false;

        Vector3 adjVec = new Vector3((vectorComps.x / mag) + transform.position.x, (vectorComps.y / mag) + transform.position.y, (vectorComps.z / mag) + transform.position.z);
        if (!GLOBALS.inFeet) // sets position in ft.
        {
            origin_axes[0].SetPosition(1, transform.position + transform.right * vectorComps.x/vectorComps.magnitude);  // set position(1=endpoint position) of origin location of x
            origin_axes[1].SetPosition(1, transform.position + transform.up * vectorComps.y / vectorComps.magnitude); // set position(1=endpoint position) of origin location of y
            origin_axes[2].SetPosition(1, transform.position + transform.forward * vectorComps.z / vectorComps.magnitude); // set position(1=endpoint position) of origin location of z

            //and update the text
            xAxisText.text = (vectorComps.x/vectorComps.magnitude).ToString(GLOBALS.format);
            yAxisText.text = (vectorComps.y/vectorComps.magnitude).ToString(GLOBALS.format);
            zAxisText.text = (-1 * vectorComps.z/vectorComps.magnitude).ToString(GLOBALS.format);
        }
        // update text labels for default vector values
        else {
            origin_axes[0].SetPosition(1,( transform.position + transform.right * vectorComps.x / vectorComps.magnitude )/GLOBALS.m2ft);  // set position(1=endpoint position) of origin location of x
            origin_axes[1].SetPosition(1, (transform.position + transform.up * vectorComps.y / vectorComps.magnitude)/GLOBALS.m2ft); // set position(1=endpoint position) of origin location of y
            origin_axes[2].SetPosition(1, (transform.position + transform.forward * vectorComps.z / vectorComps.magnitude)/GLOBALS.m2ft); // set position(1=endpoint position) of origin location of z
            //and update the text axes
            xAxisText.text = (vectorComps.x/vectorComps.magnitude * GLOBALS.m2ft).ToString(GLOBALS.format);
            yAxisText.text = (vectorComps.y/vectorComps.magnitude * GLOBALS.m2ft).ToString(GLOBALS.format);
            zAxisText.text = ((-1 * vectorComps.z/vectorComps.magnitude) * GLOBALS.m2ft).ToString(GLOBALS.format);
        }

    }

    // rotating labels to camera every frame
    private void RotateLabelsTowardUser()
    {
        Quaternion Xrotation = Quaternion.LookRotation(xAxisText.transform.position - _camera.transform.position);
        Quaternion Yrotation = Quaternion.LookRotation(yAxisText.transform.position - _camera.transform.position);
        Quaternion Zrotation = Quaternion.LookRotation(zAxisText.transform.position - _camera.transform.position);
        xAxisText.transform.rotation = Quaternion.Slerp(xAxisText.transform.rotation, Xrotation, 1.5f);
        yAxisText.transform.rotation = Quaternion.Slerp(yAxisText.transform.rotation, Yrotation, 1.5f);
        zAxisText.transform.rotation = Quaternion.Slerp(zAxisText.transform.rotation, Zrotation, 1.5f);
    }

    // called by Update() to process rotation
    // called by InitializeAxes()
    private void SetAxesPositions()
    {
        foreach (LineRenderer linerenderer in origin_axes)
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
    }

    private void InitializeAxes()
    {
        foreach (LineRenderer linerenderer in origin_axes)
        {
            linerenderer.enabled = true;
            linerenderer.startWidth = 0.01f;
            linerenderer.endWidth = 0.01f;
            linerenderer.material = beamMaterial;
        }
        
        origin_axes[0].startColor = new Color(1, 0, 0, startAlpha);
        origin_axes[0].endColor = new Color(1, 0, 0, 0);
        origin_axes[1].startColor = new Color(0, 1, 0, startAlpha);
        origin_axes[1].endColor = new Color(0, 1, 0, 0);
        origin_axes[2].startColor = new Color(0, 0, 1, startAlpha);
        origin_axes[2].endColor = new Color(0, 0, 1, 0);

        origin_axes[3].startColor = new Color(1, 1, 1, startAlpha);
        origin_axes[3].endColor = new Color(1, 1, 1, 0);
        origin_axes[4].startColor = new Color(1, 1, 1, startAlpha);
        origin_axes[4].endColor = new Color(1, 1, 1, 0);
        origin_axes[5].startColor = new Color(1, 1, 1, startAlpha);
        origin_axes[5].endColor = new Color(1, 1, 1, 0);
        SetAxesPositions();
    }
}
