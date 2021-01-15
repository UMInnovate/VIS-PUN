using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class NextScene : MonoBehaviour
{
    private MLInput.Controller _controller = null;

    void Start()
    {
        if (!MLInput.IsStarted)
            MLInput.Start();
        _controller = MLInput.GetController(MLInput.Hand.Left);
        MLInput.OnTriggerUp += OnTriggerUp;
    }

    private void OnDisable()
    {
        MLInput.OnTriggerUp -= OnTriggerUp;
        MLInput.Stop();
    }

    void OnTriggerUp(byte controllerId, float pressure)
    {
        if (controllerId == _controller.Id)
        {
            //if(SceneManager.GetActiveScene().name == "4Mod2Instructions" && PhotonRoom.room.bCanLoadMod2) // loads players into mod 5
            //{
            //    Debug.Log("Loading multiplayer level");
            //    PhotonNetwork.LoadLevel(MultiplayerSetting.multiplayerSetting.multiplayerScene);
            //}

            //if (SceneManager.GetActiveScene().name == "4Mod2Instructions" && !PhotonRoom.room.bCanLoadMod2) // wait until can load mod
            //{
            //    Debug.Log("Waiting until can load mod...try again");
            //}

            
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                     
        }
    }
}
