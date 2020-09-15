using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RPCReceiver : MonoBehaviour
{
    private PhotonView PV;

    //VECTOR LABELS 
    public TextMeshPro label;   
    private TextMeshPro headLabelV1, tailLabelV1, headLabelV2, tailLabelV2;
    private TextMeshPro _componentLabelV2;

    //VECTOR NAMES   
    public BeamPlacementM1 beamPlacementM1_Ref;
    public BeamPlacementM2 beamPlacementM2_Ref;
    public VectorMath vectorMath_Ref;
    [SerializeField, Tooltip("The 3 vectors in the scene")]
    private List<VectorControl> vectors;
    public TextMeshPro nameLabelPrefab;
    private TextMeshPro nameLabel_B_Ref;

    // B cos
    public PhotonLineRenderer dotProjectionBody;
    public PhotonLineRenderer vector2;
    private TextMeshPro _nameAdotB_Bcos;

    //DOT PROJECTION ANGLE
    public TextMeshPro angle;

    //CLEAN UP
    [SerializeField]
    private myPlayer myPlayerRef;
    [SerializeField]
    private StorableObjectBin storableObjectBin_Ref;

    //ORIGIN LABELS
    private TextMeshPro xLabelRef;
    private TextMeshPro yLabelRef;
    private TextMeshPro zLabelRef;
    public TextMeshPro xLabel;
    public TextMeshPro yLabel;
    public TextMeshPro zLabel;
    [SerializeField]
    private OriginControl originControl_Ref;
    public GameObject originSphere;

    void Start() 
    {
        PV = GetComponent<PhotonView>();
    }
  
    void Update()
    {
        CheckForNames(); // in the future, create a bool for checking if names need to be checked for (to save some power)

        CheckForLabels();

        CheckIfCanHideLabels();

        CheckIfCanPlace_ResVec_CompLabel();

        // can hide v2 head & tail label
        if (vectorMath_Ref.bCanHideV2Labels)
        {
            SendtoHide_V2_headandtail();
        }

        // can spawn v2 comp label
        if(vectorMath_Ref.bCanPlaceV2ComponentLabel)
        {
            SendtoSpawn_V2_compLabel();
        }

        if (vectorMath_Ref.bVec2AnimationHappening) 
        {
            ResetHideB();
        }

        if (vectorMath_Ref.bCanDisplayAngle)
        {
            SendDispAngleRPC();
        }
    }

    //-----------------------VECTOR LABELS----------------------------//    

    public void CheckForLabels()
    {
       // if(beamPlacementM1_Ref.bCa)
        if (beamPlacementM2_Ref.bCanPlaceVec1Labels) // V1 head & tail 
        {
            
            PV.RPC("PlaceHeadandTailLabels", RpcTarget.OthersBuffered, 0, vectors[0]._headLabel.transform.position, vectors[0]._tailLabel.transform.position, vectors[0]._headLabel.text, vectors[0]._tailLabel.text, myPlayerRef.myPlayerActorNumber);
            beamPlacementM2_Ref.bCanPlaceVec1Labels = false; // reset
        }

        else if(beamPlacementM2_Ref.bCanPlaceV1ComponentLabel) // V1 component  
        {
            PV.RPC("PlaceComponentLabel", RpcTarget.OthersBuffered, 0, vectors[0]._componentLabel.transform.position + new Vector3(0f,0.015f,0f), vectors[0]._componentLabel.text, myPlayerRef.myPlayerActorNumber);
            beamPlacementM2_Ref.bCanPlaceV1ComponentLabel = false; // reset
        }

        else if(beamPlacementM2_Ref.bCanPlaceVec2Labels) // V2 head & tail
        {
            PV.RPC("PlaceHeadandTailLabels", RpcTarget.OthersBuffered, 1, vectors[1]._headLabel.transform.position, vectors[1]._tailLabel.transform.position, vectors[1]._headLabel.text, vectors[1]._tailLabel.text, myPlayerRef.myPlayerActorNumber);
            beamPlacementM2_Ref.bCanPlaceVec2Labels = false; // reset
        }

        else
        {
            return;
        }
    }

    // head & tail labels
    [PunRPC]
    public void PlaceHeadandTailLabels(int _i, Vector3 _headPos, Vector3 _tailPos, string _headValue, string _tailValue, string _actorNumber)
    {
        switch (_i)
        {
            case 0:
                // head
                headLabelV1= Instantiate(label, _headPos, Quaternion.identity);
                headLabelV1.text = _headValue;

                // tail 
                tailLabelV1 = Instantiate(label, _tailPos, Quaternion.identity);
                tailLabelV1.text = _tailValue;

                AddToBin(_actorNumber, headLabelV1); // add to bin
                AddToBin(_actorNumber, tailLabelV1); // add to bin
                break;

            case 1:
                // head
                headLabelV2 = Instantiate(label, _headPos, Quaternion.identity);
                headLabelV2.text = _headValue;

                // tail 
                tailLabelV2 = Instantiate(label, _tailPos, Quaternion.identity);
                tailLabelV2.text = _tailValue;

                AddToBin(_actorNumber, headLabelV2); // add to bin
                AddToBin(_actorNumber, tailLabelV2); // add to bin
                break;
        }
    }

    // component labels
    [PunRPC]
    public void PlaceComponentLabel(int _i, Vector3 _pos, string _value, string _actorNumber)
    {
        Debug.Log("Placing component label");
        switch (_i)
        {
            case 0:
                Debug.Log("placing comp label vec 1");
                // v1
                TextMeshPro _componentLabelV1 = Instantiate(label, _pos, Quaternion.identity); // place a bit higher
                _componentLabelV1.text = _value;

                AddToBin(_actorNumber, _componentLabelV1); // add to bin
                break;

            case 1:
                Debug.Log("Placing comp label vec 2");
                // v2
                _componentLabelV2 = Instantiate(label, _pos, Quaternion.identity); // place a bit higher
                _componentLabelV2.text = _value;

                AddToBin(_actorNumber, _componentLabelV2); // add to bin
                break;
        }
    }

    public void CheckIfCanHideLabels()
    {
        if(beamPlacementM2_Ref.bCanHideV1Labels) // hide v1 labels
        {
            PV.RPC("HideLabels", RpcTarget.OthersBuffered, 0);
            beamPlacementM2_Ref.bCanHideV1Labels = false; // reset
        }

        else
        {
            return;
        }
    }


    private void SendtoHide_V2_headandtail()
    {
            PV.RPC("HideLabels", RpcTarget.OthersBuffered, 1);
            vectorMath_Ref.bCanHideV2Labels = false; // reset        
    }

    private void SendtoSpawn_V2_compLabel()
    {    
            PV.RPC("PlaceComponentLabel", RpcTarget.OthersBuffered, 1, vectors[1]._componentLabel.transform.position + new Vector3(0f, 0.015f, 0f), vectors[1]._componentLabel.text, myPlayerRef.myPlayerActorNumber);
            vectorMath_Ref.bCanPlaceV2ComponentLabel = false; // reset
    }

    // hide stuff
    [PunRPC]
    public void HideLabels(int _i)
    {
        Debug.Log("Going to hide a label");
        switch(_i)
        {
            // v1
            case 0:
                Debug.Log("hid v1 head & tail");
                headLabelV1.GetComponent<MeshRenderer>().enabled = false;
                tailLabelV1.GetComponent<MeshRenderer>().enabled = false;                
                break;

            // v2
            case 1:
                Debug.Log("hid v2 head & tail");
                headLabelV2.GetComponent<MeshRenderer>().enabled = false;
                tailLabelV2.GetComponent<MeshRenderer>().enabled = false;                
                break;
        }
    }
         
    //-----------------------VECTOR NAMES----------------------------//
    
    public void CheckForNames()
    {
        // these permissions (bools) come from Beam Placements (before operation selections)

        if (beamPlacementM2_Ref.bCanPlaceVec1) // A
        {            
            PV.RPC("InitVectorNames", RpcTarget.OthersBuffered, 0, vectors[0]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            beamPlacementM2_Ref.bCanPlaceVec1 = false; // reset 
        }           


        else if (beamPlacementM2_Ref.bCanPlaceVec2) // B
        {            
            PV.RPC("InitVectorNames", RpcTarget.OthersBuffered, 1, vectors[1]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            beamPlacementM2_Ref.bCanPlaceVec2 = false;
        }           

        // these permissions (bools) come from VectorMath (after operation selection)

        else if (vectorMath_Ref.bCanPlaceAplusB) // A + B
        {            
            PV.RPC("InitVectorNames", RpcTarget.OthersBuffered, 2, vectors[2]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            vectorMath_Ref.bCanPlaceAplusB = false;
        }

        else if (vectorMath_Ref.bCanPlaceAminusB) // A - B
        {
            PV.RPC("InitVectorNames", RpcTarget.OthersBuffered, 3, vectors[2]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            vectorMath_Ref.bCanPlaceAminusB = false;              
        }

        else if (vectorMath_Ref.bCanPlaceAcrossB) // A x B
        {
            PV.RPC("InitVectorNames", RpcTarget.OthersBuffered, 4, vectors[2]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            vectorMath_Ref.bCanPlaceAcrossB = false;           
        }

        // the pos of B|cos| label appeared in the origin
        // we instead get the midpoint of the vector body from PhotonLineRenderer.cs
        else if (vectorMath_Ref.bcanPlaceAdotB_Bcos) // A • B: B cos
        {
            Debug.Log("can place A • B --- B cos");
            PV.RPC("InitDotProduct", RpcTarget.OthersBuffered, 0, (dotProjectionBody.OriginLocation + dotProjectionBody.EndpointLocation) /2f, vectorMath_Ref.dotNameLabel.text, myPlayerRef.myPlayerActorNumber); 
            vectorMath_Ref.bcanPlaceAdotB_Bcos = false;          
        }

        else if(vectorMath_Ref.bCanRespawnB)   //**********ALL GO HERE HERE???
        {
            // re-instantiate B @ new pos
            // this needs to be called immediatly after the vec 2 animation happens
            PV.RPC("InitVectorNames", RpcTarget.OthersBuffered, 1, (vector2.OriginLocation + vector2.EndpointLocation) /2f, myPlayerRef.myPlayerActorNumber);
            vectorMath_Ref.bCanRespawnB = false; // reset
        }

        else if (vectorMath_Ref.bCanPlaceAdotB_Acos) // A • B: A cos
        {
            Debug.Log("can place A • B --- A cos");
            PV.RPC("InitDotProduct", RpcTarget.OthersBuffered, 1, (dotProjectionBody.OriginLocation + dotProjectionBody.EndpointLocation) / 2f, vectorMath_Ref.dotNameLabel.text, myPlayerRef.myPlayerActorNumber);
            vectorMath_Ref.bCanPlaceAdotB_Acos = false;

            // don't need to re-instantiate B at a new pos in vectorMath (this has already been done once for dot product)
        }

        else
        {
            return;
        }
    }

    // spawns vec name labels
    // stores labels and sender's actionNumber in a bin for cleanup later
    [PunRPC]
    public void InitVectorNames(int _i, Vector3 _pos, string _actorNumber) 
    {
        Debug.Log("Executing Init Vector Names RPC");   

        switch (_i)
        {
            case 0:
                Debug.Log("Instantiating A");
                TextMeshPro _nameA = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameA.text = "A";
                                       
                AddToBin(_actorNumber, _nameA); // add to bin

                break;

            case 1:
                Debug.Log("Instantiating B");
                nameLabel_B_Ref = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                nameLabel_B_Ref.text = "B";
              
                AddToBin(_actorNumber, nameLabel_B_Ref); // add to bin

                break;

            case 2:
                Debug.Log("Instantiating A + B");
                TextMeshPro _nameAplusB = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameAplusB.text = "A + B";
              
                AddToBin(_actorNumber, _nameAplusB); // add to bin

                break;

            case 3:
                Debug.Log("Instantiating A - B");
                TextMeshPro _nameAminusB = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameAminusB.text = "A - B";
            
                AddToBin(_actorNumber, _nameAminusB); // add to bin

                break;

            case 4:
                Debug.Log("Instantiating A x B");
                TextMeshPro _nameAcrossB = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameAcrossB.text = "A x B";
              
                AddToBin(_actorNumber, _nameAcrossB); // add to bin

                break;

            // deafult case needed?
        }
    }

    // handle dot product name labels separately 
    [PunRPC]
    public void InitDotProduct(int _i, Vector3 _pos, string _value, string _actorNumber)
    {
        Debug.Log("Executing Init Dot labels Product RPC");
        switch (_i)
        {
            case 0:
                Debug.Log("Instantiating A • B: B cos");
                _nameAdotB_Bcos = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameAdotB_Bcos.text = _value; // customize to show angle
       
                AddToBin(_actorNumber, _nameAdotB_Bcos); // add to bin

                break;
           
            case 1:
                // hide B cos label 
                _nameAdotB_Bcos.GetComponent<MeshRenderer>().enabled = false; //up here so it happens faster???

                Debug.Log("Instantiating A • B: A cos");
                TextMeshPro _nameAdotB_Acos = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameAdotB_Acos.text = _value; // customize to show angle  
              
                AddToBin(_actorNumber, _nameAdotB_Acos); // add to bin

                break;

                // deafult case needed???
        }
    }

    private void ResetHideB()
    {        
        Debug.Log("Going to hide B");
        PV.RPC("V2AnimationHappening", RpcTarget.OthersBuffered);
        vectorMath_Ref.bVec2AnimationHappening = false; 
    }

    // hides B
    [PunRPC]
    public void V2AnimationHappening() 
    {
        //this should run only one time: app will blank out for a sec if it doesn't           
        Debug.Log("Hid B");
        nameLabel_B_Ref.GetComponent<MeshRenderer>().enabled = false; 
    }

    // sends actor number & label to a bin
    public void AddToBin(string _actorNumber, TextMeshPro _label)
    {
        StorableObject _object = new StorableObject();
        _object.actorNumber = _actorNumber; // get number        
        _object.label = _label; // get label
        storableObjectBin_Ref.AddStorableObject(_object); //add

    }

    //-----------------------DOT PROJECTION ANGLE----------------------------//
    private void SendDispAngleRPC()
    {
        Debug.Log("Going to display angle");
        PV.RPC("DisplayAngle", RpcTarget.OthersBuffered, vectorMath_Ref.angleLabel.transform.position, vectorMath_Ref.angleLabel.text, myPlayerRef.myPlayerActorNumber);
        vectorMath_Ref.bCanDisplayAngle = false;
    }

    // displays angle
    [PunRPC]
    public void DisplayAngle(Vector3 _pos, string _value, string _actorNumber)
    {
        TextMeshPro _angle = Instantiate(angle, _pos, Quaternion.identity);
        _angle.text = _value;

        AddToBin(_actorNumber, _angle); // add to bin
    }

    //-----------------------RESULTANT VECTOR COMPONENT LABELS----------------------------//

    // this is for op add, sub, & cross prod only!!! dot prod doesn't have a comp lable

    private void CheckIfCanPlace_ResVec_CompLabel()
    {
        if(vectors[2].bCanSpawnResVecCompLabel)
        {
            PV.RPC("SpawnResultantVec_ComponentLabel", RpcTarget.OthersBuffered, vectors[2]._componentLabel.transform.position + new Vector3(0f, 0.015f, 0f), vectors[2]._componentLabel.text, myPlayerRef.myPlayerActorNumber);
            vectors[2].bCanSpawnResVecCompLabel = false; // reset
        }
    }

    [PunRPC]
    public void SpawnResultantVec_ComponentLabel(Vector3 _pos, string _val, string _actorNumber)
    {
        TextMeshPro _label = Instantiate(label, _pos, Quaternion.identity);
        _label.text = _val;

        AddToBin(_actorNumber, _label); // add to bin
    }

    //-----------------------ORIGIN----------------------------//
    public void SetUp_OriginLabel_RPC()
    {
        PV.RPC("SpawnOriginLabelsRPC", RpcTarget.OthersBuffered, originControl_Ref.xAxisText.transform.position, originControl_Ref.yAxisText.transform.position, originControl_Ref.zAxisText.transform.position, myPlayerRef.myPlayerActorNumber);
    }

    [PunRPC]
    public void SpawnOriginLabelsRPC(Vector3 _xPos, Vector3 _yPos, Vector3 _zPos, string _actorNumber)
    {
        xLabelRef = Instantiate(xLabel, _xPos, Quaternion.identity);
        yLabelRef = Instantiate(yLabel, _yPos, Quaternion.identity);
        zLabelRef = Instantiate(zLabel, _zPos, Quaternion.identity);

        AddToBin(_actorNumber, xLabelRef); // add to bin
        AddToBin(_actorNumber, yLabelRef); // add to bin
        AddToBin(_actorNumber, zLabelRef); // add to bin
    }

    public void SetUp_UpdateOriginLabel_RPC()
    {
        if (PV != null) // was giving me console null reference b4 building the project?
        {
            PV.RPC("UpdateOriginLabels_RPC", RpcTarget.OthersBuffered, originControl_Ref.xAxisText.transform.position, originControl_Ref.yAxisText.transform.position, originControl_Ref.zAxisText.transform.position);
        }
    }

    [PunRPC]
    public void UpdateOriginLabels_RPC(Vector3 _xPos, Vector3 _yPos, Vector3 _zPos)
    {
        xLabelRef.transform.position = _xPos;
        yLabelRef.transform.position = _yPos;
        zLabelRef.transform.position = _zPos;
    }

    // I disabled the mesh renderer on the scene origin sphere
    public void SpawnOriginSphere(Vector3 _spherePos)
    {
        Debug.Log("PUN: spawning origin sphere");
        GameObject _sphere = PhotonNetwork.Instantiate(originSphere.name, _spherePos, Quaternion.identity); 
    }
}
