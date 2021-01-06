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
    //[SerializeField] Button ConfirmButton;
    [SerializeField] Text IFText;

    private GameObject panel;

    //PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponent<GameObject>();
        IFText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckClicked(GameObject button)
    {
        panel.SetActive(false);
        //go back to v selection
        GLOBALS.stage++;
    }

    public void NumberButtonClicked(int buttonValue)
    {
        IFText.text += buttonValue.ToString();
    }
}
