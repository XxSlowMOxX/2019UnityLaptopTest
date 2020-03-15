using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public string RoomName = "NewRoom#1";
    public GameObject playerObject;
    string playerName = "Player#";
    public string gameVersion = "0.1.6";
    [SerializeField]
    private GameObject playerPanelPrefab;
    public Canvas mainCanvas;
    public Player[] playersInCurrentRoom;
    public GameObject[] playerPanels;
    public Text offlineModeText;
    [Header("Inputs for the Room to create/join")]
    public InputField nameInput;
    public int maxPlayers = 16;
    // Start is called before the first frame update
    void Awake()
    {
        if (PlayerPrefs.HasKey("nickname")) // Handle Nicknames as PlayerPref
        {
            nameInput.text = PlayerPrefs.GetString("nickname");
        }
        else
        {
            playerName = "Player#" + UnityEngine.Random.Range(0, 100000);
            PlayerPrefs.SetString("nickname", playerName);
            print(playerName + " is Player Name");
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
        print(PhotonNetwork.GetPing());
    }

    void Update()
    {
        offlineModeText.gameObject.SetActive(PhotonNetwork.OfflineMode);
    }

    public void Connect()
    {
        print("Trying to Ceonnect...");
        
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
            
        }
        else
        {
            print("Not Yet Connected...");
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void switchOnlineOfflineMode()
    {
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.OfflineMode = false;
            Awake();            
        }
        else
        {
            Disconnect();
        }
    }

    public void ChangeRoomName(string Name)
    {
        RoomName = Name;
    }
    public void ChangeNickName(string Name)
    {
        playerName = Name;
        PhotonNetwork.NickName = Name;
        PlayerPrefs.SetString("nickname", playerName);
    }

    GameObject SpawnPlayerObject(Player spawningPlayer)
    {
        GameObject spawnedObject = PhotonNetwork.Instantiate("CubePrefab", new Vector3(0, 0, 0), Quaternion.identity);
        spawnedObject.GetComponent<PhotonView>().TransferOwnership(spawningPlayer);
        return spawnedObject;
        
    }
    
    void UpdateCurrentPlayers()
    {        
        if (PhotonNetwork.InRoom)
        {
            playersInCurrentRoom = PhotonNetwork.PlayerList;
        }
        else
        {
            foreach (GameObject loopObject in playerPanels)
            {
                Destroy(loopObject);
            }
            //playerPanels = null;
            return;
        }
        foreach (GameObject loopObject in playerPanels)
        {
            Destroy(loopObject);
        }
        //playerPanels = null;
        List<GameObject> playerPanelList = new List<GameObject>();
        int i = 0;
        foreach (Player loopPlayer in playersInCurrentRoom)
        {
            i++;
            GameObject loopPlayerPanel = Instantiate(playerPanelPrefab);
            loopPlayerPanel.transform.parent = mainCanvas.transform.GetChild(5).GetChild(1).transform;
            loopPlayerPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2((i-1)*120, 0);
            loopPlayerPanel.GetComponent<PlayerPanel>().Initialize(loopPlayer.NickName, PhotonNetwork.IsMasterClient, loopPlayer, this);
            playerPanelList.Add(loopPlayerPanel);
        }
        playerPanels = playerPanelList.ToArray();
    }
    public void LevelLoadCall()
    {
        if(PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("TestArea");
        }
        else
        {
            print("Either not in Lobby or not the Master");
        }
        
    }
    public void KickPlayer(Player kickPlayer)
    {
        print("Player " + kickPlayer.NickName + " has been kicked from the Room");
        PhotonNetwork.CloseConnection(kickPlayer);
        UpdateCurrentPlayers();
    }
    public void JoinRandRoom()
    {
        print("Connecting to Random Room");
        PhotonNetwork.JoinRandomRoom();
    }
    public void CreateNewRoom()
    {
        PhotonNetwork.CreateRoom(RoomName, new RoomOptions { MaxPlayers = (byte)maxPlayers });
    }
    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        UpdateCurrentPlayers();
    }

    #region Overrides
    public override void OnConnectedToMaster()
    {
        print("Connected to Server");
    }
    public override void OnJoinedRoom()
    {
        print("Joined Room: " + PhotonNetwork.CurrentRoom);
        UpdateCurrentPlayers();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        print("COuld not join Random ROom");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Could not join Room");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Could Not Create ROom");
    }
    public override void OnCreatedRoom()
    {
       
        print("Created Room");
        //SpawnPlayerObject(PhotonNetwork.LocalPlayer);
        UpdateCurrentPlayers();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        print(newPlayer.NickName + " joined the Lobby");
        //SpawnPlayerObject(newPlayer);
        UpdateCurrentPlayers();
    }
    public override void OnCustomAuthenticationFailed(string debugMessage)
    {
        print("CustomAuth failed");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        print(otherPlayer.NickName + " has left the Room");
        UpdateCurrentPlayers();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnect because of: " + cause);
        if(cause == DisconnectCause.ClientTimeout || cause == DisconnectCause.DisconnectByClientLogic)
        {
            PhotonNetwork.OfflineMode = true;
        }
    }
    #endregion
    // Update is called once per frame
}