using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

    public Transform target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 velocity = Vector3.zero;
        Vector3 forward = target.transform.forward * 3f;
        Vector3 up = target.transform.up* 0.15f;
        Vector3 needPos = target.transform.position - forward+up;
        transform.position = Vector3.SmoothDamp(transform.position, needPos,ref velocity, Time.deltaTime * 1.0f); // plus la valeur est haute plus c'est lent
        var targetRotation = Quaternion.LookRotation(target.transform.position + forward - transform.position);

        // Smoothly rotate towards the target point.
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);
        transform.LookAt(target.position);
    }

}