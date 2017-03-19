using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cam2 : MonoBehaviour {

    public bool FPV;
    public GameObject camFPV;
    public Toggle toggleFPV;
    public GameObject player;       //Public variable to store a reference to the player game object

    private Vector3 offset;         //Private variable to store the offset distance between the player and camera
    Vector3 velocity;
    // Use this for initialization
    void Start()
    {
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;

    }

    //void Update()
    //{
    // velocity = player.GetComponent<Rigidbody>().velocity/2;
    // }

    // LateUpdate is called after Update each frame
    void FixedUpdate()
    {
        velocity = Vector3.zero;
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = Vector3.SmoothDamp(transform.position, player.transform.position+ offset, ref velocity, 0.025f);
        Quaternion rotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.90f); 
    }

    public void Toggle()
    {
        if (FPV == false)
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

    void SwitchFPVCam()
    {
        camFPV.gameObject.SetActive(true);
    }

    void SwitchFollowCam()
    {
        camFPV.gameObject.SetActive(false);
    }

}