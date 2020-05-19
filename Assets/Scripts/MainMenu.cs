using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Chat;
using UnityEngine.UI;
using SFB;
using ExitGames.Client.Photon;

public class MainMenu : MonoBehaviourPunCallbacks, IChatClientListener
{
    private List<RoomInfo> rooms;
    Dictionary<Player, GameObject> playerRepDict = new Dictionary<Player, GameObject>();
    Dictionary<Player, int> teamDict = new Dictionary<Player, int>();
    private Texture myImage;
    private ChatClient chatClient;
    private playerTeam myTeam;
    [Header("GameObjects")]
    public GameObject eventHandler;
    public GameObject playerTeamObject;
    public InputField nicknameField;
    public GameObject loginPanel;
    public GameObject loginCircle;
    public GameObject mainPanel;
    public GameObject creditsPanel;
    public GameObject optionsPanel;
    public GameObject multiplayerPanel;
    public GameObject hostPanel;
    public GameObject roomPanel;
    public GameObject playerRepPrefab;
    public GameObject playerRepHolder;    
    public InputField serverNameField;
    public InputField chatInput;
    public ScrollRect chatScroll;
    public GameObject chatContent;
    public Dropdown teamDropDown;
    public Slider maxPlayerSlider;
    public Toggle visibleBox;
    public Text maxPlayersText;
    public Text roomNameText;
    public Text roomPlayersText;
    public Image currentMapImage;
    public Text currentMapName;
    public RawImage outimg;
    [Header("Maps")]
    public List<Map> mapList = new List<Map>();
    private int selectedMapIndex = 0;


    void Awake()
    {
        UIBackCall(loginPanel);
        if (PlayerPrefs.HasKey("nickname")) // Nickname Loading from Player Prefs
        {
            nicknameField.text = PlayerPrefs.GetString("nickname");
        }
        Texture2D texture = ReadTextureFromPlayerPrefs("playerAvatar");
        if(texture != null)
        {
            outimg.texture = texture;
        }
        
    }

    void FixedUpdate()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
    }
    void Update()
    {
        if (PhotonNetwork.InRoom && Input.GetKey(KeyCode.Return) && chatInput.text != "")
        {
            chatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, chatInput.text);
            chatInput.text = null;
        }
    }
    public void UpdateNickNamePref() //Updates Player Pref of Nickname
    {
        PlayerPrefs.SetString("nickname", nicknameField.text);
    }
    public void MasterConnect()
    {
        UpdateNickNamePref();
        loginCircle.SetActive(true);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = nicknameField.text;
        PhotonNetwork.ConnectUsingSettings();
        chatClient = new ChatClient(this);
        chatClient.ChatRegion = "EU";
        chatClient.Connect("5b0f0924-1365-46dd-9c35-47867b1ff592", "1", new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));
    }
    public void UISingleplayerCall()
    {
        print("Singleplayer");
    }
    public void UIMultiplayerCall()
    {
        PhotonNetwork.JoinLobby();        
    }    
    public void UIExitCall()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.ExitPlaymode();
    }
    public void UIBackCall(GameObject backObject)
    {
        mainPanel.SetActive(false);
        creditsPanel.SetActive(false);
        loginPanel.SetActive(false);
        optionsPanel.SetActive(false);
        multiplayerPanel.SetActive(false);
        hostPanel.SetActive(false);
        roomPanel.SetActive(false);
        backObject.SetActive(true);
    }
    public void HostGame()
    {
        if (!ServerNameAvailable(serverNameField.text))
        {
            return; //Falls Name schon vergeben ist
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)maxPlayerSlider.value; //TODO Player Prefs
        roomOptions.IsVisible = visibleBox.isOn;
        PhotonNetwork.CreateRoom(serverNameField.text, roomOptions);
    }
    public void JoinRandRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    bool ServerNameAvailable(string serverName)
    {
        foreach (RoomInfo loopRoom in rooms)
        {
            if (serverName == loopRoom.Name)
            {
                return false;
            }
        }
        return true;
    }
    public void MaxPlayersChanged()
    {
        maxPlayersText.text = maxPlayerSlider.value.ToString();
    }
    public void MapChanged(int newIndex)
    {
        selectedMapIndex = newIndex;
        currentMapImage.sprite = mapList[newIndex].mapImage;
        currentMapName.text = mapList[newIndex].name;
    }
    public void ChangeMap(int change)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (selectedMapIndex + change > mapList.Count-1)
            {
                eventHandler.GetComponent<MenuHandler>().MapIndexChangeCall(0);
            } else if(selectedMapIndex + change < 0)
            {
                eventHandler.GetComponent<MenuHandler>().MapIndexChangeCall(mapList.Count -1);
            }
            else
            {
                eventHandler.GetComponent<MenuHandler>().MapIndexChangeCall(selectedMapIndex + change);
            }
        }
    }
    public void OnNewImage()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "png", false);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
    public static void WriteTextureToPlayerPrefs(string tag, Texture2D tex)
    {
        byte[] texByte = tex.EncodeToPNG();
        string base64Tex = System.Convert.ToBase64String(texByte);
        PlayerPrefs.SetString(tag, base64Tex);
        PlayerPrefs.Save();
    }
    public static Texture2D ReadTextureFromPlayerPrefs(string tag)
    {
        string base64Tex = PlayerPrefs.GetString(tag, null);        
        if (!string.IsNullOrEmpty(base64Tex))
        {
            byte[] texByte = System.Convert.FromBase64String(base64Tex);
            Texture2D tex = new Texture2D(100, 100);
            if (tex.LoadImage(texByte))
            {
                return tex;
            }
        }
        return null;
    }
    public void GetPlayerTexture(string name, string base64text)
    {
        if (!string.IsNullOrEmpty(base64text))
        {
            byte[] texByte = System.Convert.FromBase64String(base64text);
            Texture2D tex = new Texture2D(100, 100);
            if (tex.LoadImage(texByte))
            {
                foreach (Player loopPlayer in PhotonNetwork.PlayerList)
                {
                    if (loopPlayer.NickName == name)
                    {
                        playerRepDict[loopPlayer].GetComponentInChildren<RawImage>().texture = tex;
                    }
                }
            }
        }
        
    }
    public void SendPicCaller()
    {
        string base64Tex = PlayerPrefs.GetString("playerAvatar", null);
        if (!string.IsNullOrEmpty(base64Tex))
        {
            print("Naas");
            eventHandler.GetComponent<MenuHandler>().SendPicCall(base64Tex);            
        }
    }
    public void LoadLevelCall()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DontDestroyOnLoad(playerTeamObject);            
            foreach(Player loopPlayer in PhotonNetwork.PlayerList)
            {
                if(teamDict[loopPlayer] == 0)
                {
                    playerTeamObject.GetComponent<PlayerTeamDict>().teamDictionary.Add(loopPlayer, playerTeam.red);
                }
                else
                {
                    playerTeamObject.GetComponent<PlayerTeamDict>().teamDictionary.Add(loopPlayer, playerTeam.blue);
                }
            }
            print(playerTeamObject.GetComponent<PlayerTeamDict>().teamDictionary.ToStringFull());    
            PhotonNetwork.LoadLevel(mapList[selectedMapIndex].filename);
        }
    }
    public void DisconnectCall()
    {
        playerRepDict = new Dictionary<Player, GameObject>();
        PhotonNetwork.LeaveRoom();
        roomPanel.SetActive(false);
    }
    public void TeamChange(string playerName, int teamIndex)
    {
        foreach(Player loopPlayer in PhotonNetwork.PlayerList)
        {
            if (loopPlayer.NickName == playerName)
            {
                if (teamDict.ContainsKey(loopPlayer))
                {
                    teamDict[loopPlayer] = teamIndex;
                }
                else
                {
                    teamDict.Add(loopPlayer, teamIndex);
                }                
                if(teamIndex == 0)
                {
                    playerRepDict[loopPlayer].GetComponentInChildren<Image>().color = Color.red;
                }
                else
                {
                    playerRepDict[loopPlayer].GetComponentInChildren<Image>().color = Color.blue;
                }
            }
        }
    }

    #region overrides
    public override void OnConnectedToMaster()
    {
        loginPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("RoomListUpdate");
        rooms = roomList;
    }
    public override void OnJoinedLobby()
    {        
        print("Joined Lobby");
        UIBackCall(multiplayerPanel);
    }
    public override void OnCreatedRoom()
    {
        eventHandler.GetComponent<MenuHandler>().MapIndexChangeCall(0);
        print("RoomCreated");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError("Could not create Room. Error Message: " + message);
    }
    public override void OnJoinedRoom()
    {
        UIBackCall(roomPanel);
        roomPlayersText.text = PhotonNetwork.CurrentRoom.Players.ToStringFull();
        eventHandler.GetComponent<PhotonView>().RequestOwnership();
        int i = 0;
        foreach(Player loopPlayer in PhotonNetwork.PlayerList)
        {
            GameObject loopObject = (GameObject)Instantiate(playerRepPrefab);
            loopObject.GetComponentInChildren<Text>().text = loopPlayer.NickName;
            loopObject.transform.SetParent(playerRepHolder.transform, false);
            loopObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 100, 0);
            i++;
            playerRepDict.Add(loopPlayer, loopObject);
        }
        SendPicCaller();
        eventHandler.GetComponent<MenuHandler>().TeamChangeCall(0);
        chatClient.Subscribe(new string[] { PhotonNetwork.CurrentRoom.Name });
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            eventHandler.GetComponent<MenuHandler>().MapIndexChangeCall(selectedMapIndex);
        }
        roomPlayersText.text = PhotonNetwork.CurrentRoom.Players.ToStringFull();
        GameObject loopObject = (GameObject)Instantiate(playerRepPrefab);
        loopObject.GetComponentInChildren<Text>().text = newPlayer.NickName;
        loopObject.transform.SetParent(playerRepHolder.transform, false);
        loopObject.GetComponent<RectTransform>().anchoredPosition = new Vector2((PhotonNetwork.PlayerList.Length - 1) * 100, 0);
        playerRepDict.Add(newPlayer, loopObject);
        SendPicCaller();
        eventHandler.GetComponent<MenuHandler>().TeamChangeCall(0); 
        chatContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 16);
        chatContent.GetComponent<Text>().text += newPlayer.NickName + " entered the Room\n";
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPlayersText.text = PhotonNetwork.CurrentRoom.Players.ToStringFull();
        Destroy(playerRepDict[otherPlayer]);
        playerRepDict.Remove(otherPlayer);
        int i = 0;
        foreach(Player loopPlayer in PhotonNetwork.PlayerList)
        {
            playerRepDict[loopPlayer].GetComponent<RectTransform>().anchoredPosition = new Vector2(i * 100, 0);
            i++;
        }

        chatContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 16);
        chatContent.GetComponent<Text>().text += otherPlayer.NickName + " left the Room\n";
    }
    #endregion
    #region enums
    private IEnumerator OutputRoutine(string url)
    {
        var loader = new WWW(url);
        yield return loader;
        Texture2D textur = loader.texture;
        TextureScale.Bilinear(textur, 100, 100);
        outimg.texture = textur;
        myImage = textur;
        WriteTextureToPlayerPrefs("playerAvatar", textur);
    }
    #endregion
    #region Chat Handles
    public void DebugReturn(DebugLevel level, string message)
    {
        print("<ChatDebug> " + message);
    }

    public void OnDisconnected()
    {
        print("<ChatDebug> Disconnected");
    }

    public void OnChatStateChange(ChatState state)
    {
        print("<ChatDebug> Chat State Changed: " + state.ToString());
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        print(senders.ToStringFull());
        print(messages.ToStringFull());
        if(channelName == PhotonNetwork.CurrentRoom.Name)
        {
            int i = 0;
            foreach(string message in messages)
            {
                if(message != "")
                {
                    chatContent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 16);
                    chatContent.GetComponent<Text>().text += "<" + senders[i] + ">: " + message + "\n";
                }
                i++;
            }
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {

        print("<ChatDebug> Youve got private Mail");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        print("<ChatDebug> Channel Subscription: " + results.ToString());
    }

    public void OnUnsubscribed(string[] channels)
    {
        print("<ChatDebug> Unsubcribed");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        print("<ChatDebug> StatusUpdate");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        print("<ChatDebug> User " + user + " joined " + channel);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        print("<ChatDebug> User " + user + " left " + channel);
    }
    #endregion
}

[System.Serializable]
public struct Map
{
    public Map(string Name, string FName, Sprite textu)
    {
        name = Name;
        filename = FName;
        mapImage = textu;
    }
    [SerializeField]
    public string name;
    [SerializeField]
    public string filename;
    [SerializeField]
    public Sprite mapImage;

}

