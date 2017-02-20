using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
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

    private Controls controls;
    private Propeler propeler;
    private Rigidbody rg;

    void Awake()
    {

        controls = transform.parent.GetComponent<Controls>();
        propeler = transform.GetComponentInChildren<Propeler>();
        rg = transform.GetComponent<Rigidbody>();
        rg.mass = motWeight;
        rg.maxAngularVelocity = Mathf.Infinity;
        motActualAcc = motAcc;
        // motActualSpeed = controls.throttleMin;
        motActualVmax = motVmax;
    }

    void Update()
    {
        float Delta = Time.deltaTime;
        if (motActualSpeed < motCmdSpeed)
            motActualSpeed += motActualAcc;
        else if (motActualSpeed > motCmdSpeed)
            motActualSpeed -= motActualAcc;
        motCmdSpeed = Mathf.Clamp(motCmdSpeed, 0, motActualVmax);
        if (gameObject.GetComponentInParent<FixedJoint>() || gameObject.GetComponentInParent<HingeJoint>())
        {
            propeler.transform.Rotate(Vector3.forward * (controls.throttleMin * 60 + motActualSpeed * 1.4f) * Delta);
            motForceTrust = Mathf.Clamp(((motActualSpeed / 30) * propeler.propThrust * controls.throttleRate * Delta), 0, propeler.propThrust * 4);
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
            rg.AddTorque(new Vector3(0, torque, 0), ForceMode.VelocityChange);
        }

    }
    
}