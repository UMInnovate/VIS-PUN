using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;
    RoomOptions _roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSetting.multiplayerSetting.maxPlayers };


    //Possibly count from 1-10 so that there can be multiple mod 1 rooms. Add number to the end of room name. Ex: Mod1Room2
    private int mod1roomnum;

    private void Awake()
    {
        lobby = this;
    }
             
    void Start()
    {
        //In case start is called when a new scene is loaded
        //if(!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings(); // connects to Master photon server (under Photon Service Settings, App Version = 0)      
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are connected to the Photon Master server");
     // DEBUG TEST SCENE   RoomHandler(); 
        //PhotonNetwork.AutomaticallySyncScene = true; // when master joins a scene, all players connected to master client will load same scene***
        //JoinRandomRoom(); // join random room
        //RoomHandler();
    }

    /*
    Room Handler summary
    If a multiplayer session has been selected by the user, they must join a photon room corresponding to the module of interest
    If a room for the module selected doesn't exist, a new one will be created
    Possible issues include the room reaching its max amount of players
    In this case, the user would join a random room after OnJoinRoomFailed and OnCreateRoomFailed are called
    */
    public void RoomHandler()
    {
        Debug.Log("Handling Room Assignment");

        //If the user has not connected to the master server yet, Room Handler will be called until user is connected
        //This might not be the best handling of the issue since the boolean will be true while switching servers
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("Previously was not connected to server but is now");
            RoomHandler();
        }

        else
        {

            if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 8)
            {
                Debug.Log("In if statement for Mod 1");
                Debug.Log("Is player in a room already: " + PhotonNetwork.InRoom);
                Debug.Log("Build index was " + SceneManager.GetActiveScene().buildIndex);
                PhotonNetwork.JoinOrCreateRoom("Mod1Room", _roomOptions, TypedLobby.Default);
                //JoinRandomRoom();
            }

            else if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 10)
            {
                Debug.Log("In if statement for Mod 2");
                Debug.Log("Is player in a room already: " + PhotonNetwork.InRoom);
                Debug.Log("Build index was " + SceneManager.GetActiveScene().buildIndex);
                PhotonNetwork.JoinOrCreateRoom("Mod2Room", _roomOptions, TypedLobby.Default);
            }

            else if (SceneManager.GetActiveScene().buildIndex == 13 || SceneManager.GetActiveScene().buildIndex == 14)
            {
                Debug.Log("In if statement for Mod 3");
                Debug.Log("Is player in a room already: " + PhotonNetwork.InRoom);
                Debug.Log("Build index was " + SceneManager.GetActiveScene().buildIndex);
                PhotonNetwork.JoinOrCreateRoom("Mod3Room", _roomOptions, TypedLobby.Default);
            }
            else
                Debug.Log("Failed to Join a room");
           // PhotonNetwork.JoinOrCreateRoom("OtherRoom", _roomOptions, TypedLobby.Default); }
        }
    }

    public void JoinRandomRoom()
    {
        Debug.Log("Join random room");
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        string _roomName = "";

        Debug.Log("Tried to join a non-random game but failed. There must be no open games available.");

        if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 8)
            _roomName = "Mod1Room";

        else if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 10)
            _roomName = "Mod2Room";

        else if (SceneManager.GetActiveScene().buildIndex == 13 || SceneManager.GetActiveScene().buildIndex == 14)
            _roomName = "Mod1Room";
    //    else
    //        _roomName = "OtherRoom";

        CreateRoom(_roomName); // create a new room
    }

    void CreateRoom(string _roomName)
    {
        PhotonNetwork.CreateRoom(_roomName, _roomOptions);
        Debug.Log("New Room Created: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError("Tried to join a random game but failed. There must be no open games available.");
        CreateRandomRoom(); // create a new room
    }


    void CreateRandomRoom() // creates a new room
    {
        int _randomRoomName = Random.Range(0, 10000);
        PhotonNetwork.CreateRoom("Room" + _randomRoomName, _roomOptions);
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom() // automatically called when player has joined a room 
    {
        Debug.Log("We are now in a room");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Tried to create a new room but failed, there must already be a room with the same name.");
        CreateRandomRoom();
    }

    public void OnCancelButtonClicked()
    {
        PhotonNetwork.LeaveRoom(); // on application quit***, maybe on Restart and Back To Start?
    }

    void Update()
    {
        
    }
}
