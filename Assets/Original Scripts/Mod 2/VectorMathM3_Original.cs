using System.Collections;
//using MathNet.Numerics;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using MathNet.Numerics.LinearAlgebra;

public class VectorMathM3_Original : MonoBehaviour
{
    [SerializeField, Tooltip("The 3 vectors in the scene")]
    public List<VectorControlM3_Original> vectors;

    private static Vector3 v1, v2, v3 = Vector3.one; // ASSUME UNIT VEC V1, V2, V3
    void Start()
    {


        vectors[0].SetName("A");
        vectors[1].SetName("B");
        vectors[2].SetName("C");
        vectors[3].SetName("D");

        vectors[0].vecColor = GLOBALS.visCyan;
        vectors[1].vecColor = GLOBALS.visOrange;
        vectors[2].vecColor = GLOBALS.visMagenta;
        vectors[3].vecColor = GLOBALS.visLime;

        foreach (VectorControlM3_Original v in vectors)
        {
            v.gameObject.SetActive(false);
        }
    }

    void Update()
    {

        // check if handedness button requires inverting the Cross product
        if (GLOBALS.invertCross)
        {
            if (GLOBALS.didCross)
            {
                Vector3 crossProduct = vectors[2].GetVectorComponents();
                vectors[2].GetComponent<VectorControl_Original>()._head.position = vectors[2].transform.position - crossProduct;
                GLOBALS.invertCross = false;
            }
        }
    }

    #region public methods


    /// <summary>
    /// USE ONLY FOR TRIGGER DOWN
    /// </summary>
    /// <param name="v"></param>
    /// <param name="isHead"></param>
    /// <param name="loc"></param>
    /// 
    public void PlaceVector3(int v, bool isHead, Vector3 loc)
    {
        if (isHead)
        {
            if (vectors[v].GetComponent<VectorControl_Original>().isCorrectPlacement) //if the tail is on the poc, let the user place wherever
            {
                vectors[v].GetComponent<VectorControl_Original>()._head.position = loc;
                if (v == 0) vectors[v].SetEnabledLabels(true, true, false, false);
                else vectors[v].SetEnabledLabels(false, true, false, false);
            }
            else //if the tail is not on the poc, the user can't choose a place, it auto fills to the poc 
            {
                vectors[v].GetComponent<VectorControl_Original>()._head.position = GetComponent<BeamPlacementM3_Original>().pocPos;
                if (v == 0) vectors[v].SetEnabledLabels(true, true, false, false);
                else vectors[v].SetEnabledLabels(true, false, false, false);
                //  GLOBALS.stage++;
            }

        }
        else
        {
            if (Vector3.Distance(loc, GetComponent<BeamPlacementM3_Original>().pocPos) < 0.20f)
            {
                vectors[v].transform.position = GetComponent<BeamPlacementM3_Original>().pocPos;
                vectors[v].GetComponent<VectorControl_Original>().isCorrectPlacement = true;
                vectors[v].SetEnabledLabels(false, false, false, false);
                //  GLOBALS.stage++;
            }
            else
            {
                vectors[v].transform.position = loc;
                vectors[v].GetComponent<VectorControl_Original>().isCorrectPlacement = false;
                vectors[v].SetEnabledLabels(true, false, false, false);
            }
            vectors[v].gameObject.SetActive(true);

        }

    }

    public void PlaceVectorPoint(int v, bool isHead, Vector3 loc)  //v is vector index in list, loc is the position
    {
        if (v >= vectors.Count)
        {
            // Debug.LogError("Error: Vector index out of bounds.");
            return;
        }



        /*if (GLOBALS.stage >= Stage.m3v1p1) {
            if (isHead) //
            {
                if (vectors[v].GetComponent<VectorControl_Original>().isCorrectPlacement == true) //if the placed point has already been placed on the poc, everything is gucci u can continue
                {
                    Debug.Log("Same as POC POS");
                    vectors[v].GetComponent<VectorControl_Original>()._head.position = loc;
                     
                }
                else
                {
                    vectors[v].GetComponent<VectorControl_Original>()._head.position = GetComponent<BeamPlacementM3_Original>().pocPos; //if not....pain 
                }
                vectors[v].SetEnabledLabels(true, true, false, false);
                vectors[v].GetComponent<VectorControl_Original>().isCorrectPlacement = false; //reset
            }
            else //tail 
            {
                if (Vector3.Distance(loc, GetComponent<BeamPlacementM3_Original>().pocPos) < 0.15f) //placed at poc
                {
                    Debug.Log("already in contact w POC, can place wherever");
                    vectors[v].transform.position = GetComponent<BeamPlacementM3_Original>().pocPos; //YEET to the poc
                    vectors[v].GetComponent<VectorControl_Original>().isCorrectPlacement = true;
                }
                else //tail placed wherever
                {
                    vectors[v].transform.position = loc; 
                   // Debug.Log("assigning to poc");
                }

                vectors[v].SetEnabledLabels(true, false, false, false);
            }
            vectors[v].gameObject.SetActive(true);
        }*/

        else
        {
            if (isHead)
            {
                vectors[v].GetComponent<VectorControl_Original>()._head.position = loc;
                vectors[v].SetEnabledLabels(true, true, false, false);
            }
            else
            {
                Debug.Log("outside of m3v1p1");
                vectors[v].transform.position = loc;
                vectors[v].gameObject.SetActive(true);
                vectors[v].SetEnabledLabels(true, false, false, false);
            }
        }


    }


    //IN PROGRESS
    /* public void forceCalc()
     { 
          Matrix<float> A = Matrix<float>.Build.DenseOfArray(new float[,] {
             { v1.x, v1.y, v1.z },
             { v2.x, v2.y, v2.z},
             {v3.x, v3.y, v3.z }
         });
     }*/




    // BeamPlacement.cs can set which vector labels are displayed via this function
    public void SetVectorLabels(int v, bool tail, bool head, bool components, bool units)
    {
        // passes it to the responsible VectorControl object
        vectors[v].SetEnabledLabels(tail, head, components, units);
    }


    #endregion
}
