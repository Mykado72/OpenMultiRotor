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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision à la vitesse :" + collision.relativeVelocity.magnitude+ " de " + this.name + " avec " + collision.gameObject.name);

        // if (collision.relativeVelocity.magnitude > 10)
            // this.GetComponentInParent<Rigidbody>().useGravity = true;
    }

}
