using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GiveInstructions : MonoBehaviour
{
    [SerializeField, Tooltip("The instruction text field, in  headpose canvas")]
    public Text text;
   // [SerializeField]
   private AudioSource audioSource;

    private AudioClip clip;

    

   // [SerializeField, Tooltip("If in multiplayer")] BeamPlacementM3 beamPlacement;
    private void Start()
    {
       audioSource = GetComponent<AudioSource>();
    }

    public void EnableText(bool b)
    {
        text.enabled = b;
    }

    public void DisplayText()
    {
        EnableText(true);
            switch (GLOBALS.stage)
            {
                case Stage.m1orig:
                    text.text = "PLACE YOUR ORIGIN" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: continue";
                if(GetComponent<BeamPlacementM3>() != null)
                {
                    if(GetComponent<BeamPlacementM3>().bIsViewer)
                    {
                        text.text = "You are now a viewer. Your controller has been disabled.";
                    }
                }
                    break;
                case Stage.m1rotate:
                    text.text = "ROTATE ORIGIN" + "\n" +
                        "Touchpad: rotate" + "\n" +
                        "(swipe clockwise/anticlockwise around touchpad) " + "\n" +
                        "Trigger: continue";
                    break;
                case Stage.m1vector:
                    text.text = "CREATE VECTOR" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: place vector";
                    break;
                case Stage.m1view:
                    if (GLOBALS.displayMode.ToString() == "Units")
                    {
                        text.text = "VIEWING UNIT VECTOR" + "\n" +
                          "Bumper: toggle labels" + "\n" +
                          "Trigger: replace vector" + "\n" +
                          "Home: Main Menu";
                    }
                    else
                    {
                        text.text = "VIEWING " + GLOBALS.displayMode.ToString() + "\n" +
                            "Bumper: toggle labels" + "\n" +
                            "Trigger: replace vector" + "\n" +
                            "Home: Main Menu";
                    }
                    break;
                // note Module 1 vs. Module 2
                case Stage.m2orig:
                    text.text = "PLACE YOUR ORIGIN" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: continue";
                    break;
                case Stage.m2rotate:
                    text.text = "ROTATE ORIGIN" + "\n" +
                        "Touchpad: rotate" + "\n" +
                        "(swipe clockwise/anticlockwise around touchpad) " + "\n" +
                        "Trigger: continue";
                    break;
                case Stage.v1p1:
                    text.text = "VECTOR 1 TAIL" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: place tail";
                    break;
                case Stage.v1p2:
                    text.text = "VECTOR 1 HEAD" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: place head";
                    break;
                case Stage.v1calc:
                    text.text = "CALCULATE VECTOR 1 COMPONENTS?" + "\n" +
                        "Trigger: show calculation";
                    break;
                case Stage.v2p1:
                    text.text = "VECTOR 2 TAIL" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: place tail";
                    break;
                case Stage.v2p2:
                    text.text = "VECTOR 2 HEAD" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: place head";
                    break;
                case Stage.v2calc:
                    text.text = "VECTOR 2 COMPONENTS" + "\n" +
                        "Trigger: continue";
                    break;
                case Stage.opView:
                    text.text = "Viewing " + GLOBALS.opSelected.ToString();
                    if ((int)GLOBALS.opSelected > 2)
                        text.text += " Product";
                    if (GLOBALS.opSelected != VecOp.Dot)
                    {
                        text.text += "\n" +
                        "Bumper: toggle labels";
                    }
                    text.text += "\n" + "Home: Main Menu";
                    break;
                case Stage.m3orig:
                    text.text = "PLACE YOUR ORIGIN" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: continue";
                    break;
                case Stage.m3rotate:
                    text.text = "ROTATE ORIGIN" + "\n" +
                        "Touchpad: rotate" + "\n" +
                        "(swipe clockwise/anticlockwise around touchpad) " + "\n" +
                        "Trigger: continue";
                    break;
                case Stage.m3poc:
                    text.text = "PLACE POINT OF CONCURRENCY" + "\n" +
                        "Touchpad: adjust beam length" + "\n" +
                        "Trigger: place";
                    break;
                case Stage.m3v1p1:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 1 TAIL" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place tail" + "\n";
                    }
                    else
                    {
                        text.text = "PLACE VECTOR 1 POINT" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place point" + "\n";
                    }
                    break;
                case Stage.m3v1p2:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 1 HEAD" + "\n" +
                    "Touchpad: adjust beam length" + "\n" +
                    "Trigger: place head" + "\n";

                    }
                    else
                    {
                        text.text = "SWITCH BETWEEN HEAD/TAIL" + "\n" +
                        "Bumper: toggle head/tail" + "\n" +
                        "Trigger: validate placement" + "\n";
                    }
                    break;
                case Stage.m3v2p1:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 2 TAIL" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place tail" + "\n";
                    }
                    else
                    {
                        text.text = "PLACE VECTOR 2 POINT" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place point" + "\n";
                    }
                    break;
                case Stage.m3v2p2:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 2 HEAD" + "\n" +
                    "Touchpad: adjust beam length" + "\n" +
                    "Trigger: place head" + "\n";

                    }
                    else
                    {
                        text.text = "SWITCH BETWEEN HEAD/TAIL" + "\n" +
                        "Bumper: toggle head/tail" + "\n" +
    "                   Trigger: validate placement" + "\n";
                    }
                    break;
                case Stage.m3v3p1:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 3 TAIL" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place tail" + "\n";
                    }
                    else
                    {
                        text.text = "PLACE VECTOR 3 POINT" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place point" + "\n";
                    }
                    break;
                case Stage.m3v3p2:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 3 HEAD" + "\n" +
                    "Touchpad: adjust beam length" + "\n" +
                    "Trigger: place head" + "\n";

                    }
                    else
                    {
                        text.text = "SWITCH BETWEEN HEAD/TAIL" + "\n" +
                        "Bumper: toggle head/tail" + "\n" +
    "                   Trigger: validate placement" + "\n";
                    }
                    break;
                case Stage.m3v4p1:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 4 TAIL" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place tail" + "\n";
                    }
                    else
                    {
                        text.text = "PLACE VECTOR 4 POINT" + "\n" +
                            "Touchpad: adjust beam length" + "\n" +
                            "Trigger: place point" + "\n";
                    }
                    break;
                case Stage.m3v4p2:
                    if (SceneManager.GetActiveScene().buildIndex == 12)
                    {
                        text.text = "PLACE VECTOR 4 HEAD" + "\n" +
                    "Touchpad: adjust beam length" + "\n" +
                    "Trigger: place head" + "\n";

                    }
                    else
                    {
                        text.text = "SWITCH BETWEEN HEAD/TAIL" + "\n" +
                        "Bumper: toggle head/tail" + "\n" +
    "                   Trigger: validate placement" + "\n";
                    }
                    break;
                case Stage.m3forcesel:
                        text.text = "SELECT A VECTOR TO INPUT ITS CALCULATED FORCE VALUE BY HOVERING OVER ITS NAME LABEL" + "\n"
                         + "Touchpad: adjust beam length" + "\n"
                         + "Trigger: select name label" + "\n";
                    break;
                case Stage.m3keypad:
                    text.text = "INPUT FORCE VALUE" + "\n"
                        + "Trigger: select number" + "\n"; ;
                    break;
                case Stage.m3view:
                    text.text = "Viewing Vector " + GLOBALS.SelectedVec.name.Substring(12).ToString() +"\n"
                    + "Trigger: see force system definition" + "\n";
                    break;
                case Stage.m3forceview:
                    text.text = "SEE CALCULATIONS CANVAS FOR FORCE SYSTEM DEFINITION" + "\n"
                       + "Trigger: see system of equations" + "\n"; ;
                    break;
                case Stage.m3forcesys:
                    text.text = "SEE CALCULATIONS FOR SYSTEM OF EQUATIONS" + "\n"
                       + "Trigger: see correct force values" + "\n"; ;
                    break;
                case Stage.m3validateview:
                text.text = "SEE CALCULATIONS CANVAS FOR FORCE SYSTEM VALIDATION\n"; ;
                    break;
                case Stage.opSel:
                    text.text = "";
                    break;

                default:
                    text.text = "UNKNOWN STAGE";
                    return;
            }
        
    }


    /*  ADDING AUDIO:
     *  The VIS audio files are in a .zip on Box and Basecamp
     *  Download them and add to the project in a folder called "Resources"
     *
     *  Then load the correct clip:
     *  clip = Resources.Load("addition") as AudioClip;
     *
     *  And play the clip:
     *  audioSource.PlayOneShot(clip);
     *
     *  In the case of needing to play multiple clips sequentially, the best option
     *  is to combine the clips into a single file before loading in Unity.
     *
     *  Software such as Audacity can easily add multiple files in order then export a new .mp3 to use
     */
    public void PlayAudio()
    {
        if (GLOBALS.soundOn)
        {
            switch (GLOBALS.stage)
            {
                case Stage.m1orig:

                    break;

                case Stage.m1rotate:

                    break;
                case Stage.m1vector:

                    break;
                case Stage.m1view:

                    break;
                // note Module 1 vs. Module 2
                case Stage.m2orig:

                    break;
                case Stage.m2rotate:

                    break;
                case Stage.v1p1:
                    clip = Resources.Load("place_the_tail_of_vector_1") as AudioClip;
                    audioSource.PlayOneShot(clip);
                    break;
                case Stage.v1p2:
                    clip = Resources.Load("place_the_head_of_vector_1") as AudioClip;
                    audioSource.PlayOneShot(clip);
                    break;
                case Stage.v1calc:
                    clip = Resources.Load("click_to_show_component_calculation") as AudioClip;
                    audioSource.PlayOneShot(clip);

                    break;
                case Stage.v2p1:
                    clip = Resources.Load("place_the_tail_of_vector_2") as AudioClip;
                    audioSource.PlayOneShot(clip);

                    break;
                case Stage.v2p2:
                    clip = Resources.Load("place_the_head_of_vector_2") as AudioClip;
                    audioSource.PlayOneShot(clip);

                    break;
                case Stage.v2calc:
                    clip = Resources.Load("click_to_show_component_calculation") as AudioClip;
                    audioSource.PlayOneShot(clip);

                    break;
                case Stage.m3orig:
                    break;
                case Stage.m3rotate:
                    break;
                case Stage.m3poc:
                    break;
                case Stage.opSel:
                    clip = Resources.Load("select_an_opertation") as AudioClip;
                    audioSource.PlayOneShot(clip);
                    break;
                case Stage.opView:

                    break;

                default:
                    // nothing happens here
                    return;
            }
        }
    }
}
