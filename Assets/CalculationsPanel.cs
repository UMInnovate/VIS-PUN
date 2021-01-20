using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculationsPanel : MonoBehaviour
{
   // private const string V = @"\par";

    // public Text header;
    [SerializeField]  private TEXDraw TEXDraw3D;
    

    // Start is called before the first frame update
    void Start()
    {
       gameObject.SetActive(true);
       //TEXDraw3D = GetComponentInChildren<TEXDraw3D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCalculationsSequence()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = GLOBALS.pocPos + new Vector3(0.5f, 0, 0.5f);
        Debug.Log("calc canv init at " + gameObject.transform.position.ToString());

        Vector3 relVec = GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._head.position - GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._tail.position;
        //double a = 20.00;
        // Vector3 rel = new Vector3(a*false )
        // Vector3 rel = new Vector3(a*false )
        TEXDraw3D.text = "$$" +
            "r_" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + " = (" +
            GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._head.localPosition.x.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._tail.localPosition.x.ToString(GLOBALS.format) + ")i + (" +
            GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._head.localPosition.y.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._tail.localPosition.y.ToString(GLOBALS.format) + ")j + (" +
            GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._head.localPosition.z.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._tail.localPosition.z.ToString(GLOBALS.format) + ")k ";
        TEXDraw3D.text +=  @"\par" + "|" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + "| = " + 
            @"\sqrt[2]{(" + relVec.x.ToString(GLOBALS.format)+"^2 ) + (" + relVec.y.ToString(GLOBALS.format) + "^2 )" + relVec.z.ToString(GLOBALS.format) + "^2 )} " + 
            @"\par = " + relVec.magnitude.ToString(GLOBALS.format) + "$$";
    }

    public void MagCalcs()
    {
        //need ref to vectormath, GLOBALS
        //r components
        //"r = " + "(pocPos.x) + (selectVec.x) "+ "i" + "(pocPos.y) + (selectVec.y) "+ "j" + "(pocPos.z) + (selectVec.z) "+ "k"
        //"r = " + "(pocPos.x) + (selectVec.x) "+ "i" + "(pocPos.y) + (selectVec.y) "+ "j" + "(pocPos.z) + (selectVec.z) "+ "k"
        // |r| = \sqrt vectorComps.x^2 +  vectorComps.y^2 + vectorComps.z^2
        // |r| = vectorComps.magnitude
    }
}
