using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Item : MonoBehaviour
{
    public float DestroyTime;
    void Start()
    {
        Object.Destroy(gameObject, DestroyTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Voiture"))
        {
            //PhotonNetwork.Instantiate(explode, location.position, transform.rotation);
            //col.gameObject. RESPAWN
            Object.Destroy(gameObject);
        }
    }
}

