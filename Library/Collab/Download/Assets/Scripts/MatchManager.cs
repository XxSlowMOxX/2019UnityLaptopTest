using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MatchManager : MonoBehaviourPunCallbacks
{
    Dictionary<Player, PlayerStats> statDict;
    List<Player> dbList;
    RespawnPoint[] respawnPoints;
    Dictionary<Player, GameObject> playerObjects;

    void Awake()
    {

        print("Woke AF");
        if (PhotonNetwork.IsMasterClient)
        {
            int i = 0;
            foreach (Player loopPlayer in PhotonNetwork.PlayerList)
            { 
                GameObject loopObject = PhotonNetwork.Instantiate("CubePrefab", Vector3.zero, Quaternion.identity);
                loopObject.name = loopPlayer.NickName + " Spartan";
                loopObject.GetComponent<PhotonView>().TransferOwnership(loopPlayer);
                loopObject.GetComponent<CubeMovement>().debugArrow.GetComponent<PhotonView>().TransferOwnership(loopPlayer);
                //loopObject.GetComponent<CameraHandler>().HandleCamera();
                playerObjects.Add(loopPlayer, loopObject);
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
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.Destroy(playerObjects[otherPlayer]);
    }

    public RespawnPoint[] evalSpawnPoints(Player evalPlayer)
    {
        print(evalPlayer);
        respawnPoints = FindObjectsOfType<RespawnPoint>();
        RespawnPoint[] teamSpawnPoints = null;
        foreach(RespawnPoint point in respawnPoints)
        {
            if (point.pointTeam == statDict[evalPlayer].myTeam)
            {
                teamSpawnPoints[teamSpawnPoints.Length] = point;
            }
        }
        return teamSpawnPoints;
    }

}

public enum playerTeam { red, blue};

/* public class PlayerStats
{
    public string playerName;
    public playerTeam myTeam;
    public int playerKills;
    public int playerDeaths;
    public int shotsFired;
} */
