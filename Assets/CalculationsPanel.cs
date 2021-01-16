using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculationsPanel : MonoBehaviour
{
    public Text header;
    // Start is called before the first frame update
    void Start()
    {
       gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCalculationsSequence()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = GLOBALS.pocPos + Vector3.one;
        Debug.Log("calc canv init at " + gameObject.transform.position.ToString());

        header.text = "|" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + "| = " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().GetMagnitude().ToString(GLOBALS.format);
        header.text += "\n" + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().GetVectorComponents().x.ToString(GLOBALS.format) + "i + " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().GetVectorComponents().y.ToString(GLOBALS.format) + "j + " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().GetVectorComponents().z.ToString(GLOBALS.format) + "k";//ToString(GLOBALS.format);       
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
