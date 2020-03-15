using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraHandler : MonoBehaviour
{
    private Player myPlayer;
    public Camera myCamera;
    // Start is called before the first frame update
    public void HandleCamera()
    {
        myPlayer = this.gameObject.GetComponent<PhotonView>().Owner;
        if (!this.gameObject.GetComponent<PhotonView>().IsMine)
        {
            print("NotMine!");
            myCamera.gameObject.SetActive(false);
        }
    }
}
