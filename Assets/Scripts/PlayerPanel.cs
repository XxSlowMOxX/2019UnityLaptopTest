using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Button kickButton;
    private Player representedPlayer;
    Launcher myLauncher;

    public void Initialize(string playerName, bool master, Player thisPlayer, Launcher sendingLauncher)
    {
        nameText.text = playerName;
        representedPlayer = thisPlayer;
        myLauncher = sendingLauncher;
        if (!master)
        {
            kickButton.gameObject.SetActive(false);
        }
        if(thisPlayer == PhotonNetwork.LocalPlayer)
        {
            kickButton.gameObject.SetActive(false);
            
        }
    }

    public void KickCall()
    {
        myLauncher.KickPlayer(representedPlayer);
    }
}
