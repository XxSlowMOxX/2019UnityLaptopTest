using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class EscMenu : MonoBehaviour
{
    public GameObject escPanel;
    public Text roomText;
    public void ShowEscMenu(bool showBool)
    {
        escPanel.SetActive(showBool);
        RefreshInformation();
    }
    public void DisconnectFromRoom()
    {
        PhotonNetwork.Disconnect();
        Application.LoadLevel(0);
    }
    public void QuitGame()
    {
        Application.Quit();
        print("Mr Stark, i don't feel so good");
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    public void RefreshInformation()
    {
        if (PhotonNetwork.InRoom)
        {
            roomText.text = "Current Room: " + PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            roomText.text = "Currently running in Offline Mode";
        }
    }
}
