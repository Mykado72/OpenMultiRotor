using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cam : MonoBehaviour {

    public Transform target;
    public bool FPV;
    public GameObject camFPV;
    public Toggle toggleFPV;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 velocity = Vector3.zero;
        Vector3 rearPos = target.position- target.transform.forward * 1f+ target.transform.up * 0.25f;
        Vector3 forward = target.position+ target.transform.forward * 2f+ target.transform.up * 0.25f;
        transform.position = Vector3.SmoothDamp(transform.position, rearPos, ref velocity,  0.04f); // plus la valeur est haute plus c'est lent
        Quaternion targetRotation = Quaternion.LookRotation(forward - transform.position);              
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.4f); // plus la valeur est haute plus c'est rapide     
    }

    public void Toggle()
    {
        if (FPV== false)
        {
            SwitchFPVCam();
            FPV = true;
        }
        else
        {
            SwitchFollowCam();
            FPV = false;
        }
    }

    void  SwitchFPVCam()
    {
        camFPV.gameObject.SetActive(true);
    }

    void SwitchFollowCam()
    {
        camFPV.gameObject.SetActive(false);
    }

}