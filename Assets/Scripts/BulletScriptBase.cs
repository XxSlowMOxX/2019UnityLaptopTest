using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class BulletScriptBase : MonoBehaviour
{
    public float bulletSpeed;
    public float bulletDamage;
    private void FixedUpdate()
    {
        this.transform.position += this.transform.forward * Time.deltaTime * bulletSpeed;        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            print("Trigger Entered");
            if(other.gameObject.GetComponent<Entity>() != null)
            {
                other.gameObject.GetComponent<Entity>().TakeDamageCall(bulletDamage);
            }
            GetComponentInChildren<SpriteRenderer>().color = Color.cyan;
            
        }
    }
}
