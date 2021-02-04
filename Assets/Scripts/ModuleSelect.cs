using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;

/*  ModuleSelect is on the Canvas in Scene 1ModSelection
 *  Buttons launch the instruction scene for selected module
 */

public class ModuleSelect : MonoBehaviour
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
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    case 2:
                        SceneManager.LoadScene(4, LoadSceneMode.Single);
                        break;
                    case 3:
                        SceneManager.LoadScene(7, LoadSceneMode.Single);
                        break;
                    case 13:
                        SceneManager.LoadScene(12, LoadSceneMode.Single);
                        break;
                }

                break;
            case MLInput.Controller.TouchpadGesture.GestureDirection.Right:
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    case 2:
                        SceneManager.LoadScene(8, LoadSceneMode.Single);
                        break;
                    case 3:
                        SceneManager.LoadScene(11, LoadSceneMode.Single);
                        break;
                    case 13:
                        SceneManager.LoadScene(14, LoadSceneMode.Single);
                        break;
                }

                break;
        }
    }

}