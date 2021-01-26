using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RPCReceiverM3 : MonoBehaviour
{
    //VECTOR LABELS
    public TextMeshPro label;
    public TextMeshPro[] headLabels;
    public TextMeshPro[] tailLabels;

    //VECTOR NAMES
    public BeamPlacementM3_Original beamPlacement;
    public VectorMathM3_Original vectorMath;
    public List<VectorControlM3_Original> vectors;
    public TextMeshPro nameLabel;


        
        //CLEAN UP
    //ORIGIN LABELS


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
