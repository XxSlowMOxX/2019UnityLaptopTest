using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class WeaponBase : MonoBehaviour
{ 
    
    public enum fireMode { FullAuto, SemiAuto, Burst, Beam};
    [Header("Firing Options")]
    public fireMode firingMode;
    public float firingRate;
    public float spread;
    [Header("Mag Options")]
    public int magSize;
    public int maxAmmo;
    public int currentAmmo;
    public int currentMag;
    public float reloadingTime;
    [Header("Bullet Options")]
    public GameObject bulletObject;

    private float timeSinceShot;

    public bool Fire(float deltaTime)
    {
        timeSinceShot += deltaTime;
        if(timeSinceShot >= 60 / firingRate)
        {
            if(currentMag > 0)
            {
                timeSinceShot = 0;
                currentMag--;
                this.GetComponentInParent<Entity>().Fire(bulletObject, PhotonNetwork.LocalPlayer, this.transform.position + (this.transform.up) - new Vector3(0,1,0), this.transform.parent.rotation * Quaternion.Euler(0, Random.Range(-spread, spread), 0), Vector3.zero);
                return true;
            }
            else
            {
                print("No Ammo");
                this.GetComponent<AudioSource>().Play();
                return false;
            }
        }
        else
        {
            return false;
        }
    }
    public void ReloadStart()
    {
        if(GetComponentInParent<CubeMovement>() != null)
        {
            print("E");
            GetComponentInParent<CubeMovement>().playerHUD.GetComponent<HUDScript>().DisplayText("Reloading...", reloadingTime);
        }
        StartCoroutine(ReloadSleep(reloadingTime));
    }
    void Reload()
    {
        currentMag = magSize;
    }
    IEnumerator ReloadSleep(float seconds)
    {
        Debug.Log("Started Waiting at timestamp : " + Time.time);
        yield return new WaitForSeconds(seconds);
        Debug.Log("Finished Waiting at timestamp : " + Time.time);
        Reload();
    }
}
