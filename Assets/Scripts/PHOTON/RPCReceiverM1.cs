using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RPCReceiverM1 : MonoBehaviour
{  
    private PhotonView PV; 

    //HEAD LABEL
    [SerializeField]
    private VectorControlM1 vectorControlM1_Ref;
    [SerializeField]
    private TextMeshPro headLabelPrefab;
    private TextMeshPro headLabel_Ref;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        if (PV != null)
        {
            Debug.Log("START: Able to get the Photon View component");
        }
        else if (PV == null)
        {
            Debug.LogError("START: photon view null");
        }
    }

    void Update()
    {       
        //HeadLabelInput();
    }

   
    //-------------------------------HEAD LABEL-------------------------------------------

    //private void HeadLabelInput()
    //{
    //    if(vectorControlM1_Ref.bCanInitHeadLabel) // spawn head label
    //    {
    //        Debug.Log("COM: can init head label");
    //        if(PV != null)
    //            PV.RPC("SpawnHeadLabel", RpcTarget.OthersBuffered, vectorControlM1_Ref._headLabel.transform.position, vectorControlM1_Ref._headLabel.text);
    //        vectorControlM1_Ref.bCanInitHeadLabel = false; // reset
    //    }

    //    else if(vectorControlM1_Ref.bCanUpdateHeadLabel) // update head label pos & val
    //    {
    //        if (PV != null)
    //            PV.RPC("UpdateHeadLabel", RpcTarget.OthersBuffered, vectorControlM1_Ref._headLabel.transform.position, vectorControlM1_Ref._headLabel.text);
    //        // reset is on VecControlM1.cs
    //    }

    //    else
    //    {
    //        return;
    //    }
    //}

    //[PunRPC] // spawn head label
    //public void SpawnHeadLabel(Vector3 _pos, string _val)
    //{
    //    Debug.Log("RPC: instantiating head label");
    //    headLabel_Ref = Instantiate(headLabelPrefab, _pos, Quaternion.identity);
    //    headLabel_Ref.text = _val;
    //}

    //[PunRPC] // update head label pos & val
    //public void UpdateHeadLabel(Vector3 _pos, string _val)
    //{
    //    headLabel_Ref.transform.position = _pos;
    //    headLabel_Ref.text = _val;
    //}
}
