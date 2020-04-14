using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MenuHandler : MonoBehaviourPun
{
    public MainMenu myMenu;
    void Awake()
    {
        print("Menu Handler instantiated");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            this.GetComponent<PhotonView>().RPC("PrintMessage", RpcTarget.All, "Someone pressed D: " + PhotonNetwork.NickName);
        }
    }
    public void MapIndexChangeCall(int newIndex)
    {
        this.GetComponent<PhotonView>().RPC("MapIndexChange", RpcTarget.All, newIndex);
    }
    public void SendPicCall(string base64Tex)
    {          
        this.GetComponent<PhotonView>().RPC("PlayerImage",RpcTarget.All, PhotonNetwork.NickName, base64Tex);
    }
    public void TeamChangeCall(int teamIndex)
    {
        this.GetComponent<PhotonView>().RPC("TeamChange", RpcTarget.All, PhotonNetwork.NickName, teamIndex);
    }
    #region RPCs
    [PunRPC]
    public void PrintMessage(string message)
    {
        print(message);
    }
    [PunRPC]
    public void MapIndexChange(int newIndex)
    {
        myMenu.MapChanged(newIndex);
    }
    [PunRPC]
    public void PlayerImage(string nickname, string imagestring)
    {
        myMenu.GetPlayerTexture(nickname, imagestring);
    }
    [PunRPC]
    public void TeamChange(string nickname, int newTeam)
    {
        myMenu.TeamChange(nickname, newTeam);
    }
    #endregion
}
