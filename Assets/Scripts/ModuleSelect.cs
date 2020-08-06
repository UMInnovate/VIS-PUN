using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

/*  ModuleSelect is on the Canvas in Scene 1ModSelection
 *  Buttons launch the instruction scene for selected module
 */

public class ModuleSelect : MonoBehaviour
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
                LaunchMod1();
                break;
            case MLInputControllerTouchpadGestureDirection.Right:
                LaunchMod2();
                break;
        }
    }

    //Button:Module1.OnClick()
    private void LaunchMod1()
    {
        SceneManager.LoadScene(4, LoadSceneMode.Single);
    }

    //Button:Module2.OnClick()
    private void LaunchMod2()
    {
        SceneManager.LoadScene(6, LoadSceneMode.Single);
    }

}
