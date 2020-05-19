using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCaller : MonoBehaviour
{
    public CubeMovement mySpartan;
    public RespawnPoint myPoint;
    // Start is called before the first frame update
    
    public void RespawnCall()
    {
        mySpartan.SpartanRespawn(myPoint);
    }
}
