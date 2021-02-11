using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

[ExecuteInEditMode]
public class VectorControlM3 : MonoBehaviour
{
    #region public references
    public Camera _camera;
    public Transform _head;
    public Transform _tail;
    public TextMeshPro _xCompLabel;
    public TextMeshPro _yCompLabel;
    public TextMeshPro _zCompLabel;
    #endregion

    public Color vecColor;
    private Color lastColor;
    
    private PhotonLineRenderer _body;
    private float beamWidth = 0.01f;
    private float mag;

    private const float textSize = 0.005f;
    private const float textOffset = 0.05f;
    private const float viewOffset = 0.07f;
    [HideInInspector] public Vector3 relHeadPos;
    [HideInInspector] public Vector3 relTailPos;
    private Vector3 vectorComponents;

    [HideInInspector] public bool isCorrectPlacement = false;
    [HideInInspector] public bool unitVec;

    [SerializeField, Tooltip("xComp, yComp, zComp objects")]
    private List<LineRenderer> comps;
    [SerializeField, Tooltip("Show/Hide xComp, yComp, zComp")]
    private bool showingUnits;

    [SerializeField] private Material beamMaterial;
    [SerializeField] public TextMeshPro _nameLabel;
    [SerializeField] public TextMeshPro _headLabel;
    [SerializeField] public TextMeshPro _tailLabel;
    [SerializeField] private TextMeshPro _componentLabel;

    //***PUN
    public GameObject _headGameObjectPrefab;
    public GameObject _tailGameObjectPrefab;
    public GameObject _headGameObject;
    public GameObject _tailGameObject;
    public RPCReceiverM3 rpcReceiverReference;

    private PhotonView photonView;

    private void Start()
    {
        _body = GetComponent<PhotonLineRenderer>();
        _tail = transform.Find("Tail");
        _head = transform.Find("Head");
        ///PUN _body.startWidth = beamWidth;
        ///PUN _body.endWidth = beamWidth;
        ///
        photonView = GetComponent<PhotonView>();
        SpawnHeadAndTail(); //PUN
        InitLabels();
       // if(photonView) photonView.RPC("InitLabels", RpcTarget.AllBuffered);
        SetEnabledLabels(false, false, false, false);
        unitVec = false;
      //  InitComps();
    }

    private void Update()
    {
        if (vecColor != lastColor) // keeps track of last color
            RecolorVector();
        if (_tail.transform.hasChanged)
        {
            // tail transform should not change, only the SpaceVector transform
            _tail.transform.position = transform.position;
        }
        if (transform.hasChanged || _head.transform.hasChanged)
        {
            RebuildVector();
            if (showingUnits)
                RebuildComps();
        }
        RotateLabelsTowardUser();
        
    }

    public void SetUnitVec(bool i)
    {
        unitVec = i;
    }

    // gets called by VectorMath
    public void SetEnabledLabels(bool tail, bool head, bool components, bool units)
    {
        _tailLabel.enabled = tail;
        _headLabel.enabled = head;
        _componentLabel.enabled = components;
        _xCompLabel.enabled = units;
        _yCompLabel.enabled = units;
        _zCompLabel.enabled = units;
        showingUnits = units;
        foreach (LineRenderer linerenderer in comps)
        {
            linerenderer.enabled = units;
        }
    }

    public void SetName(string n)
    {
        _nameLabel.text = n;
    }

    public Vector3 GetVectorComponents()
    {
        return vectorComponents;
    }

    public Vector3 GetRelHeadPos()
    {
        return relHeadPos;
    }

    public Vector3 GetRelTailPos()
    {
        return relTailPos;
    }

    public float GetMagnitude()
    {
        return mag;
    }

    // happens in Update() if color changed
    private void RecolorVector()
    {
        //PUN _body.startColor = vecColor;
        //PUN _body.endColor = vecColor;
        _head.gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = vecColor;
        _tail.gameObject.GetComponent<MeshRenderer>().sharedMaterial.color = vecColor;
        lastColor = vecColor;
    }

    [PunRPC]
    private void InitLabels()
    {
        Vector3 scale = new Vector3(textSize, textSize, textSize);
        _headLabel.transform.localScale = scale;
        _tailLabel.transform.localScale = scale;
        _nameLabel.transform.localScale = scale;
        _componentLabel.transform.localScale = scale;
        _xCompLabel.transform.localScale = scale;
        _yCompLabel.transform.localScale = scale;
        _zCompLabel.transform.localScale = scale;
        _headLabel.text = "X.X, Y.Y, Z.Z";
        _tailLabel.text = "X.X, Y.Y, Z.Z";
        _componentLabel.text = "X.X, Y.Y, Z.Z";
        _xCompLabel.text = "VecX";
        _yCompLabel.text = "VecY";
        _zCompLabel.text = "VecZ";
    }

    private void InitComps()
    {
        foreach (LineRenderer linerenderer in comps)
        {
            linerenderer.SetPosition(0, transform.position);
            linerenderer.SetPosition(1, transform.position);
            linerenderer.startWidth = 0.006f;
            linerenderer.endWidth = 0.006f;
            linerenderer.material = beamMaterial;
        }
        comps[0].startColor = Color.red;
        comps[0].endColor = Color.red;
        comps[1].startColor = Color.green;
        comps[1].endColor = Color.green;
        comps[2].startColor = Color.blue;
        comps[2].endColor = Color.blue;
    }

    public void DisplayVectorComponent()
    {
        InitComps();
        comps[0].SetPosition(0, _tail.position);
        comps[1].SetPosition(0, _tail.position);
        comps[2].SetPosition(0, _tail.position);

        comps[0].SetPosition(1, transform.position + transform.right * vectorComponents.x);
        comps[1].SetPosition(1, transform.position + transform.up * vectorComponents.y);
        comps[2].SetPosition(1, transform.position + transform.forward * vectorComponents.z);

    }

    private void RebuildVector()
    {
        _body.SetPosition(0, transform.position);
        _body.SetPosition(1, _head.transform.position);
        _head.transform.rotation = Quaternion.LookRotation(_head.transform.position - transform.position);


        //***PUN
        _headGameObject.transform.rotation = _head.rotation;
        _headGameObject.transform.position = _head.position;
        _tailGameObject.transform.position = _tail.position;


        // local positions are needed because Vectors must be childed to Origin
        relTailPos = transform.localPosition;
        relHeadPos = relTailPos + _head.localPosition;
        vectorComponents = relHeadPos - relTailPos;
        mag = vectorComponents.magnitude;
    }

    private void RebuildComps()
    {
        foreach (LineRenderer linerenderer in comps)
        {
            linerenderer.SetPosition(0, transform.position);
        }

        comps[0].SetPosition(1, transform.position + transform.right * vectorComponents.x);
        comps[1].SetPosition(1, transform.position + transform.up * vectorComponents.y);
        comps[2].SetPosition(1, transform.position + transform.forward * vectorComponents.z);

        if (unitVec)
        {
            comps[0].SetPosition(1, transform.position + transform.right * (vectorComponents.x / vectorComponents.magnitude));
            comps[1].SetPosition(1, transform.position + transform.up * (vectorComponents.y / vectorComponents.magnitude));
            comps[2].SetPosition(1, transform.position + transform.forward * (vectorComponents.z / vectorComponents.magnitude));
        }
    }

    [PunRPC]
    private void RotateLabelsTowardUser()
    {
        // ex. for nameLabel, we choose to put it at half the length of the vector relative to the origin 
        _nameLabel.transform.localPosition = vectorComponents * 0.5f;
        Quaternion nameRotation = Quaternion.LookRotation(_nameLabel.transform.position - _camera.transform.position);
        _nameLabel.transform.rotation = Quaternion.Slerp(_nameLabel.transform.rotation, nameRotation, 1.5f);
        _nameLabel.transform.position -= _nameLabel.transform.forward * viewOffset;

        if (_headLabel.enabled)
        {
            _headLabel.transform.position = _head.transform.position + _head.transform.forward * textOffset;
            _headLabel.text = MakeCoordLabel(relHeadPos);
            Quaternion headRotation = Quaternion.LookRotation(_headLabel.transform.position - _camera.transform.position);
            _headLabel.transform.rotation = Quaternion.Slerp(_headLabel.transform.rotation, headRotation, 1.5f);
        }

        if (_tailLabel.enabled)
        {
            _tailLabel.transform.position = transform.position - _head.transform.forward * textOffset;
            _tailLabel.text = MakeCoordLabel(relTailPos);
            Quaternion tailRotation = Quaternion.LookRotation(_tailLabel.transform.position - _camera.transform.position);
            _tailLabel.transform.rotation = Quaternion.Slerp(_tailLabel.transform.rotation, tailRotation, 1.5f);
        }
        if (_componentLabel.enabled)
        {
            _componentLabel.transform.localPosition = vectorComponents * 0.5f;
            if (unitVec)
                _componentLabel.text = MakeCompLabel(vectorComponents / vectorComponents.magnitude);
            else _componentLabel.text = MakeCompLabel(vectorComponents);
            Quaternion compRotation = Quaternion.LookRotation(_componentLabel.transform.position - _camera.transform.position);
            _componentLabel.transform.rotation = Quaternion.Slerp(_componentLabel.transform.rotation, compRotation, 1.5f);
            _componentLabel.transform.position -= _componentLabel.transform.forward * viewOffset;
        }
        if (_xCompLabel.enabled)    // all x, y, z display together
        {
            _xCompLabel.transform.position = comps[0].GetPosition(1);
            _yCompLabel.transform.position = comps[1].GetPosition(1);
            _zCompLabel.transform.position = comps[2].GetPosition(1);

            _xCompLabel.text = "|" + _nameLabel.text.Substring(0, 1) + "x|\n(" + vectorComponents.x.ToString(GLOBALS.format) + ")";
            _yCompLabel.text = "|" + _nameLabel.text.Substring(0, 1) + "y|\n(" + vectorComponents.y.ToString(GLOBALS.format) + ")";
            _zCompLabel.text = "|" + _nameLabel.text.Substring(0, 1) + "z|\n(" + vectorComponents.z.ToString(GLOBALS.format) + ")";

            Quaternion xRotation = Quaternion.LookRotation(_xCompLabel.transform.position - _camera.transform.position);
            _xCompLabel.transform.rotation = Quaternion.Slerp(_xCompLabel.transform.rotation, xRotation, 1.5f);

            Quaternion yRotation = Quaternion.LookRotation(_yCompLabel.transform.position - _camera.transform.position);
            _yCompLabel.transform.rotation = Quaternion.Slerp(_yCompLabel.transform.rotation, yRotation, 1.5f);

            Quaternion zRotation = Quaternion.LookRotation(_zCompLabel.transform.position - _camera.transform.position);
            _zCompLabel.transform.rotation = Quaternion.Slerp(_zCompLabel.transform.rotation, zRotation, 1.5f);
        }
        // Handle positioning & rotation of any new vector labels here
    }

    #region Animations
    // Translating the entire vector via the tail
    public IEnumerator AnimateVectorTranslation(Vector3 tailEndPos)
    {
        WaitForSeconds deltaTime = new WaitForSeconds(GLOBALS.waitTime);
        int numSteps = (int)(GLOBALS.animTime / GLOBALS.waitTime) + 1;
        float[] curve = GenerateLogisticCurve(numSteps);

        Vector3 tailStartPos = transform.position;
        Vector3 tailDeltaPos = tailEndPos - tailStartPos;

        for (int i = 0; i < numSteps; i++)
        {
            transform.position = tailStartPos + (tailDeltaPos * curve[i]);
            yield return deltaTime;
        }

    }

    // Moving the vector head only
    public IEnumerator AnimateVectorHead(Vector3 headEndPos)
    {
        WaitForSeconds deltaTime = new WaitForSeconds(GLOBALS.waitTime);
        int numSteps = (int)(GLOBALS.animTime / GLOBALS.waitTime) + 1;
        float[] curve = GenerateLogisticCurve(numSteps);

        Vector3 headStartPos = _head.transform.position;
        Vector3 headDeltaPos = headEndPos - headStartPos;

        for (int i = 0; i < numSteps; i++)
        {
            _head.transform.position = headStartPos + (headDeltaPos * curve[i]);
            yield return deltaTime;
        }
        _head.transform.position = headEndPos;
    }

    // Moving the vector head only
    public IEnumerator AnimateVector(Vector3 tailEndPos, Vector3 headEndPos)
    {
        WaitForSeconds deltaTime = new WaitForSeconds(GLOBALS.waitTime);
        int numSteps = (int)(GLOBALS.animTime / GLOBALS.waitTime) + 1;
        float[] curve = GenerateLogisticCurve(numSteps);

        Vector3 tailStartPos = transform.position;
        Vector3 tailDeltaPos = tailEndPos - tailStartPos;

        Vector3 headStartPos = _head.transform.position;
        Vector3 headDeltaPos = headEndPos - headStartPos;

        for (int i = 0; i < numSteps; i++)
        {
            transform.position = tailStartPos + (tailDeltaPos * curve[i]);
            _head.transform.position = headStartPos + (headDeltaPos * curve[i]);
            yield return deltaTime;
        }
        transform.position = tailEndPos;
        _head.transform.position = headEndPos;
    }
    #endregion

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

    private string MakeCompLabel(Vector3 vector3)
    {
        // component formatting is "0.00i, 1.11j, 2.22k" on the center point
        string answer;
        if (GLOBALS.inFeet)
        {
            answer = (vector3.x * GLOBALS.m2ft).ToString(GLOBALS.format) + "i, "
                + (vector3.y * GLOBALS.m2ft).ToString(GLOBALS.format) + "j, "
                + (vector3.z * GLOBALS.flipZ * GLOBALS.m2ft).ToString(GLOBALS.format) + "k";
        }
        else
            answer = vector3.x.ToString(GLOBALS.format) + "i, " + vector3.y.ToString(GLOBALS.format) + "j, " + vector3.z.ToString(GLOBALS.format) + "k";
        return answer;
    }

    // generates a "logistic" growth curve ranging from 0 to 1 over numPts values - used for animation speed
    // "logistic" being based on a piecewise quadratic equation
    // this computation is done in advance of the animations
    public float[] GenerateLogisticCurve(int numPts)
    {
        float[] curve = new float[numPts];
        float inc = 1f / numPts;
        for (int i = 0; i < numPts; i++)
        {
            float val = inc * i;
            if (i < (numPts / 2))
                curve[i] = val * val * 2f;
            else
                curve[i] = 1f - (2f * (1f - val) * (1f - val));
        }
        return curve;
    }

    //*** PUN
    public void SpawnHeadAndTail()
    {
        Debug.Log("spawning Head and tail");
        Vector3 headPos = _head.position;
        Vector3 tailPos = _tail.position;
        Quaternion headRot = _head.localRotation; //change from rot to localrot
        Quaternion tailRot = _tail.localRotation;


        string _headGameObjectName = _headGameObjectPrefab.name;
        string _tailGameObjectName = _tailGameObjectPrefab.name;
        _headGameObject = PhotonNetwork.Instantiate(_headGameObjectName, headPos, headRot);
        _tailGameObject = PhotonNetwork.Instantiate(_tailGameObjectName, tailPos, tailRot);
    }

}
