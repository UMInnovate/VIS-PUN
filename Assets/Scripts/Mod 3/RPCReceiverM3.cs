using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

public class RPCReceiverM3 : MonoBehaviour
{
    //VECTOR LABELS
    //public TextMeshPro[] nameLabel;
    //  public TextMeshPro[] headLabels;
    //public TextMeshPro[] tailLabels;

    //public Text text; //Text in Headpose canvas

    private TextMeshPro vecLabelV1, vecLabelV2, vecLabelV3, vecLabelV4;
    private TextMeshPro nameA, nameB, nameC, nameD;
    private TextMeshPro pocLabel;
    [SerializeField] private GameObject calcPanel;

    private List<Vector3> vecPos;

  //  [SerializeField] GameObject  calcPanelPrefab;
    [SerializeField] TextMeshPro nameLabelPrefab;
    [SerializeField] TextMeshPro vecLabelPrefab;
    [SerializeField] TextMeshPro pocLabelPrefab;


    //VECTOR NAMES
    public BeamPlacementM3 beamPlacement;
    public VectorMathM3 vectorMath;
    public List<VectorControlM3> vectors;
    public KeypadPanel keypadPanel;
    public bool setVal = true; 
    [SerializeField]
    private myPlayer myPlayerRef;
    [SerializeField]
    private StorableObjectBin storableObjectBin_Ref;

    private PhotonView PV;

    private Vector3 offsetConst = new Vector3(0, 0.04f, 0);
    public List<Vector3> vector3s;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForOrigin();
        CheckNames();
        CheckForPrefab();
        CheckForLabels();

   //     Console.WriteLine("Global force val " + GLOBALS.forceVal + " and set val " + setVal);
        if(GLOBALS.forceVal != -1 && setVal)
        {
          //  Console.WriteLine("")
            PV.RPC("SetForceVal", RpcTarget.OthersBuffered, GLOBALS.forceVal, GLOBALS.chosenVecInt);
            setVal = false;
            Console.WriteLine("setval is set to " + setVal);
        }

        if(beamPlacement.bCalcPanel)
        {
            Console.WriteLine("calcpanel has been enabled by the host");
            PV.RPC("StartCalcSequence", RpcTarget.AllBuffered);
            beamPlacement.bCalcPanel = false;
        }

        if(beamPlacement.bCalc1)
        {
            Console.WriteLine("bcalc1 enabled");
            PV.RPC("MagCompSequence", RpcTarget.OthersBuffered);
            beamPlacement.bCalc1 = false;
        }

        if(beamPlacement.forceSystemText)
        {
            PV.RPC("SetText", RpcTarget.OthersBuffered, GLOBALS.isValidSystem);
            beamPlacement.forceSystemText = false; 
        }

        if(beamPlacement.bCalcSoE)
        {
            PV.RPC("SystemOfEquations", RpcTarget.OthersBuffered);
            beamPlacement.bCalcSoE = false;
        }

        if(beamPlacement.bLinearSys)
        {
            PV.RPC("LinearSys", RpcTarget.OthersBuffered);
            beamPlacement.bLinearSys = false;
        }
    }

    [PunRPC]
    public void SetText(bool valid)
    {
        if (valid) beamPlacement.GetComponent<GiveInstructions>().text.text = "Your system is valid";
        else beamPlacement.GetComponent<GiveInstructions>().text.text = "Your system is valid";

        beamPlacement.forceSystemText = false;
    }

    public void CheckForOrigin()
    {
        //if someone has placed the origin
        if(beamPlacement.bOriginPlaced) { PV.RPC("SetViewerPriviledges", RpcTarget.OthersBuffered);  }
    }

    public void CheckForPrefab()
    {
        if(vectorMath.bCanPlaceVec1)
        {
            PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 0, vectorMath.vec1Pos);
        }
        else if (vectorMath.bCanPlaceVec2)
        {
            PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 1, vectorMath.vec2Pos);
        }
        else if (vectorMath.bCanPlaceVec3)
        {
            PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 2, vectorMath.vec3Pos);
        }
        else if (vectorMath.bCanPlaceVec4)
        {
            PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 3, vectorMath.vec4Pos);
        }
        else
        {
            return;
        }
    }

    public void CheckForLabels()
    {
       // Console.WriteLine("Can Place V1: " + vectorMath.bCanPlaceVec1 + " V2 " + vectorMath.bCanPlaceVec2 + " V3 " + vectorMath.bCanPlaceVec3 + " V4 " + vectorMath.bCanPlaceVec4);

        if(beamPlacement.bCanPlacePOC)
        {
            PV.RPC("PlacePOCLabel", RpcTarget.AllBuffered, GLOBALS.pocPos + offsetConst, (GLOBALS.pocPos - beamPlacement._origin.transform.position), myPlayerRef.myPlayerActorNumber);
            beamPlacement.bCanPlacePOC = false; 
        }

        if (vectorMath.bCanPlaceVec1) // V1 head & tail 
        {
           // PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 0, vectorMath.vec1Pos);
            PV.RPC("PlaceVectorLabels", RpcTarget.AllBuffered, 0, vectorMath.vec1Pos, vectorMath.localVec1Pos, myPlayerRef.myPlayerActorNumber); 
            vectorMath.bCanPlaceVec1 = false; // reset
        }

        else if (vectorMath.bCanPlaceVec2) // V2 head & tail 
        {
         //   PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 1, vectorMath.vec2Pos);
            Console.WriteLine("place vector labels v2");
            PV.RPC("PlaceVectorLabels", RpcTarget.AllBuffered, 1, vectorMath.vec2Pos, vectorMath.localVec2Pos, myPlayerRef.myPlayerActorNumber);
          //  Console.WriteLine("after place vector labels v2");
            vectorMath.bCanPlaceVec2 = false; // reset
        }

        else if (vectorMath.bCanPlaceVec3) // V3 head & tail 
        {
           // PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 2, vectorMath.vec3Pos);
            Console.WriteLine("place vector labels v3");
            PV.RPC("PlaceVectorLabels", RpcTarget.AllBuffered, 2, vectorMath.vec3Pos, vectorMath.localVec3Pos, myPlayerRef.myPlayerActorNumber);
            Console.WriteLine("after place vector labels v3");
            vectorMath.bCanPlaceVec3 = false;
        }
        else if (vectorMath.bCanPlaceVec4) // V4 head & tail 
        {
         //   PV.RPC("SpawnHeadAndTail", RpcTarget.OthersBuffered, 3, vectorMath.vec4Pos);
            PV.RPC("PlaceVectorLabels", RpcTarget.AllBuffered, 3, vectorMath.vec4Pos, vectorMath.localVec4Pos, myPlayerRef.myPlayerActorNumber);
            vectorMath.bCanPlaceVec4 = false; // reset
        }

        else
        {
            return;
        }
    }

    public void CheckNames()
    {
        
        if(beamPlacement.bCanPlaceVec1Labels)
        {
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 0, vectors[0]._nameLabel.transform.position, 0, myPlayerRef.myPlayerActorNumber);
            beamPlacement.bCanPlaceVec1Labels = false;
        }
        else if (beamPlacement.bCanPlaceVec2Labels)
        {
           
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 1, vectors[1]._nameLabel.transform.position, 0, myPlayerRef.myPlayerActorNumber);
            
            beamPlacement.bCanPlaceVec2Labels = false;
        }
        else if (beamPlacement.bCanPlaceVec3Labels)
        {
           
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 2, vectors[2]._nameLabel.transform.position, 0, myPlayerRef.myPlayerActorNumber);
           
            beamPlacement.bCanPlaceVec3Labels = false;
        }
        else if (beamPlacement.bCanPlaceVec4Labels)
        {
            
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 3, vectors[3]._nameLabel.transform.position, 0, myPlayerRef.myPlayerActorNumber);
            
            beamPlacement.bCanPlaceVec4Labels = false;
        }
        else
        {
            return;
        }
    }


    [PunRPC]
    public void StartCalcSequence()
    {
            calcPanel.GetComponent<CalculationsPanelM3>().StartCalculationsSequence();
            calcPanel.SetActive(true);
            Console.WriteLine("calcpanel has been instantiated at " + calcPanel.transform.position);
            beamPlacement.bCalcPanel = false; 
    }

    [PunRPC]
    public void MagCompSequence()
    {
        calcPanel.GetComponent<CalculationsPanelM3>().MagCalcs();
        Console.WriteLine("textline at 0: " + calcPanel.GetComponent<CalculationsPanelM3>().textLine[0].text);
        calcPanel.GetComponent<CalculationsPanelM3>().ComponentCalcs();
        beamPlacement.bCalc1 = false;
    }

    [PunRPC]
    public void SystemOfEquations()
    {
        //   beamPlacement.GetComponent<VectorMathM3>().ValidateForceSystem();
        Console.WriteLine("excuting rpc for soe");
        calcPanel.GetComponent<CalculationsPanelM3>().SystemOfEqs();

        beamPlacement.bCalcSoE = false; 
    }

    [PunRPC]
    public void LinearSys()
    {
        calcPanel.GetComponent<CalculationsPanelM3>().LinearCalc();
        Console.WriteLine("executing rpc for linearsys");
        beamPlacement.bLinearSys = false;
    }


    [PunRPC] 
    public void SetForceVal(int val, int v)
    {
        GLOBALS.chosenVecInt = v;
        GLOBALS.forceVal = val;
       // keypadPanel.forceVal = val;
        if(val != -1 && v >= 0)
        {
                vectors[v].GetComponent<VectorPropertiesM3>().SetForceVal(val);
                GLOBALS.SelectedVec = vectors[v].gameObject;
                GLOBALS.GivenForceVec = vectors[v].gameObject;
                UpdateNameLabel();
              //  Console.WriteLine("updated name label w " + val);
                vectors[v].GetComponent<VectorPropertiesM3>().isGivenForceValue = true;
                vectors[v].GetComponent<VectorPropertiesM3>().BuildForceVector();
              //  Console.WriteLine("set the force val as " + val);
        }
      //  setVal = false; 
    }

    [PunRPC]
    public void PlacePOCLabel(Vector3 placementPos, Vector3 _value, string _actorNumber) 
    {
        GLOBALS.pocPos = placementPos;
        pocLabel = Instantiate(pocLabelPrefab, placementPos, Quaternion.identity);
        pocLabel.text = _value.ToString(GLOBALS.format);
        beamPlacement.adjPOCPos = _value;
       // Console.WriteLine("poc val: " + _value);
        AddToBin(_actorNumber, pocLabel);
    }

    [PunRPC]
    public void SetViewerPriviledges() //possible issue if two people press trigger to place ori at the same time
    {
        beamPlacement.bIsViewer = true; 
    }

    [PunRPC] 
    public void SpawnHeadAndTail(int vec, Vector3 pos)
    {
        //vectors[vec].SpawnHeadAndTail(); 
        vectorMath.PlaceVector3Point(vec, pos);
      //  Debug.Log("spawning Head and tail of vector " + vec + " of sbe " + beamPlacement.storedBeamEnd);
       // Console.WriteLine("Point Value Vector " + vec + " has value " + pos);
    }

    public void SetBuild(int v)
    {
        vectors[v].GetComponent<VectorPropertiesM3>().isGivenForceValue = false;
        vectors[v].GetComponent<VectorPropertiesM3>().BuildForceVector();
    }

    public void UpdateNameLabel()
    {
        Console.WriteLine("index of chosen: " + GLOBALS.chosenVecInt);
        switch(GLOBALS.chosenVecInt)
        {
            case 0:
             //   nameA.GetComponent<MeshRenderer>().enabled = false; 
               // nameUpdated = Instantiate(pocLabelPrefab, vector3s[0], Quaternion.identity);
                nameA.text = "A = " + GLOBALS.forceVal + " N";
                AddToBin(myPlayerRef.myPlayerActorNumber, nameA);
                SetBuild(1); SetBuild(2); SetBuild(3);
                break;
            case 1:
              //  Destroy(nameB);
              //  nameB = Instantiate(pocLabelPrefab, vector3s[1], Quaternion.identity);
                nameB.text = "B = " + GLOBALS.forceVal + " N";
                AddToBin(myPlayerRef.myPlayerActorNumber, nameB);
                SetBuild(0); SetBuild(2); SetBuild(3);
                break;
            case 2:
               // Destroy(nameC);
              //  nameC = Instantiate(pocLabelPrefab, vector3s[2], Quaternion.identity);
                nameC.text = "C = " + GLOBALS.forceVal + " N";
                AddToBin(myPlayerRef.myPlayerActorNumber, nameC);
                SetBuild(1); SetBuild(0); SetBuild(3);
                break;
            case 3:
              //  Destroy(nameD);
              //  nameD = Instantiate(pocLabelPrefab, vector3s[3], Quaternion.identity);
                nameD.text = "D = " + GLOBALS.forceVal + " N";
                AddToBin(myPlayerRef.myPlayerActorNumber, nameD);
                SetBuild(1); SetBuild(2); SetBuild(0);
                break;
            default:
                Console.WriteLine("default case");
                break;
        }
        Console.WriteLine("value of vecLabel: " + vecLabelV1.text + " " + vecLabelV2.text + " " + vecLabelV3.text + " " + vecLabelV4.text + " ");
    }


    [PunRPC]
    public void PlaceVectorLabels(int _v, Vector3 ptPos, Vector3 localPos, string _actorNumber)
    {
        Console.WriteLine("in vectorlabels with index " + _v);
        switch (_v)
        {
            case 0:
                Console.WriteLine("case 0 is being called in placevectorlabels");
                vecLabelV1 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);

                if (beamPlacement.bIsViewer) //if you are only viewing the system, rely on the value fed by rpc
                {
                    vecLabelV1.text = (localPos).ToString(GLOBALS.format);
                    vectors[_v].photonPos = localPos;
                }
                else  //if you are controlling the experience, call your local var
                    vecLabelV1.text = (vectorMath.localVec1Pos).ToString(GLOBALS.format);

                AddToBin(_actorNumber, vecLabelV1); // add to bin
                vectorMath.bCanPlaceVec1 = false;
                break;

            case 1:
                Console.WriteLine("case 1 is being called in placevectorlabels");
                vecLabelV2 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);

                if (beamPlacement.bIsViewer)
                {
                    vecLabelV2.text = (localPos).ToString(GLOBALS.format);
                    vectors[_v].photonPos = localPos;
                }
                else
                    vecLabelV2.text = (vectorMath.localVec2Pos).ToString(GLOBALS.format);

                AddToBin(_actorNumber, vecLabelV2); // add to bin
                vectorMath.bCanPlaceVec2 = false;
                break;
            case 2:
                Console.WriteLine("case 2 is being called in placevectorlabels");
                vecLabelV3 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);

                if (beamPlacement.bIsViewer)
                {
                    vecLabelV3.text = (localPos).ToString(GLOBALS.format);
                    vectors[_v].photonPos = localPos;
                }
                else
                    vecLabelV3.text = (vectorMath.localVec3Pos).ToString(GLOBALS.format);

                AddToBin(_actorNumber, vecLabelV3); // add to bin
                vectorMath.bCanPlaceVec3 = false;
                break;
            case 3:
                Console.WriteLine("case 3 is being called in placevectorlabels");
                vecLabelV4 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);

                if (beamPlacement.bIsViewer)
                {
                    vecLabelV4.text = (localPos).ToString(GLOBALS.format);
                    vectors[_v].photonPos = localPos; //Save it into a new var
                }
                else
                    vecLabelV4.text = (vectorMath.localVec4Pos).ToString(GLOBALS.format);

                AddToBin(_actorNumber, vecLabelV4); // add to bin
                vectorMath.bCanPlaceVec4 = false;
                break;
        }
    }

    [PunRPC]
    public void InitVectorName(int _i, Vector3 _pos, int fval, string _actorNumber)
    {
        Debug.Log("Executing Init Vector Names RPC");

        switch (_i)
        {
            case 0:
              Console.WriteLine("Instantiating A");
              nameA = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
              nameA.text = "A";
             //   vector3s[0] = _pos;
               AddToBin(_actorNumber, nameA); // add to bin

                beamPlacement.bCanPlaceVec1Labels = false; 
                break;
            case 1:
                Console.WriteLine("Instantiating b");
                nameB = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                nameB.text = "B";
            //    vector3s[1] = _pos;
                AddToBin(_actorNumber, nameB); // add to bin

                beamPlacement.bCanPlaceVec2Labels = false;
                break;
            case 2:
                Console.WriteLine("Instantiating C");
                nameC = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                nameC.text = "C";
            //    vector3s[2] = _pos;
                AddToBin(_actorNumber, nameC); // add to bin

                beamPlacement.bCanPlaceVec3Labels = false;
                break;
            case 3:
                Console.WriteLine("Instantiating D");
                nameD = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                nameD.text = "D";
             //   vector3s[3] = _pos;
                AddToBin(_actorNumber, nameD); // add to bin

                beamPlacement.bCanPlaceVec4Labels = false;
                break;
        }
    }

    // sends actor number & label to a bin
    public void AddToBin(string _actorNumber, TextMeshPro _label)
    {
        StorableObject _object = new StorableObject();
        _object.actorNumber = _actorNumber; // get number        
        _object.label = _label; // get label
        storableObjectBin_Ref.AddStorableObject(_object); //add
    }



}
