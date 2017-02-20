using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour {

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

    private Controls controls;
    private Propeler propeler;
    private Rigidbody rg;

    void Awake () {

        controls = transform.parent.GetComponent<Controls>();
        propeler = transform.GetComponentInChildren<Propeler>();
        rg = transform.GetComponent<Rigidbody>();
        rg.mass = motWeight;
        rg.maxAngularVelocity = Mathf.Infinity;
        motActualAcc = motAcc;
        // motActualSpeed = controls.throttleMin;
        motActualVmax = motVmax;
    }

    void FixedUpdate()
    {
        float Delta = Time.deltaTime;
        float torque;
        if (motActualSpeed < motCmdSpeed)
            motActualSpeed += motActualAcc;
        else if (motActualSpeed > motCmdSpeed)
            motActualSpeed -= motActualAcc;
        motCmdSpeed = Mathf.Clamp(motCmdSpeed, 0, motActualVmax);
        if (gameObject.GetComponentInParent<FixedJoint>()|| gameObject.GetComponentInParent<HingeJoint>())
        {
            propeler.transform.Rotate(Vector3.forward * (controls.throttleMin*2+motActualSpeed*1.4f) *  Delta);
            float trust= Mathf.Clamp(((motActualSpeed / 20)* propeler.propThrust * controls.throttleRate * Delta), 0, propeler.propThrust*4); 
            rg.AddRelativeForce(new Vector3(0, trust, 0), ForceMode.Force);
            if (anticlockwise == true)
                torque = -(motCmdSpeed/4 ) - rg.angularVelocity.y * Delta;
            else
                torque = (motCmdSpeed/4 ) - rg.angularVelocity.y * Delta;            
            rg.AddTorque(new Vector3(0, torque, 0), ForceMode.VelocityChange);
        }
    }
}
