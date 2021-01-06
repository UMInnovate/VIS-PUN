using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationsPanel : MonoBehaviour
{
    private GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        panel = GetComponent<GameObject>();
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCalculationsSequence()
    {
        panel.transform.position = GLOBALS.pocPos + Vector3.one;

        panel.SetActive(true);
    }
}
