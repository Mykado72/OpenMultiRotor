using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    void OnJointBreak(float breakForce)
    {
        Debug.Log("A joint has just been broken!, force: " + breakForce);
        transform.GetComponent<Rigidbody>().useGravity = true;
    }  

}
