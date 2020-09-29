using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

/*  ModuleSelect is on the Canvas in Scene 1ModSelection
 *  Buttons launch the instruction scene for selected module
 */

public class ModuleSelect2: MonoBehaviour
{
    private MLInputController _controller = null;

    private void Start()
    {
        if (!MLInput.IsStarted)
            MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);
    }


    private void Update()
    {
        switch (_controller.TouchpadGesture.Direction)
        {
            case MLInputControllerTouchpadGestureDirection.Left:
                LaunchMod2Single();
                break;
            case MLInputControllerTouchpadGestureDirection.Right:
                LaunchMod2Multi();
                break;
        }
    }

    //Button:Module2.OnClick()
    private void LaunchMod2Single()
    {
        SceneManager.LoadScene(8, LoadSceneMode.Single); 
    }

    private void LaunchMod2Multi()
    {
        SceneManager.LoadScene(10, LoadSceneMode.Single);
    }

}
