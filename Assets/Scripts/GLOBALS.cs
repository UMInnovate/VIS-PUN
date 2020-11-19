

/*  GLOBALS contains the global variables, constants, and enums for VIS2
 *  
 *  GLOBALS.cs does not need to be attached to any GameObject
 *  and is automatically accessible in all scenes and all scripts
 */

using UnityEngine;

// for module 2: incremented by helper function in BeamPlacement.cs
public enum Stage
{
    m1orig, // Module 1 begins HERE
    m1rotate,
    m1vector,
    m1view,
    m2orig,     // Module 2 Begins HERE: place origin
    m2rotate,   // rotate origin
    v1p1,       // place Vector 1 Point 1 (tail)
    v1p2,       // place Point 2 (head)
    v1calc,     // shows the user the calculation of Vector components (head-tail)
    v2p1,       // repeat for second vector...
    v2p2,
    v2calc,
    opSel,      // Choose an operation to perform
    opView,     // watch the app animate/show the operation
    m3orig,     //MODULE 3 Begins HERE:
    m3rotate,   //rotate origin
    m3pin,
    m3poc,      //point of concurrency
    m3v1p1,     //place a vector tail
    m3v1p2,     //place a vector head
    m3v2p1,     //place a vector tail
    m3v2p2,     //place a vector head
    m3v3p1,     //place a vector tail
    m3v3p2,     //place a vector head
    m3v4p1,     //place a vector tail
    m3v4p2,     //place a vector head
    m3val,      //validate vector
    m3pop,      //popup for user interaction, "is this a correct vector?"
    m3opSel,    //choose a force vector
    m3opView   //watch the app animate force 
};

public enum VecOp
{
    none,
    Addition,
    Subtraction,
    Dot,
    Cross
};

// for Module 1 Vector Display
public enum DispMode
{
    Vector,
    Components,
    Units,
    Angles
}

public static class GLOBALS
{
    // default units are meters
    public static bool inFeet = true;
    // add audio settings
    public static bool soundOn = true;
    // is the vector correctly placed?
    public static bool isCorrectVectorPlacement; 
    // conversion value
    public const float m2ft = 3.28084f;
    // Unity is lefthanded, typical math is righthanded
    public static bool rightHanded = true;
    // flipZ is used for multiplying on z coordinates
    // and strictly follows handedness (-1 = right, 1 = left)
    public static int flipZ = -1;
    // Grid enable and size are settings
    public static bool gridOn = false;
    public static float gridSize = 0.1f;
    // Stage of application usage
    public static Stage stage;
    // what operation was selected (M2)
    public static VecOp opSelected = VecOp.none;
    // what mode we are viewing (M1)
    public static DispMode displayMode = DispMode.Vector;

    // animation parameters
    public const float waitTime = 0.02f;
    public const float animTime = 2.2f;
    // precision of digits
    public const string format = "F2";

    public static bool didCross = false;
    // temporary flag in case need to switch cross product
    public static bool invertCross = false;

    public static bool showingCoords = false;


    /* DEBUG GLOBALS */
    public static Vector3 headPos;
    public static Vector3 tailPos;
    public static Vector3 pocPos; 
    #region premade colors
    public static Color visCyan = new Color(0.4f, 1, 1, 0.5f);
    public static Color visOrange = new Color(1, 0.7f, 0, 0.5f);
    public static Color visMagenta = new Color(1, 0, 1, 0.5f);
    public static Color visLime = new Color(0.4f, 1, 0);
    public static Color visValid = new Color(207, 255, 212);
    public static Color visInvalid = new Color(237, 0, 24);
    public static Color visHovered = new Color(222, 248, 255);
    #endregion
}
