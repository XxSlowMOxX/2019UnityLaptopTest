using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    [Header("GameObjects")]
    public InputField nicknameField;
    public GameObject loginPanel;
    public GameObject loginCircle;
    public GameObject mainPanel;

    void Awake()
    {
        mainPanel.SetActive(false);
        if (PlayerPrefs.HasKey("nickname")) // Nickname Loading from Player Prefs
        {
            nicknameField.text = PlayerPrefs.GetString("nickname");
        }
        loginPanel.SetActive(true);
    }
    public void MasterConnect()
    {
        loginCircle.SetActive(true);
        PhotonNetwork.NickName = nicknameField.text;
        PhotonNetwork.ConnectUsingSettings();        
    }
    public void ExitCall()
    {
        Application.Quit();
        UnityEditor.EditorApplication.ExitPlaymode();
    }
    #region overrides
    public override void OnConnectedToMaster()
    {
        loginPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    #endregion
}
