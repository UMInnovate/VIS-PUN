using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    private void Awake()
    {
        lobby = this;
    }
             
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // connects to Master photon server (under Photon Service Settings, App Version = 0)      
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("We are connected to the Photon Master server");
        //PhotonNetwork.AutomaticallySyncScene = true; // when master joins a scene, all players connected to master client will load same scene***
        JoinRandomRoom(); // join random room

    }

    public void JoinRandomRoom()
    {       
        PhotonNetwork.JoinRandomRoom();       
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError("Tried to join a random game but failed. There must be no open games available.");
        CreateRoom(); // create a new room
    }

    void CreateRoom() // creates a new room
    {
        int _randomRoomName = Random.Range(0, 10000);
        RoomOptions _roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSetting.multiplayerSetting.maxPlayers};
        PhotonNetwork.CreateRoom("Room" + _randomRoomName, _roomOptions);
    }

    public override void OnJoinedRoom() // automatically called when player has joined a room 
    {
        Debug.Log("We are now in a room");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Tried to create a new room but failed, there must already be a room with the same name.");
        CreateRoom();
    }

    public void OnCancelButtonClicked()
    {
        PhotonNetwork.LeaveRoom(); // on application quit***, maybe on Restart and Back To Start?
    }

    void Update()
    {
        
    }
}
