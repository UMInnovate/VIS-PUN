using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]

/**
 * vector control for module 1
 */

public class VectorControlM1_Original : MonoBehaviour
{
    public BeamPlacementM1_Original BeamPlacementM1_Original;
    public Camera _camera;
    public Transform _head;
    public Color vecColor;

    private Color lastColor;
    private LineRenderer _body;
    private float beamWidth = 0.01f;
    private float mag;

    private const float textSize = 0.0065f;
    private const float textOffset = 0.05f;
    private Vector3 relHeadPos;
    private Vector3 vectorComponents;

    [SerializeField] private Material beamMaterial;
    [SerializeField] private TextMeshPro _headLabel;
    [SerializeField, Tooltip("The origin sphere at the tail of the vector")]
    private GameObject _origin;

    void Start()
    {
        _body = GetComponent<LineRenderer>();
        _head = transform.Find("Head");

        _body.startWidth = beamWidth;
        _body.endWidth = beamWidth;
        _body.SetPosition(0, _origin.transform.position);
        InitLabels();

        SetEnabledLabels(true);
    }

    void Update()
    {
        if (vecColor != lastColor)
            RecolorVector();
        if (transform.hasChanged)
        {
            transform.position = _origin.transform.position;
        }
        if(_head.transform.hasChanged)
        {
            RebuildVector();
        }
        RotateLabelsTowardUser();
    }

    public void SetEnabledLabels(bool head)
    {
        _headLabel.enabled = head;
    }

    public Vector3 GetVectorComponents()
    {
        return vectorComponents;
    }

    public Vector3 GetRelHeadPos()
    {
        return relHeadPos;
    }

    private void RecolorVector()
    {
        _body.startColor = vecColor;
        _body.endColor = vecColor;
        _head.gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = vecColor;
        lastColor = vecColor;
    }

    private void InitLabels()
    {
        Vector3 scale = new Vector3(textSize, textSize, textSize);
        _headLabel.transform.localScale = scale;
        _headLabel.text = "X.X, Y.Y, Z.Z";
    }

    private void RebuildVector()
    {
        _body.SetPosition(1, _head.transform.position);
        _head.transform.rotation = Quaternion.LookRotation(_head.transform.position - transform.position);

        // local positions are needed because Vectors must be childed to Origin
        //vectorComponents = _head.transform.localPosition;
        vectorComponents = _head.transform.position - BeamPlacementM1_Original._origin.transform.position;
       // vectorComponents = Vector3.Distance(, _origin.transform.position); testing out a dif way of getting rel head, not successful
        mag = _head.transform.localPosition.magnitude;
    }

    private void RotateLabelsTowardUser()
    {
        if (_headLabel.enabled)
        {
            _headLabel.transform.position = _head.transform.position + _head.transform.forward * textOffset;
            _headLabel.text = MakeCoordLabel(vectorComponents);
            Quaternion headRotation = Quaternion.LookRotation(_headLabel.transform.position - _camera.transform.position);
            _headLabel.transform.rotation = Quaternion.Slerp(_headLabel.transform.rotation, headRotation, 1.5f);
        }
    }

    public float GetMagnitude()
    { return mag; }

    private string MakeCoordLabel(Vector3 vector3)
    {
        // coordinate formatting is "(0.00, 1.11, 2.22)" on each endpoint
        string answer;
        if (GLOBALS.inFeet)
        {
            answer = "(" + (vector3.x * GLOBALS.m2ft).ToString(GLOBALS.format) + ", "
                + (vector3.y * GLOBALS.m2ft).ToString(GLOBALS.format) + ", "
                + (vector3.z * GLOBALS.flipZ * GLOBALS.m2ft).ToString(GLOBALS.format) + ")";
        }
        else
            answer = "(" + vector3.x.ToString(GLOBALS.format) + ", " + vector3.y.ToString(GLOBALS.format) + ", " + (vector3.z * GLOBALS.flipZ).ToString(GLOBALS.format) + ")";
        return answer;
    }
}
