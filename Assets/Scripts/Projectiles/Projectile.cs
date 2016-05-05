﻿using UnityEngine;
using System.Collections;

public class Projectile : Photon.MonoBehaviour
{

    public bool trackTarget;
    public float stunDuration = 2;

    public int shooterId;

    // Speed
    public float speed = 700;

    // Target
    public Transform target;
    Pickup Pickups;
    void Start()
    {
        
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        shooterId = info.sender.ID;
    }

    void FixedUpdate()
    {
        if (trackTarget)
        {
            if (target)
            {
                transform.LookAt(target.position);
                // Fly towards the target        
                GetComponent<Rigidbody>().velocity = transform.forward * speed;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<Player>().playerID != shooterId)
        {
            other.gameObject.GetComponent<Vehicle>().Stun();
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider otherShip)
    {
        //make a code for detecting a ship that isnt yourself.
        if (otherShip.gameObject.layer != 8 && otherShip.gameObject.layer == 9 && target == null)
        {
            target = otherShip.transform;
        }
    }
}
