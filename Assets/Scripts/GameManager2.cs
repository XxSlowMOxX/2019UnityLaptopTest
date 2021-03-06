﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager2 : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Dictionary<Player, PlayerStats> statsDict = new Dictionary<Player, PlayerStats>();

    private PlayerTeamDict teamDict;

    void Awake()
    {        
        print("Woke AF");
        if (PhotonNetwork.IsMasterClient)
        {
            teamDict = GameObject.Find("PlayerTeamDict").GetComponent<PlayerTeamDict>();
            foreach (Player loopPlayer in PhotonNetwork.PlayerList)
            {
                GameObject loopObject = PhotonNetwork.Instantiate("CubePrefab", Vector3.zero, Quaternion.identity);
                loopObject.GetComponent<Entity>().name = loopPlayer.NickName;
                loopObject.name = loopPlayer.NickName + " Spartan";
                loopObject.GetComponent<PhotonView>().TransferOwnership(loopPlayer);
                loopObject.GetComponent<CubeMovement>().debugArrow.GetComponent<PhotonView>().TransferOwnership(loopPlayer);
                loopObject.GetComponent<CubeMovement>().TeamChangeCall(teamDict.teamDictionary[loopPlayer].ToString());
                loopObject.GetComponent<CubeMovement>().NameChangeCall(loopPlayer.NickName);
                PlayerStats loopStats = new PlayerStats();
                loopStats.playerObject = loopObject;
                statsDict.Add(loopPlayer, loopStats);
                
                
            }
            this.GetComponent<PhotonView>().RequestOwnership();
        }
        else if (PhotonNetwork.InRoom)
        {
            this.GetComponent<PhotonView>().RequestOwnership();
            foreach(Player loopPlayer in PhotonNetwork.PlayerList)
            {
                PlayerStats loopStats = new PlayerStats();
                statsDict.Add(loopPlayer, loopStats);
            }
        }
        else //Offline Mode for Editor Testing
        {
            print("not Master CLient, creating stats anyway");
            foreach (Player loopPlayer in PhotonNetwork.PlayerList)
            {                
                PlayerStats loopStats = new PlayerStats();
                statsDict.Add(loopPlayer, loopStats);
            }
            if (!PhotonNetwork.IsConnected)
            {
                Application.LoadLevel(0);
                //PhotonNetwork.ConnectUsingSettings();
                GameObject spawnedObject = PhotonNetwork.Instantiate("CubePrefab", Vector3.zero, Quaternion.identity);
                spawnedObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
                //PhotonNetwork.CreateRoom("OfflineRoom");
                //spawnedObject.GetComponent<CubeMovement>().HandleCamera();
            }
        }
    }
    public List<RespawnPoint> GetRespawnPoints(playerTeam respawnTeam)
    {
        RespawnPoint[] allPoints = FindObjectsOfType<RespawnPoint>();
        List<RespawnPoint> validPoints = new List<RespawnPoint>();
        foreach(RespawnPoint point in allPoints)
        {
            if(point.pointTeam == respawnTeam)
            {
                validPoints.Add(point);
                print(point.name);
            }
        }
        return validPoints;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.Destroy(statsDict[otherPlayer].playerObject.GetComponent<PhotonView>());        
    }
}

public class PlayerStats
{
    public string playerName;
    public GameObject playerObject;
    public playerTeam myTeam;
    public int playerKills;
    public int playerDeaths;
    public int shotsFired;    
}

public enum playerTeam
{
    red, blue
}