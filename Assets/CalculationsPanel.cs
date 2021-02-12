using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalculationsPanel : MonoBehaviour
{
    // private const string V = @"\par";

    // public Text header;
    public List<TEXDraw> textLine;
    public Camera cam;

    private CalculationsPanel calcPanel; //private variable for the PUN instance of the CalcPanel script
    [SerializeField]
    private myPlayer myPlayerRef;
    [SerializeField]
    private StorableObjectBin storableObjectBin_Ref;

    // Start is called before the first frame update
    void Start()
    {
       gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
            gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position - cam.transform.position);
        if (PhotonNetwork.InRoom && gameObject.activeSelf)
        {
            Debug.Log("if in room and act");
            calcPanel.GetComponentInParent<GameObject>().gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position - cam.transform.position);
        }
    }

    public void StartCalculationsSequence()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = GLOBALS.pocPos + new Vector3(0.5f, 0, 0.5f);
        if(PhotonNetwork.InRoom)
        {
            GameObject calcPanelObject = PhotonNetwork.Instantiate("CalcPanel", gameObject.transform.position, Quaternion.identity);
            calcPanel = calcPanelObject.GetComponent<CalculationsPanel>();
            Debug.Log("is calcPanelObj active: " + calcPanelObject.gameObject.activeSelf + " and is calcPanel active: " + calcPanel.name  );
            // cam = GameObject.Find("Main Camera").GetComponent<Camera>();
          //  AddToBin(myPlayerRef.myPlayerActorNumber, calcPanel);
        }
    }

    public void CleanCanvas()
    {
        textLine[0].text = "Calc";
        textLine[1].text = "Panel";
    }

    //make smaller font
    public void ComponentCalcs()
    {
        if (SceneManager.GetActiveScene().buildIndex == 12)
        {
            textLine[0].text = "$$" +
       "r_" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + " = (" +
       GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relHeadPos.x.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relTailPos.x.ToString(GLOBALS.format) + @")i + \par(" +
       GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relHeadPos.y.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relTailPos.y.ToString(GLOBALS.format) + @")j + \par(" +
       GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relHeadPos.z.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().relTailPos.z.ToString(GLOBALS.format) + @")k $$";
        }

        else {
                calcPanel.textLine[0].text = "$$" +
                    "r_" + GLOBALS.SelectedVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = (" +
                    GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relHeadPos.x.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relTailPos.x.ToString(GLOBALS.format) + @")i + \par(" +
                    GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relHeadPos.y.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relTailPos.y.ToString(GLOBALS.format) + @")j + \par(" +
                    GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relHeadPos.z.ToString(GLOBALS.format) + " - " + GLOBALS.SelectedVec.GetComponent<VectorControlM3>().relTailPos.z.ToString(GLOBALS.format) + @")k $$";

        }
   
    }



    public void MagCalcs()
    {
           if (SceneManager.GetActiveScene().buildIndex == 12)
           {
               Vector3 relVec = GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._head.position - GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>()._tail.position;
               textLine[1].text = @"$$\par \par|" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + "| = " +
                  @"\sqrt[2]{(" + relVec.x.ToString(GLOBALS.format) + ")^2 + (" + relVec.y.ToString(GLOBALS.format) + ")^2 + (" + relVec.z.ToString(GLOBALS.format) + ")^2 } " +
                  @"\par = " + relVec.magnitude.ToString(GLOBALS.format) + "$$";
           }
           else
           {
               Vector3 relVec = GLOBALS.SelectedVec.GetComponent<VectorControlM3>()._head.position - GLOBALS.SelectedVec.GetComponent<VectorControlM3>()._tail.position;
               calcPanel.textLine[1].text = @"$$\par \par|" + GLOBALS.SelectedVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + "| = " +
                  @"\sqrt[2]{(" + relVec.x.ToString(GLOBALS.format) + ")^2 + (" + relVec.y.ToString(GLOBALS.format) + ")^2 + (" + relVec.z.ToString(GLOBALS.format) + ")^2 } " +
                  @"\par = " + relVec.magnitude.ToString(GLOBALS.format) + "$$";
           }
    }

    public void SystemOfEqs()
    {
        if (SceneManager.GetActiveScene().buildIndex == 12)
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



            for (int i = 0; i <= 2; i++)
            {
                textLine[2].text += @"\par \bf F_" + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " = | "
                + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " | * ( " //GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().forceValue + " * ("
                + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().uVec.x.ToString(GLOBALS.format) + "i + "
                + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().uVec.y.ToString(GLOBALS.format) + "j + "
                + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().uVec.z.ToString(GLOBALS.format) + "k"
                + ")";

                // Debug.Log("printed unknownvec " + i + " " + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().uVec.x);
            }
            textLine[2].text += "$$";
        }
        else
        {
            calcPanel.textLine[0].gameObject.SetActive(false);
            calcPanel.textLine[1].gameObject.SetActive(false);
            calcPanel.textLine[2].gameObject.SetActive(true);
            Debug.Log("in soe, tl 0 is " + textLine[0].isActiveAndEnabled + " and t2 is " + textLine[1].isActiveAndEnabled);
            calcPanel.textLine[2].text = " ";

            calcPanel.textLine[2].text = "$$ F_" + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + "= "
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().forceValue.ToString(GLOBALS.format) + " * ("
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().uVec.x.ToString(GLOBALS.format) + "i + "
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().uVec.y.ToString(GLOBALS.format) + "j + "
           + GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().uVec.z.ToString(GLOBALS.format) + "k"
           + ") ";


            // float a = (float)GLOBALS.GivenForceVec.GetComponent<VectorPropertiesM3>().forceValue;
            Debug.Log("just printed the known fvec: " + textLine[1].text);



            for (int i = 0; i <= 2; i++)
            {
                calcPanel.textLine[2].text += @"\par \bf F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = | "
                + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " | * ( " //GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().forceValue + " * ("
                + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.x.ToString(GLOBALS.format) + "i + "
                + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.y.ToString(GLOBALS.format) + "j + "
                + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.z.ToString(GLOBALS.format) + "k"
                + ")";

                // Debug.Log("printed unknownvec " + i + " " + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().uVec.x);
            }
            calcPanel.textLine[2].text += "$$";
        }
    }

    public void LinearCalc()
    {


        if (SceneManager.GetActiveScene().buildIndex == 12)
        {
            textLine[0].gameObject.SetActive(true);
            textLine[1].gameObject.SetActive(true);
            textLine[2].gameObject.SetActive(true);

            textLine[0].text = @"$$\Sigma F_x = ";
            textLine[1].text = @"$$\Sigma F_y = ";
            textLine[2].text = @"$$\Sigma F_z = ";

            for (int i = 0; i < 2; i++)
            {
                textLine[0].text += GLOBALS.unknownUVecs[i].x.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " + ";

                textLine[1].text += GLOBALS.unknownUVecs[i].y.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " + ";
                textLine[2].text += GLOBALS.unknownUVecs[i].z.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " + ";
            }
            textLine[0].text += GLOBALS.unknownUVecs[2].x.ToString(GLOBALS.format) + GLOBALS.unknownVecs[2].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " = 0$$";
            textLine[1].text += GLOBALS.unknownUVecs[2].y.ToString(GLOBALS.format) + GLOBALS.unknownVecs[2].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " = 0$$";
            textLine[2].text += GLOBALS.unknownUVecs[2].z.ToString(GLOBALS.format) + GLOBALS.unknownVecs[2].GetComponent<VectorProperties>().gameObject.name.Substring(12) + " = 0$$";
        }
        else
        {
            calcPanel.textLine[0].gameObject.SetActive(true);
            calcPanel.textLine[1].gameObject.SetActive(true);
            calcPanel.textLine[2].gameObject.SetActive(true);

            calcPanel.textLine[0].text = @"$$\Sigma F_x = ";
            calcPanel.textLine[1].text = @"$$\Sigma F_y = ";
            calcPanel.textLine[2].text = @"$$\Sigma F_z = ";

            for (int i = 0; i < 2; i++)
            {
                calcPanel.textLine[0].text += GLOBALS.unknownUVecs[i].x.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " + ";

                calcPanel.textLine[1].text += GLOBALS.unknownUVecs[i].y.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " + ";
                calcPanel.textLine[2].text += GLOBALS.unknownUVecs[i].z.ToString(GLOBALS.format) + "F_" + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " + ";
            }
            calcPanel.textLine[0].text += GLOBALS.unknownUVecs[2].x.ToString(GLOBALS.format) + GLOBALS.unknownVecs[2].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = 0$$";
            calcPanel.textLine[1].text += GLOBALS.unknownUVecs[2].y.ToString(GLOBALS.format) + GLOBALS.unknownVecs[2].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = 0$$";
            calcPanel.textLine[2].text += GLOBALS.unknownUVecs[2].z.ToString(GLOBALS.format) + GLOBALS.unknownVecs[2].GetComponent<VectorPropertiesM3>().gameObject.name.Substring(12) + " = 0$$";
        }

    }

    public void isValid()
    {
        Debug.Log("Validating");
        textLine[0].gameObject.SetActive(true);
        textLine[1].gameObject.SetActive(false);
        textLine[2].gameObject.SetActive(false);

        if (GLOBALS.isValidSystem)
        {
            textLine[0].text = @"\par Your System Is Correct";
        }
        else
        {
            textLine[0].text = @"\par Your System Is Incorrect";
        }
    }


}
