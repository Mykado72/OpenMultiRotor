using System.Collections;
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
        rg.transform.position = transform.position;
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
        if (motActualSpeed < motCmdSpeed* motActualAcc)
            motActualSpeed += motActualAcc;
        else if (motActualSpeed > motCmdSpeed* motActualAcc)
            motActualSpeed -= motActualAcc;
        // motCmdSpeed = Mathf.Clamp(motCmdSpeed, 0, motActualVmax);
        if (gameObject.GetComponentInParent<FixedJoint>() || gameObject.GetComponentInParent<HingeJoint>())
        {
            propeler.transform.Rotate(Vector3.forward * (1500+controls.desiredSpeed / 40 + motActualSpeed/2) * Delta);
            motForceTrust = ((controls.desiredSpeed/200)* controls.throttleRate + (motActualSpeed / 50) * propeler.propThrust)* Delta;
            // motForceTrust = Mathf.Clamp(((motActualSpeed / 30) * propeler.propThrust * controls.throttleRate * Delta), 0, propeler.propThrust * 4);
        }
        else
        {
            motForceTrust = 0;
        }
    }

    void FixedUpdate()
    {
        float torque;
        rg.AddRelativeForce(new Vector3(0, motForceTrust, 0), ForceMode.Force);
        if (motForceTrust > 0)
        {
            if (anticlockwise == true)
                torque = -(motCmdSpeed / 2) - rg.angularVelocity.y;
            else
                torque = (motCmdSpeed / 2) - rg.angularVelocity.y;
            rg.AddRelativeTorque(new Vector3(0, torque, 0), ForceMode.VelocityChange);
        }
        FloorEffect();
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
        if (floorDistance > 0 & floorDistance < 0.3f)
        {
            Debug.Log("Floor Effect " + floorDistance);
            floorEffect = controls.desiredSpeed / (floorDistance*1000000);
            rg.AddRelativeForce(Vector3.up * floorEffect, ForceMode.Force);
        }
        else
        {
            floorEffect = 0;
        }
    }

}