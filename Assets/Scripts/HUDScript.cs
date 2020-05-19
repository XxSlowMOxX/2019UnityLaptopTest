using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class HUDScript : MonoBehaviour
{
    Entity thisEntity;
    public Text ammoCount;
    public Text healthText;
    public Text nameText;
    public Image testLicht;
    public Text displayText;
    public Slider healthBar;
    private bool waitFinished = true;
    public bool lightGreen = false;
    public GameObject respawnPanel;

    void Awake()
    {
        thisEntity = this.GetComponentInParent<Entity>();
        ammoCount.text = "No Weapon!";        
    }
    // Update is called once per frame
    void Update()
    {
        if(thisEntity.weaponList.Count > 0)
        {
            ammoCount.text = (thisEntity.weaponList[thisEntity.currentWeaponIndex].currentMag).ToString() + " / " + (thisEntity.weaponList[0].magSize.ToString());
        }
        healthText.text = thisEntity.health.ToString();
        nameText.text = PhotonNetwork.LocalPlayer.NickName;
        if (thisEntity.health <= 100.0f && thisEntity.health >= 0)
        {            
            healthBar.value = thisEntity.health / 100;
        }        
        if (waitFinished)
        {
            displayText.text = "";
        }
        if (lightGreen)
        {
            testLicht.color = Color.green;
        }
        else
        {
            testLicht.color = Color.red;
        }
    }

    public void DisplayText(string text, float time)
    {
        waitFinished = false;
        displayText.text = text;
        StartCoroutine(Sleep(time));        
    }
    IEnumerator Sleep(float seconds)
    {
        Debug.Log("Started Waiting at timestamp : " + Time.time);
        yield return new WaitForSeconds(seconds);
        waitFinished = true;
        Debug.Log("Finished Waiting at timestamp : " + Time.time);
    }
}
