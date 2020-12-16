using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHead : MonoBehaviour
{

    Renderer Renderer;
   public SphereCollider _collider;
    //Color initialColor;

    // Start is called before the first frame update
    void Start()
    {
        // _collider = GetComponent<SphereCollider>();
        if (_collider == null) Debug.Log("SphereCollider is null");
        _collider.radius = 0.25f;
        _collider.center = Vector3.zero;

       // initialColor = Renderer.material.color;
        
        Renderer = GetComponent<Renderer>();
        if (Renderer == null) Debug.Log("Renderer is null");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.tag == "tail" && other.gameObject.tag == "poc")
        { this.GetComponent<VectorControl_Original>().isCorrectPlacement = true;
            Debug.Log("this " + this.GetComponentInParent<GameObject>().gameObject.name + "has valid placement");  }
        Debug.Log("Collision detected between " + this.gameObject.name + " and " + other.gameObject.name);
        if (other.gameObject.tag == "pointer")
        {
          //  Renderer.material.color = GLOBALS.visHovered;
           // Debug.Log("Changing color to hovered.");
        }
        if (other.gameObject.tag == "poc")
        {
            //Renderer.material.color = GLOBALS.visValid;
            
            //Debug.Log("Changing color to valid");
        }
    }

    private void OnTriggerExit(Collider other)
    {
    //    Renderer.material.color = initialColor; //goback to init color
    }


}
