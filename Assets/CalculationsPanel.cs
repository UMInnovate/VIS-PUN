﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculationsPanel : MonoBehaviour
{
    // private const string V = @"\par";

    // public Text header;
    public List<TEXDraw> textLine; 
    

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
        gameObject.transform.position = GLOBALS.pocPos + new Vector3(0.5f, 0, 0.5f);
    }

    public void CleanCanvas()
    {
        textLine[0].text = "Calc";
        textLine[1].text = "Panel";
    }

    //make smaller font
    public void ComponentCalcs()
    {
        textLine[0].text = "$$" +
           "r_" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + " = (" +
           GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relHeadPos.x.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relTailPos.x.ToString(GLOBALS.format) + @")i + \par(" +
           GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relHeadPos.y.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relTailPos.y.ToString(GLOBALS.format) + @")j + \par(" +
           GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relHeadPos.z.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relTailPos.z.ToString(GLOBALS.format) + @")k $$";
    }



    public void MagCalcs()
    {
        Vector3 relVec = GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._head.position - GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._tail.position;
        textLine[1].text = @"$$\par \par|" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + "| = " +
           @"\sqrt[2]{(" + relVec.x.ToString(GLOBALS.format) + ")^2 + (" + relVec.y.ToString(GLOBALS.format) + ")^2 + (" + relVec.z.ToString(GLOBALS.format) + ")^2 } " +
           @"\par = " + relVec.magnitude.ToString(GLOBALS.format) + "$$";
    }

    public void SystemOfEqs()
    {
        textLine[0].gameObject.SetActive(false);
        textLine[1].gameObject.SetActive(false);
        textLine[2].gameObject.SetActive(true);
        Debug.Log("in soe, tl 0 is " + textLine[0].isActiveAndEnabled + " and t2 is " + textLine[1].isActiveAndEnabled);
        textLine[2].text = " ";
        textLine[2].text = "$$ F_" + GLOBALS.GivenForceVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + "= "
        + GLOBALS.GivenForceVec.GetComponent<VectorProperties>().forceValue.ToString(GLOBALS.format) + " * ("
        + GLOBALS.GivenForceVec.GetComponent<VectorProperties>().uVec.x.ToString(GLOBALS.format) + "i + "
        + GLOBALS.GivenForceVec.GetComponent<VectorProperties>().uVec.y.ToString(GLOBALS.format) + "j + "
        + GLOBALS.GivenForceVec.GetComponent<VectorProperties>().uVec.z.ToString(GLOBALS.format) + "k"
        + ") ";


       // float a = (float)GLOBALS.GivenForceVec.GetComponent<VectorProperties>().forceValue;
        Debug.Log("just printed the known fvec: " + textLine[1].text);

        for (int i = 0; i<= 2; i++)
        {
            textLine[2].text += @"\par \bf F_" + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().gameObject.name.Substring(12) + "= "
            + "|\bf F_" + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().gameObject.name.Substring(12) + "| * ("
            + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().uVec.x.ToString(GLOBALS.format) + "i + "
            + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().uVec.y.ToString(GLOBALS.format) + "j + "
            + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().uVec.z.ToString(GLOBALS.format) + "k"
            + ")";

            Debug.Log("printed unknownvec " + i);
        }
    }


}
