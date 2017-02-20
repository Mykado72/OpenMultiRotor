using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera : MonoBehaviour {

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
        Vector3 forward = target.transform.forward * 2.0f;
        Vector3 up = target.transform.up* 1.3f;
        Vector3 needPos = target.transform.position - forward+up;
        transform.position = Vector3.SmoothDamp(transform.position, needPos,ref velocity,  0.075f); // plus la valeur est haute plus c'est lent
        var targetRotation = Quaternion.LookRotation(target.transform.position + forward - transform.position);              
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 4.5f); // plus la valeur est haute plus c'est rapide     
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