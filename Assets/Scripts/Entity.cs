using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
using Photon.Pun;

public class Entity : MonoBehaviour
{
    [Space]
    public string name;

    [Header("Health")]
    public float health = 100.0f;
    public float maxHealth = 100.0f;

    [Header("rest")]
    public Vector3 forwardVector;
    public enum movementType { Walk, Run, Sneak, Aim, Boost, Drive};
    public movementType myMovement;
    public enum entityState { Alive, Dead};
    public entityState myState;
    public playerTeam myTeam;
    public List<WeaponBase> weaponList = new List<WeaponBase>();
    public GameObject entitySprite;
    public int spawnIndex = 0;
    public int currentWeaponIndex;
    [Tooltip("Size of 0 equals invisible on MiniMap")]
    public float miniMapSize;
    public UnityEvent deathEvent;

    #region rpc calls
    public void Fire(GameObject prefab, Player spawningPlayer, Vector3 position, Quaternion Rotation, Vector3 velocity)
    {
        //print("Fire called");
        //print(prefab.name);   
        
    }
    public void TakeDamageCall(float damage, string inflicter)
    {
        print("Ich nehme Schaden");
        this.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, damage, inflicter);
    }
    public void PickUpCall(string objectName)
    {
        this.GetComponent<PhotonView>().RPC("PickUpWeapon", RpcTarget.All, objectName);
    }
    public void ChangeWeaponCall()
    {
        if(weaponList.Count == 2)
        {
            if(currentWeaponIndex == 0)
            {
                this.GetComponent<PhotonView>().RPC("ChangeWeapon", RpcTarget.All, 1, 0);
            }
            else
            {
                this.GetComponent<PhotonView>().RPC("ChangeWeapon", RpcTarget.All, 0, 1);
            }
        }
    }
    public void HealCall(float amount, bool fullyHeal = false)
    {
        if(fullyHeal == true)
        {
            this.GetComponent<PhotonView>().RPC("Heal", RpcTarget.All, maxHealth - health);
        }
        else
        {
            this.GetComponent<PhotonView>().RPC("Heal", RpcTarget.All, amount );
        }
    }
    public void DeathCall()
    {
        this.GetComponent<PhotonView>().RPC("EntityDeath", RpcTarget.All, "myself");
    }
    public void DropWeaponCall()
    {
        if(weaponList.Count != 0)
        {
            print("Will Drop Weapon");
            this.GetComponent<PhotonView>().RPC("DropWeapon", RpcTarget.All, currentWeaponIndex);
        }
    }    
    #endregion
    #region rpcs
    [PunRPC]
    void Heal(float amount)
    {
        health += amount;
    }
    [PunRPC]
    void ChangeWeapon(int swapIndex, int hideIndex)
    {
        GameObject newWeapon = weaponList[swapIndex].gameObject;
        newWeapon.transform.localEulerAngles = new Vector3(90, 0, 0);
        newWeapon.transform.localPosition = new Vector3(0.3f, 1, 0.45f);
        newWeapon.SetActive(true);
        weaponList[hideIndex].gameObject.SetActive(false);
        currentWeaponIndex = swapIndex;
    }
    [PunRPC]
    void DropWeapon(int dropIndex)
    {
        GameObject dropWeapon = weaponList[dropIndex].gameObject;
        dropWeapon.transform.localPosition = new Vector3(0.3f, 0.0f, 0.45f);
        dropWeapon.transform.SetParent(null, true);
        weaponList.Remove(weaponList[dropIndex]);
    }
    [PunRPC]
    void TakeDamage(float damage, string inflicter)
    {
        if (myState == entityState.Dead)
        {
            return;
        }
        health -= damage;
        if(health<=0 && PhotonNetwork.IsMasterClient)
        {            
            this.GetComponent<PhotonView>().RPC("EntityDeath", RpcTarget.All, inflicter);
        }
    }
    [PunRPC]
    void EntityDeath(string killedName)
    {
        print("<EntityDebug>: " + name + " has been killed");
        /*if(myState == entityState.Dead)
        {
            return;
        }*/
        deathEvent.Invoke();
        myState = entityState.Dead;
    }
    [PunRPC]
    void SpawnObject(string prefab, Player spawningPlayer, Vector3 position, Quaternion Rotation, Vector3 velocity, float destroyTimer, PhotonMessageInfo info)
    {
        //print("SpawnObject Called");
        spawnIndex+=1;
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        print("Lag is: " + lag);
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject serverObject = Instantiate(Resources.Load(prefab) as GameObject, position, Rotation);
            serverObject.GetComponent<Rigidbody>().velocity = velocity;
            serverObject.transform.position += velocity * lag;
            //serverObject.GetComponentInChildren<SpriteRenderer>().color = Color.red;
            serverObject.name += spawnIndex;
            if(destroyTimer != 0)
            {
                Destroy(serverObject, destroyTimer);
            }                        
        }
        else
        {
            GameObject clientObject = Instantiate(Resources.Load(prefab) as GameObject, position, Rotation);
            clientObject.GetComponent<Rigidbody>().velocity = velocity;
            clientObject.transform.position += velocity * lag;
            clientObject.name += spawnIndex;
            if (destroyTimer != 0)
            {
                Destroy(clientObject, destroyTimer);
            }
        }
    }
    [PunRPC]
    void PickUpWeapon(string weaponTag)
    {
        if(weaponList.Count == 0)
        {
            GameObject weapon = GameObject.Find(weaponTag);
            weapon.transform.SetParent(entitySprite.transform);
            weapon.transform.localEulerAngles = new Vector3(90, 0, 0);
            weapon.transform.localPosition = new Vector3(0.3f, 1, 0.45f);
            weaponList.Add(weapon.GetComponent<WeaponBase>());
            currentWeaponIndex = 0;
        } else if(weaponList.Count == 1)
        {
            GameObject weapon = GameObject.Find(weaponTag);
            weapon.transform.SetParent(entitySprite.transform);
            weaponList.Add(weapon.GetComponent<WeaponBase>());
            weapon.SetActive(false);
        }else
        {
            if(this.gameObject.GetComponent<CubeMovement>() != null)
            {
                print("Yo Dawg");
                this.gameObject.GetComponent<CubeMovement>().playerHUD.GetComponent<HUDScript>().DisplayText("Too many Weapons", 2);
            }
        }
        
    }
    #endregion

}
