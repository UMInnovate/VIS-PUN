using System.Collections;
//using MathNet.Numerics;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using MathNet.Numerics.LinearAlgebra;

public class VectorMathM3 : MonoBehaviour
{
    [SerializeField, Tooltip("The 3 vectors in the scene")]
    public List<VectorControlM3> vectors;

    private static float[,] Fsystem;

    private static Vector3 v1, v2, v3 = Vector3.one; // ASSUME UNIT VEC V1, V2, V3
    private int debugCount = 0; 
    [HideInInspector]
    public bool bCanPlaceVec1, bCanPlaceVec2,
    bCanPlaceVec3, bCanPlaceVec4 = false; ///* PUN STUFF 

    [HideInInspector] public List<float> systemSolution; 

    [HideInInspector] public Vector3 vec1Pos, vec2Pos, vec3Pos, vec4Pos; //* PUN STUFF
    [HideInInspector] public Vector3 localVec1Pos, localVec2Pos, localVec3Pos, localVec4Pos; 
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

        foreach (VectorControlM3 v in vectors)
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
                vectors[2].GetComponent<VectorControlM3>()._head.position = vectors[2].transform.position - crossProduct;
                GLOBALS.invertCross = false;
            }
        }
    }

    #region public methods

    public void PlaceVector3Point(int v, Vector3 loc) 
    {

        if (vectors[v].GetComponent<VectorControlM3>().canPlaceHead) //if inputting the head, snap the tail to poc
        {
            vectors[v].transform.position = GetComponent<BeamPlacementM3>().pocPos;
            vectors[v].GetComponent<VectorControlM3>()._head.position = loc;

            //vectors[v].SetEnabledLabels(false, true, false, false);
            GLOBALS.headPos = loc;
            GLOBALS.tailPos = GetComponent<BeamPlacementM3>().pocPos;

        }
        else //if inputting the tail, snap the head
        {
            vectors[v].transform.position = loc;
            Console.WriteLine("loc " + loc);
            switch (v) {
                case 0:
                    localVec1Pos = vectors[v].transform.localPosition;
                    Console.WriteLine("local loc at v1 " + v + " is " + localVec1Pos);
                    break;
                case 1:
                    localVec2Pos = vectors[v].transform.localPosition;
                    break;
                case 2:
                    localVec3Pos = vectors[v].transform.localPosition;
                    break;
                case 3:
                    localVec4Pos = vectors[v].transform.localPosition;
                    break;
                default:
                    Debug.Log("this is so disgusting im so sorry.");
                    break;

            } 

            vectors[v].GetComponent<VectorControlM3>()._head.position = GetComponent<BeamPlacementM3>().pocPos;

            Console.WriteLine("raising flag for v" + v);
            RaiseFlagsForHeadTailLabels(v, true, loc);
            // vectors[v].SetEnabledLabels(true, false, false, false);
            GLOBALS.headPos = GetComponent<BeamPlacementM3>().pocPos;
            GLOBALS.tailPos = loc;

        }

        vectors[v].gameObject.SetActive(true);
    }

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
            if (vectors[v].GetComponent<VectorControlM3>().isCorrectPlacement) //if the tail is on the poc, let the user place wherever
            {
                vectors[v].GetComponent<VectorControlM3>()._head.position = loc;

              //  RaiseFlagsForHeadTailLabels(v, true); 
                vectors[v].GetComponent<VectorControlM3>()._headGameObject.gameObject.transform.position = loc;
                if (v == 0)
                {
                    vectors[v].SetEnabledLabels(true, true, false, false);
                   // RaiseFlagsForHeadTailLabels(v, true, true);
                }
                else
                {
                    vectors[v].SetEnabledLabels(false, true, false, false);
                  // RaiseFlagsForHeadTailLabels(v, false, true);
                }
            }
            else //if the tail is not on the poc, the user can't choose a place, it auto fills to the poc 
            {
                vectors[v].GetComponent<VectorControlM3>()._head.position = GetComponent<BeamPlacementM3>().pocPos;
                vectors[v].GetComponent<VectorControlM3>()._headGameObject.gameObject.transform.position = GetComponent<BeamPlacementM3>().pocPos;
                if (v == 0)
                {
                    vectors[v].SetEnabledLabels(true, true, false, false);
                   // RaiseFlagsForHeadTailLabels(v, true, true);
                }
                else
                {
                    vectors[v].SetEnabledLabels(true, false, false, false);
                   // RaiseFlagsForHeadTailLabels(v, true, false);
                }
            }
        }

        else
        {
            if (Vector3.Distance(loc, GetComponent<BeamPlacementM3>().pocPos) < 0.15f)
            {
                vectors[v].transform.position = GetComponent<BeamPlacementM3>().pocPos;
                vectors[v].GetComponent<VectorControlM3>().isCorrectPlacement = true;
                vectors[v].SetEnabledLabels(false, false, false, false);
                //if (v == 0) RaiseFlagsForHeadTailLabels(v, true); //it's snapped to poc, so just show tail
               // else RaiseFlagsForHeadTailLabels(v, true);
            }
            else
            {
                vectors[v].transform.position = loc;
                vectors[v].GetComponent<VectorControlM3>().isCorrectPlacement = false;
                vectors[v].SetEnabledLabels(true, false, false, false);
               // RaiseFlagsForHeadTailLabels(v, true);
            }
            vectors[v].gameObject.SetActive(true);

        }

    }

    /// <summary>
    /// Raise Flags for RPCRECEIVERM3 Class to send RPCs accoridngly
    /// </summary>
    /// <param name="v">Vector Number in reference to vectorcontorl arr</param>
    /// <param name="tail">is the tail going to be sent out</param>
    /// <param name="head">is the head going to be sent out</param>
    private void RaiseFlagsForHeadTailLabels(int v, bool flag, Vector3 loc)
    {
        debugCount++;
        Console.WriteLine("DB COUNT " + debugCount);
        switch(v)
        {
            case 0:
                if (flag) { bCanPlaceVec1 = true; vec1Pos = loc; bCanPlaceVec2 = false; bCanPlaceVec3 = false; bCanPlaceVec4 = false; }
                break;
            case 1:
                if (flag) { bCanPlaceVec2 = true; vec2Pos = loc; bCanPlaceVec1 = false; bCanPlaceVec3 = false; bCanPlaceVec4 = false; }
                break;
            case 2:
                if (flag) { bCanPlaceVec3 = true; vec3Pos = loc; bCanPlaceVec1 = false; bCanPlaceVec2 = false; bCanPlaceVec4 = false; }
                break;
            case 3:
                if (flag) { bCanPlaceVec4 = true; vec4Pos = loc; bCanPlaceVec1 = false; bCanPlaceVec2 = false; bCanPlaceVec3 = false; }
                break;

            default:
                Debug.Log("inaccessible index requested in vectormathm3: raiseflagsheadtail.");
                break;
        }
    }

    public void PlaceVectorPoint(int v, bool isHead, Vector3 loc)  //v is vector index in list, loc is the position
    {
        if (v >= vectors.Count)
        {
            // Debug.LogError("Error: Vector index out of bounds.");
            return;
        }
        else
        {
            if (isHead)
            {
                vectors[v].GetComponent<VectorControlM3>()._head.position = loc;
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
    static int GaussianElimination(float[,] a, int n)
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
        //Grab globas UnVec index at n get vector properties and at correct force magnitude to public variable
        else
        {
            for (int i = 0; i < n; i++)
            {
                Debug.Log("result item " + a[i, n] / a[i, i] + " ");
                res.Add(a[i, n] / a[i, i]);
                GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().correctForceValue = a[i, n] / a[i, i];
            }
            return res;
        }
    }

    static float[,] BuildMatrix()
    {
        float[,] Fsystem = {
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
        systemSolution = solution; 

        if (solution == null)
        {
            Debug.Log("solution is 0 or inf");
        }
        else
        {
            //go through each unknown vector and multiply the calculated (x,y,z) components to its unit vec
            //this is saved as the correct force vector in vectorproperties, which is then compared with the student's force vector, 
            //.. calculated with their inputted forceVal.
            for (int i = 0; i < n; i++)
            {
                GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().forceVec =
                new Vector3(GLOBALS.unknownUVecs[i].x * solution[0],
                GLOBALS.unknownUVecs[i].y * solution[1],
                GLOBALS.unknownUVecs[i].z * solution[2]);

                Debug.Log("force vec for " + GLOBALS.unknownVecs[i].name + " is "
                    + GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().forceVec);
            }
        }
    }

    public void ValidateForceSystem()
    {
        //Summary: Check if input force values equal that of the calculated force values
        //Calcuate tolerance allowed of every calculated values
        //Check if Input1 is > lower tolerance and < high tolerance
        //If yes mark input force value as correct
        //If no mark input force value as incorrect
        int correctForces = 0;
        float tol = 0.1f;
        float[,] forceTol =
        {
                { 0, 0 },
                { 0, 0 },
                { 0, 0 }
            };


        for (int i = 0; i < 3; i++)
        {

            forceTol[i, 0] = GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().correctForceValue * (1 - tol);
            forceTol[i, 1] = GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().correctForceValue * (1 + tol);

            //Only swaps high and low tolerances if values are negative
            //EX: low tol = -9 and high tol = -14. These values need to be change so that the conditions of low <= force value <= high
            swapTol(forceTol, i);

            Debug.Log("low force tol at " + i + " is " + forceTol[i, 0] + " high force tol at " + i + " is " + forceTol[i, 1]);
        }

     /*   for (int i = 0; i < 3; i++)
        {
            if ((float)GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().forceValue > forceTol[i, 0]
                && (float)GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().forceValue < forceTol[i, 1])
            {
                Debug.Log("Input force " + (float)GLOBALS.unknownVecs[i].GetComponent<VectorPropertiesM3>().forceValue
                    + " is greater than " + forceTol[i, 0] + " and less than " + forceTol[i, 1]);
                correctForces++;
            }
            else
            {
                Debug.Log("Input force " + (float)GLOBALS.unknownVecs[i].GetComponent<VectorProperties>().forceValue
                    + " is not greater than " + forceTol[i, 0] + " and less than " + forceTol[i, 1]);
            }
        }*/

        if (correctForces == 3)
        {
            Debug.Log("All force values were correct");
            GLOBALS.isValidSystem = true;
        }

        else
        {
            Debug.Log("Only " + correctForces + " force value(s) were correct");
            GLOBALS.isValidSystem = false;
        }
    }

    void swapTol(float[,] tols, int i)
    {
        float temp = 0;
        if (tols[i, 1] < tols[i, 0])
        {
            temp = tols[i, 0];
            tols[i, 0] = tols[i, 1];
            tols[i, 1] = temp;
        }
    }
}


#endregion

