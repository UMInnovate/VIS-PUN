using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        _collider.radius = 1f;
        _collider.center = Vector3.zero;

       // initialColor = Renderer.material.color;
        
        Renderer = GetComponent<Renderer>();
        if (Renderer == null) Debug.Log("Renderer is null");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(this.gameObject.tag == "NameLabel" && other.gameObject.tag == "pointer")
        {
          //  Debug.Log("beam colliding with label");
            this.gameObject.GetComponent<TextMeshPro>().fontSize = 100; //make font biger
            this.gameObject.GetComponent<TextMeshPro>().color = Color.red;
            this.gameObject.GetComponentInParent<VectorProperties>().SetNameLabelHoverState(true);
        }

        if (this.gameObject.tag == "tail" && other.gameObject.tag == "poc")
        {
            this.GetComponent<VectorControl_Original>().isCorrectPlacement = true;
            Debug.Log("this " + this.GetComponentInParent<GameObject>().gameObject.name + "has valid placement");  }
       // Debug.Log("Collision detected between " + this.gameObject.name + " and " + other.gameObject.name);
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
        else
        {
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.gameObject.tag == "NameLabel" && other.gameObject.tag == "pointer")
        {
         //   Debug.Log("beam exiting collision with label");
            this.gameObject.GetComponent<TextMeshPro>().fontSize = 40; //make font biger
            this.gameObject.GetComponent<TextMeshPro>().color = Color.white;
            this.gameObject.GetComponentInParent<VectorProperties>().SetNameLabelHoverState(false);
        }
        else
        {
            return;
        }
        //    Renderer.material.color = initialColor; //goback to init color
    }



}
