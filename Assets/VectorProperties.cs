using UnityEngine;
using UnityEngine.XR.MagicLeap;
//using MathNet.Numerics.LinearAlgebra;


public class VectorProperties : MonoBehaviour
{
    [HideInInspector]
    public bool isValidPlacement; //is the head or tail component colliding?

    [HideInInspector]
    public bool isForceKnown; //has the user selected/inputted a constant force value

    [HideInInspector]
    public int forceValue; //user-inputted force value

    [HideInInspector]
    public Vector3 forceVec;

    private bool nameLabelHovered;
    [SerializeField] private MLInput.Controller inputController;
    public GameObject keypad;

    [SerializeField] BeamPlacementM3_Original beamPlacement;

    //GetComponent<VectorControlM3_Original()> //get the transform of the vec
    
    #region Public Methods
    public void SetNameLabelHoverState(bool isHovered)
    { nameLabelHovered = isHovered; }

    public void CalculateForceVector()
    {
        Vector3 relVec = GetComponent<VectorControlM3_Original>()._head.position - GetComponent<VectorControlM3_Original>()._tail.position; //r
        float floatrelMag = relVec.magnitude; //|r|
        Vector3 uVec = new Vector3(relVec.x / floatrelMag, relVec.y / floatrelMag, relVec.z / floatrelMag); //u
        forceVec = forceValue * uVec; //FORCE VECTOR, make PUBLIC VAR to access it from where you end up doing the calcs
        /*var M = Matrix<float>.Build;
        float[] x =  { forceVec.x, forceVec.y, forceVec.z };
        var f = M.Dense(1,3,x);
        //float[,] x =  {{ forceVec.x, forceVec.y, forceVec.z }};
        //var f = M.DenseOfArray(x);*/
        Debug.Log("Force Vector " + forceVec.ToString());
    }

    //set force value of vector from keypad panel
    public void SetForceVal(int fval)
    {
        forceValue = fval;

        //REGEX \b([A]|[B]|[C]|[D])
        //SpaceVector A
        string subA = gameObject.name.Substring(12);
        Debug.Log("subA = " + subA);
        string subB = gameObject.name.Substring(11);
        Debug.Log("subB = " + subB);
        if(GLOBALS.inFeet) gameObject.GetComponent<VectorControlM3_Original>().SetName(subA + " = " + fval.ToString() + " lbs");
        else gameObject.GetComponent<VectorControlM3_Original>().SetName(subA + " = " + fval.ToString() + " N");
    }

    #endregion
    
     void Start()
    {
        keypad.SetActive(false);
        inputController = MLInput.GetController(MLInput.Hand.Right);
        if (!MLInput.IsStarted)
            MLInput.Start();
        MLInput.OnTriggerDown += OnTriggerDown;
    }

    private void OnTriggerDown(byte controllerId, float pressure)
    {
        if (nameLabelHovered)
        {
            isForceKnown = true;
            //TRIGGER NEW STATE (ENTRY KEYPAD)
            //origin, content root, content
            GLOBALS.SelectedVec = gameObject;
            if (GLOBALS.stage == Stage.m3forcesel)
            {  //placed vectors, going into force keypad
                Debug.Log("hover detected");
                Debug.Log("trigger press dec vec prop on vector " + gameObject.name);
                keypad.SetActive(true);

                keypad.GetComponent<KeypadPanel>().ReceiveVector(gameObject);
                GLOBALS.stage++; //now in keypad
            }

            }
        }

    }

    
