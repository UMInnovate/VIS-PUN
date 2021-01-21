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

    private static float [,] Fsystem;

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

    // Function to print the matrix 
    static void PrintMatrix(float[,] a, int n)
    {
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= n; j++)
                Debug.Log("item " + a[i, j] + " ");
            Debug.Log("");
        }
    }

    // function to reduce matrix to reduced row echelon form. 
    static int GaussianElimination(float [,] a, int n)
    {
        int i, j, k = 0, c, flag = 0;

        // Performing elementary operations 
        for (i = 0; i < n; i++)
        {
            if (a[i, i] == 0)
            {
                c = 1;
                while ((i + c) < n && a[i + c, i] == 0)
                    c++;
                if ((i + c) == n)
                {
                    flag = 1;
                    break;
                }
                for (j = i, k = 0; k <= n; k++)
                {
                    float temp = a[j, k];
                    a[j, k] = a[j + c, k];
                    a[j + c, k] = temp;
                }
            }

            for (j = 0; j < n; j++)
            {

                // Excluding all i == j 
                if (i != j)
                {

                    // Converting Matrix to reduced row 
                    // echelon form(diagonal matrix) 
                    float p = a[j, i] / a[i, i];

                    for (k = 0; k <= n; k++)
                        a[j, k] = a[j, k] - (a[i, k]) * p;
                }
            }
        }
        return flag;
    }

    // To check whether infinite solutions  
    // exists or no solution exists 
    static int CheckConsistency(float[,] a,
                                int n, int flag)
    {
        int i, j;
        float sum;

        // flag == 2 for infinite solution 
        // flag == 3 for No solution 
        flag = 3;
        for (i = 0; i < n; i++)
        {
            sum = 0;
            for (j = 0; j < n; j++)
                sum = sum + a[i, j];
            if (sum == a[i, j])
                flag = 2;
        }
        return flag;
    }


    static List<float> PrintResult(float[,] a,
                        int n, int flag)
    {
        Debug.Log("Result is : ");
        List<float> res = new List<float>();

        if (flag == 2)
        {
            Debug.Log("Infinite Solutions Exists");
            return null;
        }
        else if (flag == 3)
        {
            Debug.Log("No Solution Exists");
            return null;
        }

        // Printing the solution by dividing  
        // constants by their respective 
        // diagonal elements 
        else
        {
            for (int i = 0; i < n; i++)
            {
                Debug.Log("result item " + a[i, n] / a[i, i] + " ");
                res.Add(a[i, n] / a[i, i]);
            }
            return res;
        }
    }

    static float[,] BuildMatrix()
    {
        float [,] Fsystem = {
            { GLOBALS.unknownUVecs[0].x, GLOBALS.unknownUVecs[1].x, GLOBALS.unknownUVecs[2].x, GLOBALS.forceVector.x },
            { GLOBALS.unknownUVecs[0].y, GLOBALS.unknownUVecs[1].y, GLOBALS.unknownUVecs[2].y, GLOBALS.forceVector.y },
            { GLOBALS.unknownUVecs[0].z, GLOBALS.unknownUVecs[1].z, GLOBALS.unknownUVecs[2].z, GLOBALS.forceVector.z },
        };

        return Fsystem;
    }

    public void SolveSystemOfEquations()
    {
        int n = 3, flag = 0;

        Fsystem = BuildMatrix();
        PrintMatrix(Fsystem, n);

        // Performing Matrix transformation 
        flag = GaussianElimination(Fsystem, n);

        if (flag == 1)
            flag = CheckConsistency(Fsystem, n, flag);

        List<float> solution = PrintResult(Fsystem, n, flag);
        if(solution == null)
        {
            Debug.Log("solution is 0 or inf");
        }
        else
        {
            //go through each unknown vector and multiply the calculated (x,y,z) components to its unit vec
            //this is saved as the correct force vector in vectorproperties, which is then compared with the student's force vector, 
            //.. calculated with their inputted forceVal.
            for (int i = 0; i< n; i++) 
            {
                GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().forceVec =
                new Vector3(GLOBALS.unknownUVecs[i].x * solution[0],
                GLOBALS.unknownUVecs[i].y * solution[1],
                GLOBALS.unknownUVecs[i].z * solution[2]);

                Debug.Log("force vec for " + GLOBALS.unknownVecs[i].name + " is "
                    + GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().forceVec);
            }
            
        }
    }

        public void ValidateForceSystem()
        {
            Debug.Log("In VFS");
            
            //Summary: Check if input force values equal that of the calculated force values
            //Calcuate tolerance allowed of every calculated values
            //Check if Input1 is > lower tolerance and < high tolerance
            //If yes mark input force value as correct
            //If no mark input force value as incorrect
            float tol = 0.02f;
            List<float[]> force_tol = new List<float[]>();          
            float[] force_tolInit = { 0, 0 };
            force_tol.Add(force_tolInit);
            force_tol.Add(force_tolInit);
            force_tol.Add(force_tolInit);
            force_tol.Add(force_tolInit);

            for (int i=0; i<force_tol.Count; i++)
            {
                Tolerance(GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().forceValue, force_tol[i], tol);
            }

            for(int i=0; i<force_tol.Count; i++)
            {
                if((float)GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().forceValue > force_tol[i][0] || 
                   (float)GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().forceValue < force_tol[i][1])
                {
                    Debug.Log("Correct force value");
                }
                else
                {
                    Debug.Log("Incorrect force value");
                }
            }


    }

    public void Tolerance(float val, float [] val_tol, float tol)
    {
        val_tol[0] = val * (1 - tol) ;
        val_tol[1] = val * (1 + tol);
    }
}


    #endregion

