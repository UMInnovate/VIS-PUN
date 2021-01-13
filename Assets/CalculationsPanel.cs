using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalculationsPanel : MonoBehaviour
{
    public Text header;
    // Start is called before the first frame update
    void Start()
    {
      // gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCalculationsSequence()
    {
        gameObject.SetActive(true);
        gameObject.transform.position = GLOBALS.pocPos + Vector3.one;
        Debug.Log("calc canv init at " + gameObject.transform.position.ToString());

        header.text = "|" + GLOBALS.SelectedVec.GetComponent<VectorProperties>().gameObject.name.Substring(12) + "| = " + GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().GetMagnitude();
        header.text +=  GLOBALS.SelectedVec.GetComponent<VectorControlM3_Original>().GetVectorComponents().ToString(GLOBALS.format);       
    }
}
