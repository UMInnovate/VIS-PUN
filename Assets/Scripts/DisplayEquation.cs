using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEquation : MonoBehaviour
{
    [SerializeField, Tooltip("The background panel, in equation canvas")]
    private GameObject backPanel;
    [SerializeField, Tooltip("The 3 lines of text in equation canvas")]
    private List<Text> eqLines;

    private void Start()
    {
        backPanel.SetActive(false);
        foreach (Text text in eqLines)
            text.color = new Color(1, 1, 1, 0);
    }

    public void SetPanelEnabled(bool b)
    {
        backPanel.SetActive(b);
    }

    public IEnumerator WriteLine(int line, string text, float time = 1f)
    {
        if (line > eqLines.Count - 1)
            yield break;
        eqLines[line].color = new Color(1, 1, 1, 0);
        eqLines[line].text = text;
        yield return StartCoroutine(FadeIn(time, line));
    }

    // Dot Equation 1 refers to calculating the dot product through COMPONENT multiplication
    // this will display in the form "A · B = |Ax||Bx| + |Ay||By| + |Az||Bz|"
    public IEnumerator DisplayDotEquation1(Vector3 vec1, Vector3 vec2)
    {
        if (GLOBALS.inFeet)
        {
            vec1 *= GLOBALS.m2ft;
            vec2 *= GLOBALS.m2ft;
        }

        // Dot product calc via components
        eqLines[0].color = new Color(1, 1, 1, 0);
        eqLines[1].color = new Color(1, 1, 1, 0);
        eqLines[2].color = new Color(1, 1, 1, 0);
        backPanel.SetActive(true);
        eqLines[0].text = "A · B = |Ax||Bx| + |Ay||By| + |Az||Bz|";
        yield return StartCoroutine(FadeIn(1f, 0));

        float dot = Vector3.Dot(vec1, vec2);

        eqLines[1].text = "= (" + vec1.x.ToString(GLOBALS.format) + ")(" + vec2.x.ToString(GLOBALS.format) + ") + (" +
            vec1.y.ToString(GLOBALS.format) + ")(" + vec2.y.ToString(GLOBALS.format) + ") + (" +
            vec1.z.ToString(GLOBALS.format) + ")(" + vec2.z.ToString(GLOBALS.format) + ")";
        yield return StartCoroutine(FadeIn(1f, 1));
        eqLines[2].text = "A · B = " + dot.ToString(GLOBALS.format);
        if (GLOBALS.inFeet)
            eqLines[2].text += " ft²";
        else
            eqLines[2].text += " m²";
        yield return StartCoroutine(FadeIn(1f, 2));
        yield return new WaitForSeconds(4f);
        yield return StartCoroutine(FadeOut(1f));
        backPanel.SetActive(false);
    }


    // Dot Equation 2 refers to projecting vector B onto vector A
    // B*cos(angle) will be the projection
    public IEnumerator DisplayDotEquation2(Vector3 vec1, Vector3 vec2)
    {
        if (GLOBALS.inFeet)
        {
            vec1 *= GLOBALS.m2ft;
            vec2 *= GLOBALS.m2ft;
        }

        // Dot product calc via projection of B onto A
        eqLines[0].color = new Color(1, 1, 1, 0);
        eqLines[1].color = new Color(1, 1, 1, 0);
        eqLines[2].color = new Color(1, 1, 1, 0);
        backPanel.SetActive(true);
        eqLines[0].text = "A · B = |A||B|cos(angle)";
        yield return StartCoroutine(FadeIn(1f, 0));

        float dot = Vector3.Dot(vec1, vec2);
        float theta = Vector3.Angle(vec1, vec2);

        eqLines[1].text = "= (" + vec1.magnitude.ToString(GLOBALS.format) + ")(" + vec2.magnitude.ToString(GLOBALS.format) + ")cos(" + theta.ToString("F0") + "°)";
        yield return StartCoroutine(FadeIn(2f, 1));
        eqLines[2].text = "A · B = " + dot.ToString(GLOBALS.format);
        if (GLOBALS.inFeet)
            eqLines[2].text += " ft²";
        else
            eqLines[2].text += " m²";
        yield return StartCoroutine(FadeIn(2f, 2));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeOut(1f));

        backPanel.SetActive(false);
    }

    // Dot Equation 3 refers to projecting vector A onto vector B
    // A*cos(angle) will be the projection
    public IEnumerator DisplayDotEquation3(Vector3 vec1, Vector3 vec2)
    {
        if (GLOBALS.inFeet)
        {
            vec1 *= GLOBALS.m2ft;
            vec2 *= GLOBALS.m2ft;
        }

        // Dot product calc via projection of A onto B
        eqLines[0].color = new Color(1, 1, 1, 0);
        eqLines[1].color = new Color(1, 1, 1, 0);
        eqLines[2].color = new Color(1, 1, 1, 0);
        backPanel.SetActive(true);
        eqLines[0].text = "B · A = |B||A|cos(angle)";
        yield return StartCoroutine(FadeIn(1f, 0));

        float dot = Vector3.Dot(vec1, vec2);
        float theta = Vector3.Angle(vec1, vec2);
        eqLines[1].text = "= (" + vec2.magnitude.ToString(GLOBALS.format) + ")(" + vec1.magnitude.ToString(GLOBALS.format) + ")cos(" + theta.ToString("F0") + "°)";
        yield return StartCoroutine(FadeIn(2f, 1));
        eqLines[2].text = "B · A = " + dot.ToString(GLOBALS.format);
        if (GLOBALS.inFeet)
            eqLines[2].text += " ft²";
        else
            eqLines[2].text += " m²";
        yield return StartCoroutine(FadeIn(2f, 2));
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(FadeOut(1f));

        backPanel.SetActive(false);
    }

    // This will fade in whichever line of text is needed.
    // a time of ~1-2 seconds looks normal
    public IEnumerator FadeIn(float time, int line)
    {
        backPanel.SetActive(true);
        float inc = 0.01f;
        WaitForSeconds wait = new WaitForSeconds(inc);
        float alpha = 0.0f;
        while (alpha < 1.0f)
        {
            alpha += inc / time;
            eqLines[line].color = new Color(1, 1, 1, alpha);
            yield return wait;
        }
        eqLines[line].color = new Color(1, 1, 1, 1);
    }

    // This will fade out whichever line of text is needed.
    // a time of ~1-2 seconds looks normal
    // if line is not specified, ALL lines fade out
    public IEnumerator FadeOut(float time, int line = -1)
    {
        float inc = 0.01f;
        WaitForSeconds wait = new WaitForSeconds(inc);
        float alpha = 1.0f;

        // -1 is default, ALL lines fade out
        if(line == -1)
        {
            while (alpha > 0.0f)
            {
                alpha -= inc / time;
                foreach(Text text in eqLines)
                    text.color = new Color(1, 1, 1, alpha);
                yield return wait;
            }
            foreach(Text text in eqLines)
                text.color = new Color(1, 1, 1, 0);
        }
        // specific line fade out
        else
        {
            while(alpha > 0.0f)
            {
                alpha -= inc / time;
                eqLines[line].color = new Color(1, 1, 1, alpha);
                yield return wait;
            }
            eqLines[line].color = new Color(1, 1, 1, 0);
        }
    }
}
