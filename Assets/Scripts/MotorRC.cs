﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorRC : MonoBehaviour
{

    // Use this for initialization
    public float motVmax;
    public float motAcc;
    public float motTmax;
    public float motWeight;
    public float motActualVmax;

    public bool anticlockwise;
    public float motActualAcc;
    public float motActualSpeed;
    public float motCmdSpeed;
    public float motForceTrust;
    public float floorEffect;
    public float floorDistance;
    private Controls controls;
    private Propeler propeler;
    private Rigidbody rg;

    void Awake()
    {
        controls = GameObject.Find("Controls").GetComponent<Controls>();
        propeler = transform.GetComponentInChildren<Propeler>();
        rg = transform.GetComponent<Rigidbody>();
        rg.transform.position = new Vector3(transform.position.x, 0, transform.position.z);  
        rg.transform.rotation = transform.rotation;
        rg.isKinematic = true; // clear forces
        rg.isKinematic = false;
        rg.velocity = Vector3.zero;
        rg.angularVelocity = Vector3.zero;
        rg.ResetInertiaTensor();
        rg.centerOfMass = Vector3.zero;
        rg.mass = motWeight;
        rg.maxAngularVelocity = Mathf.Infinity;
        motActualAcc = motAcc;
        // motActualSpeed = controls.throttleMin;
        motActualVmax = motVmax;
    }

    void Start()
    {

    }


    void Update()
    {
        float Delta = Time.deltaTime;
        if (motActualSpeed < motCmdSpeed)
            motActualSpeed += motActualAcc;
        else if (motActualSpeed > motCmdSpeed)
            motActualSpeed -= motActualAcc;
        // motCmdSpeed = Mathf.Clamp(motCmdSpeed, 0, motActualVmax);
        if (gameObject.GetComponentInParent<FixedJoint>() || gameObject.GetComponentInParent<HingeJoint>())
        {
            propeler.transform.Rotate(Vector3.forward * (controls.throttleMin*1.5f+controls.desiredSpeed*0.4f+motActualSpeed*10) * Delta);
        }
        else
        {
            motForceTrust = 0;
        }
    }

    void FixedUpdate()
    {
        float torque;
        if (gameObject.GetComponentInParent<FixedJoint>() || gameObject.GetComponentInParent<HingeJoint>())
        {
            motForceTrust = (controls.desiredSpeed/1000)+(controls.throttleRate * (motActualSpeed / 240) * propeler.propThrust);
            if (anticlockwise == true)
                torque = -(motCmdSpeed / 2) - rg.angularVelocity.y;
            else
                torque = (motCmdSpeed / 2) - rg.angularVelocity.y;
            //rg.AddRelativeTorque(new Vector3(0, torque, 0), ForceMode.VelocityChange);
            rg.AddRelativeForce(new Vector3(0, motForceTrust, 0), ForceMode.Force);
            FloorEffect();
        }
    }

    float FloorDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(rg.position, -Vector3.up, out hit))
        {
            // Debug.Log(hit.transform.name + " : " + hit.distance);
            return hit.distance;
        }
        else
            return 0;
    }

    void FloorEffect()
    {
        floorDistance = FloorDistance();
        if (floorDistance > 0 & floorDistance < 0.45f)
        {
            // Debug.Log("Floor Effect " + floorDistance);
            floorEffect = controls.desiredSpeed / (floorDistance*300000);
            rg.AddRelativeForce(Vector3.up * floorEffect, ForceMode.Force);
        }
        else
        {
            floorEffect = 0;
        }
    }

}