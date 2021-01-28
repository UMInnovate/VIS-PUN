using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    // room info
    public static PhotonRoom room;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    // player info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playerInGame;

    public bool bCanLoadMod2 = false;

    // my player @@@
    [SerializeField]
    private myPlayer myPlayerRef;
    [SerializeField]
    private StorableObjectBin storableObjectRef;

    private void Awake()
    {
        if(PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if(PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;

            }
        }
        DontDestroyOnLoad(this.gameObject); //!!! mod 1 vs 2?
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();        
    }

    // sets player data when join the room
    public override void OnJoinedRoom() // automatically called when player has joined a room 
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        photonPlayers = PhotonNetwork.PlayerList;
        myNumberInRoom = playersInRoom;
        PhotonNetwork.NickName = myNumberInRoom.ToString();

        // when player joins room, store his actor number in a public variable @@@
        string _myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        myPlayerRef.GetMyPlayerActorNumber(_myActorNumber);
        Debug.Log("My unique ID is: " + _myActorNumber);

        StartGame();
    }

    // gets called whenever a new player joins the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player has joined the room");
        playersInRoom++;

        // display the player's unique ID
        string _uniqueID = newPlayer.ActorNumber.ToString();
        Debug.Log(_uniqueID + " has entered the room");
    }

    // gets called whenever a player leaves the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        string _uniqueID = otherPlayer.ActorNumber.ToString();
        Debug.Log(_uniqueID + " has left the room");


        storableObjectRef.RemoveStorableObject_RPC(_uniqueID); //@@@ sends the ID of the player who left & clears their stuff

    }

    void StartGame()
    {
        isGameLoaded = true;
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        bCanLoadMod2 = true;
        //PhotonNetwork.LoadLevel(MultiplayerSetting.multiplayerSetting.multiplayerScene);
    }

    // leaving the scene
    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == MultiplayerSetting.multiplayerSetting.multiplayerScene)
        {
            isGameLoaded = true;

            //RPC_CreatePlayer();
        }
    }

    [PunRPC]
    public void RPC_CreatePlayer()
    {
        Debug.Log("Instantiating player");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }
}
