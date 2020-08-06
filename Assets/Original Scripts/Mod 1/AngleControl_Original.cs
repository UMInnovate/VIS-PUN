using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*  AngleControl.cs handles placing the angle arcs and labels for Module 1.
 *  Consists of 32-pt line renderer angle arcs and TextMeshPro angle labels
 *  Similar to VectorMath.cs -> CreateArc()
 */

public class AngleControl_Original : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private VectorControlM1_Original vector;

    [SerializeField] private List<TextMeshPro> _labels;
    [SerializeField] private List<LineRenderer> _arcs;

    private bool isActive = false;
    private int arcPosCount = 32;
    private float arcRad = 0.05f;
    private float arcWidth = 0.005f;

    private void Start()
    {
        foreach(LineRenderer arc in _arcs)
        {
            arc.positionCount = arcPosCount;
            arc.startWidth = arcWidth;
            arc.endWidth = arcWidth;
        }
    }

    private void Update()
    {
        if (isActive)
        {
            RotateLabelsTowardUser();            
        }
        if (vector.transform.hasChanged)
        {
            Redraw();
        }
    }

    public void SetActive(bool b)
    {
        isActive = b;
        gameObject.SetActive(b);
        if (isActive)
            Redraw();
    }

    private void Redraw()
    {
        Vector3 center = transform.position;
        Vector3 posA = center;
        Vector3 posB = vector._head.forward * arcRad;
        Vector3 temp = posA;

        // iterate over X, Y, Z
        for (int i = 0; i < 3; i++)
        {
            switch(i)
            {
                case 0:
                    posA = transform.right * arcRad;
                    break;
                case 1:
                    posA = transform.up * arcRad;
                    break;
                case 2:
                    posA = transform.forward * GLOBALS.flipZ * arcRad;
                    break;
            }
            temp = posA;
            float theta = Vector3.Angle(posA, posB);
            _labels[i].text = theta.ToString("F0") + "°";
            _labels[i].transform.position = center + posA + posB;

            float deltaTheta = Mathf.Deg2Rad * theta / arcPosCount;
            for (int n = 0; n < arcPosCount; n++)
            {
                _arcs[i].SetPosition(n, center + temp);
                temp = Vector3.RotateTowards(temp, posB, deltaTheta, 0.0f);
            }
        }
    }

    private void RotateLabelsTowardUser()
    {
        foreach(TextMeshPro tmp in _labels)
        {
            Quaternion labelRotation = Quaternion.LookRotation(tmp.transform.position - _camera.transform.position);
            tmp.transform.rotation = Quaternion.Slerp(tmp.transform.rotation, labelRotation, 1.5f);
        }
    }
}
