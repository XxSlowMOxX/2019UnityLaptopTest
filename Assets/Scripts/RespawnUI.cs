using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnUI : MonoBehaviour
{
    public GameObject scrollViewport;
    [SerializeField]
    private GameObject UISpawnPoint;
    private CubeMovement mySpartan;
    private RespawnPoint[] spawnPoints;

    public void Initialize(RespawnPoint[] initPoints, CubeMovement sender)
    {
        mySpartan = sender;
        foreach (RespawnPoint point in initPoints)
        {
            GameObject loopPanel = Instantiate(UISpawnPoint);
            loopPanel.transform.SetParent(scrollViewport.transform, false);
        }
    }
}
