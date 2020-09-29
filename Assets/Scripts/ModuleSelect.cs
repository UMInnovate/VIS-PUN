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
                LaunchMod1Single();
                break;
            case MLInputControllerTouchpadGestureDirection.Right:
                LaunchMod1Multi();
                break;
        }
    }

    //Button:Module1.OnClick()
    private void LaunchMod1Multi()
    {
        SceneManager.LoadScene(8, LoadSceneMode.Single);
    }

    private void LaunchMod1Single()
    {
        SceneManager.LoadScene(4, LoadSceneMode.Single);
    }

}
