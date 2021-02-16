using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RPCReceiverM3 : MonoBehaviour
{
    //VECTOR LABELS
    public TextMeshPro[] nameLabel;
    public TextMeshPro[] headLabels;
    public TextMeshPro[] tailLabels;

    private TextMeshPro headLabelV1, tailLabelV1, headLabelV2, tailLabelV2, 
        
        headLabelV3, tailLabelV3, headLabelV4, tailLabelV4;

    [SerializeField] TextMeshPro nameLabelPrefab;
    [SerializeField] TextMeshPro headLabelPrefab;
    [SerializeField] TextMeshPro tailLabelPrefab;


    //VECTOR NAMES
    public BeamPlacementM3 beamPlacement;
    public VectorMathM3 vectorMath;
    public List<VectorControlM3> vectors;

    [SerializeField]
    private myPlayer myPlayerRef;
    [SerializeField]
    private StorableObjectBin storableObjectBin_Ref;

    private PhotonView PV; 
        //CLEAN UP
    //ORIGIN LABELS


    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckNames();
        CheckForLabels();
    }

    public void CheckForLabels()
    {

        if (vectorMath.bCanPlaceVec1Head || vectorMath.bCanPlaceVec1Tail) // V1 head & tail 
        {

            PV.RPC("PlaceHeadandTailLabels", RpcTarget.OthersBuffered, 0, vectorMath.bCanPlaceVec1Tail, vectorMath.bCanPlaceVec1Head, vectors[0]._headLabel.transform.position, vectors[0]._tailLabel.transform.position, vectors[0]._headLabel.text, vectors[0]._tailLabel.text, myPlayerRef.myPlayerActorNumber);
            vectorMath.bCanPlaceVec1Head = false; vectorMath.bCanPlaceVec1Tail = false; // reset
        }
        else if (vectorMath.bCanPlaceVec2Head || vectorMath.bCanPlaceVec2Tail) // V2 head & tail 
        {

            PV.RPC("PlaceHeadandTailLabels", RpcTarget.OthersBuffered, 1, vectorMath.bCanPlaceVec2Tail, vectorMath.bCanPlaceVec2Head, vectors[1]._headLabel.transform.position, vectors[1]._tailLabel.transform.position, vectors[1]._headLabel.text, vectors[1]._tailLabel.text, myPlayerRef.myPlayerActorNumber);
            vectorMath.bCanPlaceVec2Head = false; vectorMath.bCanPlaceVec2Tail = false; // reset
        }
        else if (vectorMath.bCanPlaceVec3Head || vectorMath.bCanPlaceVec3Tail) // V3 head & tail 
        {

            PV.RPC("PlaceHeadandTailLabels", RpcTarget.OthersBuffered, 2, vectorMath.bCanPlaceVec3Tail, vectorMath.bCanPlaceVec3Head, vectors[2]._headLabel.transform.position, vectors[2]._tailLabel.transform.position, vectors[2]._headLabel.text, vectors[2]._tailLabel.text, myPlayerRef.myPlayerActorNumber);
            vectorMath.bCanPlaceVec3Head = false; vectorMath.bCanPlaceVec3Tail = false; // reset
        }
        else if (vectorMath.bCanPlaceVec4Head || vectorMath.bCanPlaceVec4Tail) // V4 head & tail 
        {

            PV.RPC("PlaceHeadandTailLabels", RpcTarget.OthersBuffered, 3, vectorMath.bCanPlaceVec4Tail, vectorMath.bCanPlaceVec4Head, vectors[3]._headLabel.transform.position, vectors[3]._tailLabel.transform.position, vectors[3]._headLabel.text, vectors[3]._tailLabel.text, myPlayerRef.myPlayerActorNumber);
            vectorMath.bCanPlaceVec4Head = false; vectorMath.bCanPlaceVec4Tail = false; // reset
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
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 0, vectors[0]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
           beamPlacement.bCanPlaceVec1Labels = false;
        }
        if (beamPlacement.bCanPlaceVec2Labels)
        {
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 1, vectors[1]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            beamPlacement.bCanPlaceVec2Labels = false;
        }
        if (beamPlacement.bCanPlaceVec3Labels)
        {
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 2, vectors[2]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            beamPlacement.bCanPlaceVec3Labels = false;
        }
        if (beamPlacement.bCanPlaceVec4Labels)
        {
            PV.RPC("InitVectorName", RpcTarget.OthersBuffered, 3, vectors[3]._nameLabel.transform.position, myPlayerRef.myPlayerActorNumber);
            beamPlacement.bCanPlaceVec4Labels = false;
        }
    }

    [PunRPC]
    public void PlaceHeadandTailLabels(int _i, bool tail, bool head, Vector3 _headPos, Vector3 _tailPos, string _headValue, string _tailValue, string _actorNumber)
    {
        switch (_i)
        {
            case 0:
                // head
                if (head)
                {
                    headLabelV1 = Instantiate(headLabelPrefab, _headPos, Quaternion.identity);
                    headLabelV1.text = _headValue;
                    Debug.Log("head label value is " + headLabelV1.text);
                    AddToBin(_actorNumber, headLabelV1); // add to bin
                }

                if (tail)
                {
                    // tail 
                    tailLabelV1 = Instantiate(tailLabelPrefab, _tailPos, Quaternion.identity);
                    tailLabelV1.text = _tailValue;
                    Debug.Log("tail label value is " + tailLabelV1.text);
                    AddToBin(_actorNumber, tailLabelV1); // add to bin
                }
                break;

            case 1:
                // head
                if (head)
                {
                    headLabelV2 = Instantiate(headLabelPrefab, _headPos, Quaternion.identity);
                    headLabelV2.text = _headValue;
                    AddToBin(_actorNumber, headLabelV2); // add to bin
                }

                if (tail)
                {
                    // tail 
                    tailLabelV2 = Instantiate(tailLabelPrefab, _tailPos, Quaternion.identity);
                    tailLabelV2.text = _tailValue;
                    AddToBin(_actorNumber, tailLabelV2); // add to bin
                }
                break;
            case 2:
                // head
                if (head)
                {
                    headLabelV3 = Instantiate(headLabelPrefab, _headPos, Quaternion.identity);
                    headLabelV3.text = _headValue;
                    AddToBin(_actorNumber, headLabelV3); // add to bin
                }

                if (tail)
                {
                    // tail 
                    tailLabelV3 = Instantiate(tailLabelPrefab, _tailPos, Quaternion.identity);
                    tailLabelV3.text = _tailValue;
                    AddToBin(_actorNumber, tailLabelV3); // add to bin
                }
                break;
            case 3:
                // head
                if (head)
                {
                    headLabelV4 = Instantiate(headLabelPrefab, _headPos, Quaternion.identity);
                    headLabelV4.text = _headValue;
                    AddToBin(_actorNumber, headLabelV4); // add to bin
                }

                if (tail)
                {
                    // tail 
                    tailLabelV4 = Instantiate(tailLabelPrefab, _tailPos, Quaternion.identity);
                    tailLabelV4.text = _tailValue;
                    AddToBin(_actorNumber, tailLabelV4); // add to bin
                }
                break;
        }
    }

    [PunRPC]
    public void InitVectorName(int _i, Vector3 _pos, string _actorNumber)
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
                Debug.Log("Instantiating b");
                TextMeshPro _nameB = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameB.text = "B";

                AddToBin(_actorNumber, _nameB); // add to bin

                break;
            case 2:
                Debug.Log("Instantiating C");
                TextMeshPro _nameC = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameC.text = "C";

                AddToBin(_actorNumber, _nameC); // add to bin

                break;
            case 3:
                Debug.Log("Instantiating D");
                TextMeshPro _nameD = Instantiate(nameLabelPrefab, _pos, Quaternion.identity);
                _nameD.text = "D";

                AddToBin(_actorNumber, _nameD); // add to bin

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
