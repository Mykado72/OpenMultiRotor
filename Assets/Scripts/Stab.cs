using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stab : MonoBehaviour {


    public float stability = 0.1f;
    public float speed = 2.0f;
    public Rigidbody rgChassi;
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 predictedUp = Quaternion.AngleAxis(
            rgChassi.angularVelocity.magnitude * Mathf.Rad2Deg * stability / speed,
            rgChassi.angularVelocity
        ) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
        rgChassi.AddTorque(torqueVector * speed * speed);
    }

}
