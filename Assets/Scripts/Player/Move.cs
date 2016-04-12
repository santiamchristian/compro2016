﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(PhotonView))]
public class Move : Photon.MonoBehaviour, IPunObservable
{
    public GameObject Vehicle;
    bool inMotion; //Is this object in motion
    float horizontalspeed; //How fast you can move side to side
    float acceleration;
    float rotateSpeed = 1.2f;
    float turnSpeed = 1.5f;
    //float tiltAngle = 5; maybe use later
    private Vector3 mousePosition;
    float invertOrNot;
    float velocityZerotoOne;
    public ParticleSystem[] engines;
    Vehicle Vehicles;
    public float stunDuration;
    InputInformation InputInfo;

    private Vector3 latestCorrectPos;
    private Vector3 onUpdatePos;
    private float fraction;


    // Use this for initialization
    void Start ()
    {
        Vehicles = GetComponent<Vehicle>();
        InputInfo = GetComponent<InputInformation>();

        this.latestCorrectPos = transform.position;
        this.onUpdatePos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        if (this.photonView.isMine)
        {
            acceleration = Vehicles.fowardAccel;
            horizontalspeed = Vehicles.horizontalSpeed;
            stunDuration = Vehicles.vehicleStun;
            if (stunDuration <= 0)
            {
                GetComponent<Rigidbody>().AddForce(transform.forward * (acceleration + Vehicles.boostSpeed) * InputInfo.Forward());
                GetComponent<Rigidbody>().AddForce(transform.right * horizontalspeed * InputInfo.SideMovement());
                transform.Rotate(Vector3.forward * rotateSpeed * InputInfo.RotateShip());
                transform.Rotate(Vector3.right * turnSpeed * InputInfo.AxisY());
                transform.Rotate(Vector3.up * turnSpeed * InputInfo.AxisX());
                foreach (ParticleSystem engine in engines)
                {
                    if (InputInfo.Forward() > 0)
                        engine.Play();
                    else
                        engine.Stop();
                }
            }
            else
            {
                stunDuration -= Time.deltaTime;
            }
        }
        else
        {
            this.fraction = this.fraction + Time.deltaTime * 9;
            transform.position = Vector3.Lerp(this.onUpdatePos, this.latestCorrectPos, this.fraction); // set our pos between A and B
        }
    }

    

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            stream.Serialize(ref pos);
            stream.Serialize(ref rot);
        }
        else
        {
            // Receive latest state information
            Vector3 pos = Vector3.zero;
            Quaternion rot = Quaternion.identity;

            stream.Serialize(ref pos);
            stream.Serialize(ref rot);

            this.latestCorrectPos = pos;                // save this to move towards it in FixedUpdate()
            this.onUpdatePos = transform.position; // we interpolate from here to latestCorrectPos
            this.fraction = 0;                          // reset the fraction we alreay moved. see Update()

            transform.rotation = rot;              // this sample doesn't smooth rotation
        }
    }
}