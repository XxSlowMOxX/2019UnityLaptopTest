using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Scoreboard : MonoBehaviour
{
    private GameManager2 gameManager;
    public GameObject scorePanel;
    Dictionary<Player, ScoreboardPointer> scObjects = new Dictionary<Player, ScoreboardPointer>();

    public void ShowScoreboard(bool showBool)
    {
        scorePanel.SetActive(showBool);
    }
    public void Init()
    {
        gameManager = FindObjectOfType<GameManager2>();
        foreach(Player loopPlayer in PhotonNetwork.PlayerList)
        {
            
        }
    }
}
