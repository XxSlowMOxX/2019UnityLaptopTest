using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeSwitcher : MonoBehaviour
{
    public enum playerMode { Spartan, Commander};
    public playerMode currentMode = playerMode.Spartan;
    private CubeMovement thisSpartan;
    private CommandScript thisCommander;


    void Awake()
    {
        thisSpartan = this.GetComponent<CubeMovement>();
    }
    // Update is called once per frame
    public void switchMode()
    {
        if(currentMode == playerMode.Spartan)
        {
            thisSpartan.enabled = false;
            thisCommander.enabled = true;
            currentMode = playerMode.Commander;
        }
        else
        {
            thisSpartan.enabled = true;
            thisCommander.enabled = false;
            currentMode = playerMode.Spartan;
        }
    }
}
