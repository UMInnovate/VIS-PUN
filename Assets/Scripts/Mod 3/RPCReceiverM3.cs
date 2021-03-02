﻿using System.Collections;
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

    private TextMeshPro vecLabelV1, vecLabelV2, vecLabelV3, vecLabelV4;
    private TextMeshPro nameA, nameB, nameC, nameD;
    private TextMeshPro pocLabel;

    [SerializeField] TextMeshPro nameLabelPrefab;
    [SerializeField] TextMeshPro vecLabelPrefab;
    [SerializeField] TextMeshPro pocLabelPrefab;


    //VECTOR NAMES
    public BeamPlacementM3 beamPlacement;
    public VectorMathM3 vectorMath;
    public List<VectorControlM3> vectors;
    public KeypadPanel keypadPanel;

    [SerializeField]
    private myPlayer myPlayerRef;
    [SerializeField]
    private StorableObjectBin storableObjectBin_Ref;

    private PhotonView PV;

    private Vector3 offsetConst = new Vector3(0, 0.04f, 0);
        //CLEAN UP
    //ORIGIN LABELS


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
        Console.WriteLine("Can Place V1: " + vectorMath.bCanPlaceVec1 + " V2 " + vectorMath.bCanPlaceVec2 + " V3 " + vectorMath.bCanPlaceVec3 + " V4 " + vectorMath.bCanPlaceVec4);

        if(beamPlacement.bCanPlacePOC)
        {
            PV.RPC("PlacePOCLabel", RpcTarget.AllBuffered, GLOBALS.pocPos + offsetConst, (GLOBALS.pocPos - beamPlacement._origin.transform.position).ToString(GLOBALS.format), myPlayerRef.myPlayerActorNumber);
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
            Console.WriteLine("after place vector labels v2");
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
    public void PlacePOCLabel(Vector3 placementPos, string _value, string _actorNumber) 
    {
        pocLabel = Instantiate(pocLabelPrefab, placementPos, Quaternion.identity);
        pocLabel.text = _value;
        Console.WriteLine("poc val: " + _value);
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


    [PunRPC]
    public void PlaceVectorLabels(int _v, Vector3 ptPos, Vector3 localPos, string _actorNumber)
    {
        Console.WriteLine("in vectorlabels with index " + _v);
        switch (_v)
        {
            case 0:
                Console.WriteLine("case 0 is being called in placevectorlabels");
                vecLabelV1 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);

                if(beamPlacement.bIsViewer) //if you are only viewing the system, rely on the value fed by rpc
                    vecLabelV1.text = (localPos).ToString(GLOBALS.format); 
                else  //if you are controlling the experience, call your local var
                    vecLabelV1.text = (vectorMath.localVec1Pos).ToString(GLOBALS.format);

                AddToBin(_actorNumber, vecLabelV1); // add to bin
                vectorMath.bCanPlaceVec1 = false;
                break;

            case 1:
                Console.WriteLine("case 1 is being called in placevectorlabels");
                vecLabelV2 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);

                if (beamPlacement.bIsViewer)
                    vecLabelV2.text = (localPos).ToString(GLOBALS.format); 
                else
                    vecLabelV2.text = (vectorMath.localVec2Pos).ToString(GLOBALS.format);

                AddToBin(_actorNumber, vecLabelV2); // add to bin
                vectorMath.bCanPlaceVec2 = false;
                break;
            case 2:
                Console.WriteLine("case 2 is being called in placevectorlabels");
                vecLabelV3 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);
                
                if (beamPlacement.bIsViewer)
                    vecLabelV3.text = (localPos).ToString(GLOBALS.format); 
                else
                    vecLabelV3.text = (vectorMath.localVec3Pos).ToString(GLOBALS.format);

                AddToBin(_actorNumber, vecLabelV3); // add to bin
                vectorMath.bCanPlaceVec3 = false;
                break;
            case 3:
                Console.WriteLine("case 3 is being called in placevectorlabels");
                vecLabelV4 = Instantiate(pocLabelPrefab, ptPos + offsetConst, Quaternion.identity);
                
                if (beamPlacement.bIsViewer)
                    vecLabelV4.text = (localPos).ToString(GLOBALS.format); 
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
                    TextMeshPro nameA = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                    nameA.text = "A";

                    AddToBin(_actorNumber, nameA); // add to bin

                beamPlacement.bCanPlaceVec1Labels = false; 
                break;
            case 1:
                Console.WriteLine("Instantiating b");
                TextMeshPro nameB = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                nameB.text = "B";

                AddToBin(_actorNumber, nameB); // add to bin

                beamPlacement.bCanPlaceVec2Labels = false;
                break;
            case 2:
                Console.WriteLine("Instantiating C");
                TextMeshPro nameC = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                nameC.text = "C";

                AddToBin(_actorNumber, nameC); // add to bin

                beamPlacement.bCanPlaceVec3Labels = false;
                break;
            case 3:
                Console.WriteLine("Instantiating D");
                TextMeshPro nameD = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                nameD.text = "D";

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
