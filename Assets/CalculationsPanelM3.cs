using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

 

public class CalculationsPanelM3 : MonoBehaviour
{

    public List<TEXDraw> textLine;



    [SerializeField] private BeamPlacementM3 bp3;

    public void StartCalculationsSequence()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = GLOBALS.pocPos + new Vector3(0.5f, 0, 0.5f);
        gameObject.transform.rotation = Quaternion.LookRotation((GLOBALS.pocPos + new Vector3(0.5f, 0f, 0.5f)) -
            Camera.main.transform.position);

        for(int i = 0; i < 3; i++)
        {
            textLine[i] = GetComponentsInChildren<TEXDraw>()[i];
        }
     //   CleanCanvas();
       // Console.WriteLine(textLine[0].text);
   
    }

    public void CleanCanvas()
    {
        textLine[0].text = "Select a vector label. ";
     //   textLine[1].text = "Panel";
    }

    //make smaller font
    public void ComponentCalcs()
    {
        if (bp3.bIsViewer)
        {
            textLine[0].text = "$$" +
                    "r_" + GLOBALS.SelectedVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = (" +
                    bp3.adjPOCPos.x.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().photonPos.x.ToString(GLOBALS.format) + @")i + \par(" +
                   bp3.adjPOCPos.y.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().photonPos.y.ToString(GLOBALS.format) + @")j + \par(" +
                    bp3.adjPOCPos.z.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().photonPos.z.ToString(GLOBALS.format) + @")k $$";

        }
        else
        {
            textLine[0].text = "$$" +
                    "r_" + GLOBALS.SelectedVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = (" +
                    bp3.adjPOCPos.x.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relTailPos.x.ToString(GLOBALS.format) + @")i + \par(" +
                    bp3.adjPOCPos.y.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relTailPos.y.ToString(GLOBALS.format) + @")j + \par(" +
                    bp3.adjPOCPos.z.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relTailPos.z.ToString(GLOBALS.format) + @")k $$";
        }
    }


    public void MagCalcs()
    {
        
            Vector3 relVec = GLOBALS.pocPos - GLOBALS.SelectedVec.GetComponent<VectorControlM3>().transform.position;
           textLine[1].text = @"$$ \begin{center} \par \par|" + GLOBALS.SelectedVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + "| = " +
               @"\sqrt[2]{(" + relVec.x.ToString(GLOBALS.format) + ")^2 + (" + relVec.y.ToString(GLOBALS.format) + ")^2 + (" + relVec.z.ToString(GLOBALS.format) + ")^2 } " +
               @"\par = " + relVec.magnitude.ToString(GLOBALS.format) + @"\end{center} $$";
    }

    public void SystemOfEqs()
    {
        
            textLine[0].gameObject.SetActive(false);
            textLine[1].gameObject.SetActive(false);
            textLine[2].gameObject.SetActive(true);
            Debug.Log("in soe, tl 0 is " + textLine[0].isActiveAndEnabled + " and t2 is " + textLine[1].isActiveAndEnabled);
            textLine[2].text = " ";

            textLine[2].text = "$$ F_" + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + "= "
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().forceValue.ToString(GLOBALS.format) + " * ("
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().uVec.x.ToString(GLOBALS.format) + "i + "
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().uVec.y.ToString(GLOBALS.format) + "j + "
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().uVec.z.ToString(GLOBALS.format) + "k"
           + ") ";


            // float a = (float)GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().forceValue;
            Debug.Log("just printed the known fvec: " + textLine[1].text);



            for (int i = 0; i <= 2; i++)
            {
                textLine[2].text += @"\par \bf F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = | "
                + "F_" + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " | * ( " //GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().forceValue + " * ("
                + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.x.ToString(GLOBALS.format) + "i + "
                + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.y.ToString(GLOBALS.format) + "j + "
                + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.z.ToString(GLOBALS.format) + "k"
                + ")";

                // Debug.Log("printed unknownvec " + i + " " + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.x);
            }
            textLine[2].text += "$$";
        
    }

    public void LinearCalc()
    {
            textLine[0].gameObject.SetActive(true);
            textLine[1].gameObject.SetActive(true);
            textLine[2].gameObject.SetActive(true);

            textLine[0].text = @"$$\Sigma F_x = ";
            textLine[1].text = @"$$\Sigma F_y = ";
            textLine[2].text = @"$$\Sigma F_z = ";

            for (int i = 0; i < 2; i++)
            {
                textLine[0].text += GLOBALS.unknownUVecs[i].x.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " + ";

                textLine[1].text += GLOBALS.unknownUVecs[i].y.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " + ";
                textLine[2].text += GLOBALS.unknownUVecs[i].z.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " + ";
            }
            textLine[0].text +=  GLOBALS.unknownUVecs[2].x.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[2].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = 0$$";
            textLine[1].text +=  GLOBALS.unknownUVecs[2].y.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[2].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = 0$$";
            textLine[2].text += GLOBALS.unknownUVecs[2].z.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[2].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = 0$$";
        

    }

    //Displays the validity of the force system entered by the user
    public void isValid()
    {
        Debug.Log("Validating");
        textLine[0].gameObject.SetActive(true);
        textLine[1].gameObject.SetActive(false);
        textLine[2].gameObject.SetActive(false);

        textLine[0].text = " ";

        //given force value
        textLine[0].text += @"$$\par F_" + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = " + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().forceValue;
        for(int i = 0; i <= 2; i++)
        {
            textLine[0].text += @"\par F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = " + bp3.GetComponent<VectorMathM3>().systemSolution[i].ToString(GLOBALS.format);
        }
        textLine[0].text += "$$"; 
    }
}
