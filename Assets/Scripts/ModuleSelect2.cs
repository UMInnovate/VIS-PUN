using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

/*  ModuleSelect is on the Canvas in Scene 1ModSelection
 *  Buttons launch the instruction scene for selected module
 */

public class ModuleSelect2: MonoBehaviour
{
    private MLInput.Controller _controller = null;

    private void Start()
    {
        if (!MLInput.IsStarted)
            MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);
    }


    private void Update()
    {
        switch (_controller.CurrentTouchpadGesture.Direction)
        {
            case MLInput.Controller.TouchpadGesture.GestureDirection.Left:
                LaunchMod2Single();
                break;
            case MLInput.Controller.TouchpadGesture.GestureDirection.Right:
                LaunchMod2Multi();
                break;
        }
    }

    //Button:Module2.OnClick()
    private void LaunchMod2Single()
    {
        SceneManager.LoadScene(7, LoadSceneMode.Single); 
    }

    private void LaunchMod2Multi()
    {
        SceneManager.LoadScene(11, LoadSceneMode.Single);
    }

}
