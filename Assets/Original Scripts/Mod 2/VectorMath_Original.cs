using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VectorMath_Original : MonoBehaviour
{
    [SerializeField, Tooltip("The 3 vectors in the scene")]
    public List<VectorControl_Original> vectors;
    
    // display the equations, stored in other script
    DisplayEquation _displayEquation = null;
    [SerializeField, Tooltip("The angle text label")]
    private TextMeshPro angleLabel;
    [SerializeField, Tooltip("The angle arc")]
    private LineRenderer arc;

    [SerializeField, Tooltip("Dot Product Projection")]
    private LineRenderer dotProjection;
    [SerializeField, Tooltip("Dot Product Name Label")]
    private TextMeshPro dotNameLabel;

    void Start()
    {
        _displayEquation = GetComponent<DisplayEquation>();
        arc.enabled = false;
        angleLabel.enabled = false;

        dotProjection.startColor = GLOBALS.visLime;
        dotProjection.endColor = GLOBALS.visLime;
        dotProjection.startWidth = 0.012f;
        dotProjection.endWidth = 0.012f;
        dotProjection.enabled = false;

        dotNameLabel.text = "";
        dotNameLabel.enabled = false;

        vectors[0].SetName("A");
        vectors[1].SetName("B");
        vectors[2].SetName("C");
        vectors[3].SetName("D");

        vectors[0].vecColor = GLOBALS.visCyan;
        vectors[1].vecColor = GLOBALS.visOrange;
        vectors[2].vecColor = GLOBALS.visMagenta;
        vectors[3].vecColor = GLOBALS.visLime;

        foreach (VectorControl_Original v in vectors)
        {
            v.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // have all labels face the camera every frame
        RotateLabelsTowardsUser();

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
                if (v== 0) vectors[v].SetEnabledLabels(true, true, false, false);
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
            if (isHead) {
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
    
    // BeamPlacement.cs can set which vector labels are displayed via this function
    public void SetVectorLabels(int v, bool tail, bool head, bool components, bool units)
    {
        // passes it to the responsible VectorControl object
        vectors[v].SetEnabledLabels(tail, head, components, units);
    }

    public IEnumerator ComponentCalc(int v)
    {
        Vector3 headPos = vectors[v].GetRelHeadPos();
        Vector3 tailPos = vectors[v].GetRelTailPos();
        Vector3 answer = vectors[v].GetVectorComponents();

        if (GLOBALS.inFeet)
        {
            headPos *= GLOBALS.m2ft;
            tailPos *= GLOBALS.m2ft;
            answer *= GLOBALS.m2ft;
        }

        // x components
        string answerLine = answer.x.ToString(GLOBALS.format) + "i";
        yield return StartCoroutine(_displayEquation.WriteLine(0, "   " + headPos.x.ToString(GLOBALS.format)));
        yield return StartCoroutine(_displayEquation.WriteLine(1, "-  " + tailPos.x.ToString(GLOBALS.format)));
        yield return StartCoroutine(_displayEquation.WriteLine(2, answerLine));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(_displayEquation.FadeOut(0.2f, 0));
        yield return StartCoroutine(_displayEquation.FadeOut(0.2f, 1));

        // y
        answerLine += ", " + answer.y.ToString(GLOBALS.format) + "j";
        yield return StartCoroutine(_displayEquation.WriteLine(0, "   " + headPos.y.ToString(GLOBALS.format)));
        yield return StartCoroutine(_displayEquation.WriteLine(1, "-  " + tailPos.y.ToString(GLOBALS.format)));
        yield return StartCoroutine(_displayEquation.WriteLine(2, answerLine, 0.2f));
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(_displayEquation.FadeOut(0.2f, 0));
        yield return StartCoroutine(_displayEquation.FadeOut(0.2f, 1));

        // z
        answerLine += ", " + answer.z.ToString(GLOBALS.format) + "k";
        yield return StartCoroutine(_displayEquation.WriteLine(0, "   " + headPos.z.ToString(GLOBALS.format)));
        yield return StartCoroutine(_displayEquation.WriteLine(1, "-  " + tailPos.z.ToString(GLOBALS.format)));
        yield return StartCoroutine(_displayEquation.WriteLine(2, answerLine, 0.2f));
        yield return new WaitForSeconds(2);

        yield return StartCoroutine(_displayEquation.FadeOut(1f));
        _displayEquation.SetPanelEnabled(false);
    }
    #endregion

    #region public operations
    public IEnumerator DoVectorAddition()
    {
        /* vector addition will be done by:
         1) animation of translating Vector B's tail to Vector A's head
         2) placing Vector C's tail at Vector A's tail
         3) animation of Vector C's head moving to Vector B's head
        */
        vectors[1].SetEnabledLabels(false, false, true, false); // sets labels
        yield return StartCoroutine(vectors[1].AnimateVectorTranslation(vectors[0].GetComponent<VectorControl_Original>()._head.position)); // animates position translation
        vectors[2].transform.position = vectors[0].transform.position; // sets vector C position to vector A's tail 
        vectors[2].GetComponent<VectorControl_Original>()._head.position = vectors[2].transform.position; // vector C head at the same position
        vectors[2].gameObject.SetActive(true); // Initialize and enable 3rd Vector (Vector A + B) results
        vectors[2].SetName("A + B"); // Set name label
        yield return StartCoroutine(vectors[2].AnimateVectorHead(vectors[1].GetComponent<VectorControl_Original>()._head.position)); // moves head of vector 2 and passes parameters to define position of our Vector B
        vectors[2].SetEnabledLabels(false, false, true, false);
    }

    public IEnumerator DoVectorSubtraction()
    {
        /* vector subtraction will be done by:
         1) animated inversion of Vector B
         2) completing vector addition as above
        */
        vectors[1].SetEnabledLabels(false, false, true, false); // set component label
        Vector3 oldHeadPos = vectors[1].GetComponent<VectorControl_Original>()._head.position; // get old head label position
        Vector3 oldTailPos = vectors[1].transform.position; // transform old tail position of the label
        yield return StartCoroutine(vectors[1].AnimateVector(oldHeadPos, oldTailPos)); // inverting vector B, by swapping head and tail positions
        vectors[1].SetName("-B"); 
        vectors[2].SetName("A - B");
        yield return StartCoroutine(vectors[1].AnimateVectorTranslation(vectors[0].GetComponent<VectorControl_Original>()._head.position)); // animate the 3 vector translations to the head position
        vectors[2].transform.position = vectors[0].transform.position; // transforms tail position to head position 
        vectors[2].GetComponent<VectorControl_Original>()._head.position = vectors[2].transform.position; // get head component 
        vectors[2].gameObject.SetActive(true);  // activate head position object
        yield return StartCoroutine(vectors[2].AnimateVectorHead(vectors[1].GetComponent<VectorControl_Original>()._head.position)); // animate the 3 vector head position
        vectors[2].SetEnabledLabels(false, false, true, false);
    }

    public IEnumerator DoVectorDot()
    {
        /* Dot product will be done by:
         * Displaying the components of each vector
         * Displaying the equation for component calculation
         * Displaying answer upon click
         * Clearing components/answer
         *
         * Animating B tail to A tail
         *
         * Projecting B onto A
         *
         * Project A onto B
        */
        // DISPLAY THE EQUATION BASED ON COMPONENTS
        vectors[0].SetEnabledLabels(false, false, false, true); // set unit vectors labels for the head
        vectors[1].SetEnabledLabels(false, false, false, true); // set unit vectors lalbes for the tail

        // Display the equation for the component calculation
        yield return StartCoroutine(_displayEquation.DisplayDotEquation1(vectors[0].GetVectorComponents(), vectors[1].GetVectorComponents()));  

        // MOVE TAIL-TAIL, PROJECT
        vectors[0].SetEnabledLabels(false, false, true, false);
        vectors[1].SetEnabledLabels(false, false, true, false);
        yield return StartCoroutine(vectors[1].AnimateVectorTranslation(vectors[0].transform.position)); // Animate B tail to A tail
        DisplayAngle(); // Displays vector angles
        yield return StartCoroutine(ProjectDotProduct(0)); // Activate dot product coroutine for projecting B onto A
        yield return StartCoroutine(_displayEquation.DisplayDotEquation2(vectors[0].GetVectorComponents(), vectors[1].GetVectorComponents())); // display vector equation  component answer for new A
        yield return StartCoroutine(ProjectDotProduct(1)); // Activate dot product coroutine for projecting A onto B
        yield return StartCoroutine(_displayEquation.DisplayDotEquation3(vectors[0].GetVectorComponents(), vectors[1].GetVectorComponents())); // display vector equation component answer for new B

    }

    public IEnumerator DoVectorCross()
    {
        /* vector cross product will be done by:
         1) animated translation of Vector B's tail to Vector A's tail
         2) placing Vector C's tail at Vector A's tail
         3) animation of Vector C's head moving to calulated cross product (this is why there is handedness flipping)
        */

        yield return StartCoroutine(vectors[1].AnimateVectorTranslation(vectors[0].transform.position)); // Animated translation of Vector B's tail to Vector A's tail

        Vector3 crossProduct = Vector3.Cross(vectors[0].GetVectorComponents(), vectors[1].GetVectorComponents()); // Vector 3 cross product animate over vector A's tail to vector B's tail
        if (GLOBALS.rightHanded) // checks the handedness of the coordinate system
            crossProduct = -crossProduct; // inverts the cross product

        vectors[2].transform.position = vectors[0].transform.position; // Vector C's tail placed at vector A's tail
        vectors[2].GetComponent<VectorControl_Original>()._head.position = vectors[0].transform.position; // intializing vector C head position before animation
        vectors[2].gameObject.SetActive(true); // Activate vector C object
        vectors[2].SetName("A x B"); // set Vector dot product label name

        yield return StartCoroutine(vectors[2].AnimateVectorHead(vectors[2].transform.position + crossProduct)); // Animation of Vector C's head moving to calulated cross product
        vectors[2].SetEnabledLabels(false, false, true, false); // Set component label text for vector C
        
    }
    #endregion

    #region Private/helper functions
    private void DisplayAngle()
    {
        angleLabel.enabled = true;
        Vector3 relLabelPos = vectors[0]._head.forward + vectors[1]._head.forward;
        angleLabel.transform.position = vectors[1].transform.position + relLabelPos * 0.06f;

        float theta = Vector3.Angle(vectors[0].GetVectorComponents(), vectors[1].GetVectorComponents());
        angleLabel.text = theta.ToString("F0") + "°";
        CreateArc();
    }

    private void RotateLabelsTowardsUser()
    {
        if(angleLabel.enabled)
        {
            Quaternion angleRotation = Quaternion.LookRotation(angleLabel.transform.position - vectors[0]._camera.transform.position);
            angleLabel.transform.rotation = Quaternion.Slerp(angleLabel.transform.rotation, angleRotation, 1.5f);
        }
        if(dotNameLabel.enabled)
        {
            dotNameLabel.transform.position = (2*dotProjection.GetPosition(0) + dotProjection.GetPosition(1)) / 3;
            Quaternion dotRotation = Quaternion.LookRotation(dotNameLabel.transform.position - vectors[0]._camera.transform.position);
            dotNameLabel.transform.rotation = Quaternion.Slerp(dotNameLabel.transform.rotation, dotRotation, 1.5f);
            dotNameLabel.transform.position -= dotNameLabel.transform.forward * 0.07f;
        }
    }

    private void CreateArc()
    {
        arc.enabled = true;
        arc.startWidth = 0.005f;
        arc.endWidth = 0.005f;
        int n = 32;
        float rad = 0.035f;
        Vector3 center = vectors[0].transform.position;
        Vector3 posA = vectors[0]._head.forward * rad;
        Vector3 posB = vectors[1]._head.forward * rad;
        Vector3 temp = posA;
        arc.positionCount = n;
        float deltaTheta = Mathf.Deg2Rad * Vector3.Angle(vectors[0].GetVectorComponents(), vectors[1].GetVectorComponents()) / n;

        for (int i = 0; i < n; i++)
        {
            arc.SetPosition(i, center + temp);
            temp = Vector3.RotateTowards(temp, posB, deltaTheta, 0.0f);
        }
    }

    

    private IEnumerator ProjectDotProduct(int n)
    {
        // n = 0: project B onto A, and get A * Bcos(angle)
        // n = 1: project A onto B, and get B * Acos(angle)
        float theta = Vector3.Angle(vectors[0].GetVectorComponents(), vectors[1].GetVectorComponents());
        float mag = vectors[1 - n].GetMagnitude();
        if(GLOBALS.inFeet)
        {
            mag *= GLOBALS.m2ft;
        }
        float projectLen = mag * Mathf.Cos(Mathf.Deg2Rad * theta);
        Vector3 anchorPos = vectors[0].transform.position;
        Vector3 animStartPos = vectors[1-n]._head.position;
        Vector3 animEndPos = anchorPos + vectors[n]._head.forward * projectLen;
        Vector3 animDeltaPos = animEndPos - animStartPos;

        dotNameLabel.text = "";
        dotProjection.SetPosition(0, anchorPos);
        dotProjection.SetPosition(1, animStartPos);
        dotProjection.enabled = true;

        vectors[0].vecColor = new Color(0.4f, 0.4f, 0.4f, 0.2f);
        vectors[1].vecColor = new Color(0.4f, 0.4f, 0.4f, 0.2f);
        vectors[0].SetEnabledLabels(false, false, false, false);
        vectors[1].SetEnabledLabels(false, false, false, false);

        WaitForSeconds deltaTime = new WaitForSeconds(GLOBALS.waitTime);
        int numSteps = (int)(GLOBALS.animTime / GLOBALS.waitTime) + 1;
        float[] curve = vectors[0].GenerateLogisticCurve(numSteps);
        for (int i = 0; i < numSteps; i++)
        {
            dotProjection.SetPosition(1, animStartPos + (animDeltaPos * curve[i]));
            yield return deltaTime;
        }
        dotProjection.SetPosition(1, animEndPos);
        dotNameLabel.enabled = true;
        if (n == 0)
            dotNameLabel.text = "|B|cos(" + theta.ToString("F0") + "°)";
        else
            dotNameLabel.text = "|A|cos(" + theta.ToString("F0") + "°)";
    }
    #endregion

}
