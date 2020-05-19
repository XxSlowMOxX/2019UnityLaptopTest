using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUI : MonoBehaviour
{
    public GameObject scrollViewport;
    [SerializeField]
    private GameObject UISpawnPoint;
    private CubeMovement mySpartan;
    private RespawnPoint[] spawnPoints;

    public void Initialize(RespawnPoint[] initPoints, CubeMovement sender)
    {
        RespawnCaller[] oldUI = scrollViewport.GetComponentsInChildren<RespawnCaller>();
        foreach(RespawnCaller oldCaller in oldUI)
        {
            Destroy(oldCaller.gameObject);
        }
        mySpartan = sender;
        foreach (RespawnPoint point in initPoints)
        {
            print(point.gameObject.name);
            GameObject loopPanel = Instantiate(UISpawnPoint);
            loopPanel.GetComponent<RespawnCaller>().myPoint = point;
            loopPanel.GetComponent<RespawnCaller>().mySpartan = sender;
            loopPanel.GetComponentInChildren<Text>().text = point.pointName;
            loopPanel.transform.SetParent(scrollViewport.transform, false);
        }
    }
}
