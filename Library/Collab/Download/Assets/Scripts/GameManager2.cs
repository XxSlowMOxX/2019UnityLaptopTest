using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager2 : MonoBehaviourPunCallbacks
{
    public Dictionary<Player, PlayerStats> statsDict = new Dictionary<Player, PlayerStats>();

    void Awake()
    {
        
        print("Woke AF");
        if (PhotonNetwork.IsMasterClient)
        {
            foreach (Player loopPlayer in PhotonNetwork.PlayerList)
            {
                GameObject loopObject = PhotonNetwork.Instantiate("CubePrefab", Vector3.zero, Quaternion.identity);
                loopObject.name = loopPlayer.NickName + " Spartan";
                loopObject.GetComponent<PhotonView>().TransferOwnership(loopPlayer);
                loopObject.GetComponent<CubeMovement>().debugArrow.GetComponent<PhotonView>().TransferOwnership(loopPlayer);
                //loopObject.GetComponent<CameraHandler>().HandleCamera();
                //playerObjects.Add(loopPlayer, loopObject);
                PlayerStats loopStats = new PlayerStats();
                loopStats.playerObject = loopObject;
                statsDict.Add(loopPlayer, loopStats);
            }
        }
        else //Offline Mode for Editor Testing
        {            
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
    public List<RespawnPoint> GetRespawnPoints(Player deadPlayer)
    {
        RespawnPoint[] allPoints = FindObjectsOfType<RespawnPoint>();
        List<RespawnPoint> validPoints = new List<RespawnPoint>();
        foreach(RespawnPoint point in allPoints)
        {
            if(point.pointTeam == statsDict[deadPlayer].myTeam)
            {
                validPoints.Add(point);
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

