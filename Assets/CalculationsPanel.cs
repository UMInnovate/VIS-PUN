using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculationsPanel : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
       gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCalculationsSequence()
    {
        gameObject.SetActive(true);
        gameObject.transform.position =  new Vector3(1.5f, 0f, 1f);
        Debug.Log("calc canv init at " + gameObject.transform.position.ToString());
    }
}
