using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon;
using Photon.Realtime;
using Photon.Pun;

public class KeypadPanel : MonoBehaviour
{
    [SerializeField] List<Button> ValueButtons;
    [SerializeField] Button CheckButton;
    [SerializeField] Button ConfirmButton;

    //PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckClicked()
    {
        GetComponent<GameObject>().SetActive(false);
        //go back to v selection
        GLOBALS.stage = Stage.m3v4p2;
    }
}
