using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour {

    public int rcMode;
    public bool joystick;
    public float throttle;
    public float throttleMin;
    public float throttleRate;
    public float rollRate;
    public float pitchRate;
    public float yawRate;
    public float rcRate;
    public MotorSet motorSet;
    public Vector3 consignVector;
    public float cmdRoll;
    public float cmdPitch;
    public float cmdYaw;
    public float convertedThrottle;
    public float desiredSpeed;
    public float floorEffect;
    public float floorDistance;
    public Rigidbody rgChassi;
    public Rigidbody motorFrontLeft;
    private float motorCmdFrontLeft;
    public Rigidbody motorFrontRight;
    private float motorCmdFrontRight;
    public Rigidbody motorRearLeft;
    private float motorCmdRearLeft;
    public Rigidbody motorRearRight;
    private float motorCmdRearRight;
    public string joystickname;

    // Use this for initialization
    void  Awake () {
        rgChassi.maxAngularVelocity = 60;
        rgChassi.centerOfMass = new Vector3(0,-0.05f,0);
        string[] joynames = Input.GetJoystickNames();
        if (joynames.Length > 0)
        {
            if (joynames[0] == "FrSky Taranis Joystick")
                Debug.Log("Cool RcTransmitter !!!");
            if (joynames[0] != "")
                joystick = true;
            else
                joystick = false;
        }
        else
        {
            joystick = false;
        }

    }

    // Update is called once per frame
    void Update () {
        
        float Delta = Time.deltaTime;

        switch (rcMode)
        {
        case 1:
                if (joystick)
                {
                    consignVector.x = Input.GetAxis("JoyRoll");
                    consignVector.y = Input.GetAxis("JoyYaw");
                    consignVector.z = Input.GetAxis("JoyPitch(Mode1)-Trottle(Mode2)");
                    throttle = Input.GetAxis("JoyTrottle(Mode1)-Pitch(Mode2)");
                }
                else
                {
                    consignVector.x = Input.GetAxis("KeyRoll");
                    consignVector.y = Input.GetAxis("KeyYaw");
                    consignVector.z = Input.GetAxis("KeyPitch(Mode1)-Trottle(Mode2)");
                    throttle = Input.GetAxis("KeyTrottle(Mode1)-Pitch(Mode2)");
                }
                break;
        case 2:
                if (joystick)
                {
                    consignVector.x = Input.GetAxis("JoyRoll");
                    consignVector.y = Input.GetAxis("JoyYaw");
                    consignVector.z = Input.GetAxis("JoyTrottle(Mode1)-Pitch(Mode2)");
                    throttle = Input.GetAxis("JoyPitch(Mode1)-Trottle(Mode2)");
                }
                else
                {
                    consignVector.x = Input.GetAxis("KeyRoll");
                    consignVector.y = Input.GetAxis("KeyYaw");
                    consignVector.z = Input.GetAxis("KeyTrottle(Mode1)-Pitch(Mode2)");
                    throttle = Input.GetAxis("KeyPitch(Mode1)-Trottle(Mode2)");
                }
                break;
        case 3:
            break;
        case 4:
            break;
        default:
            break;
        }
        convertedThrottle = (1 + throttle) / 2 ;
        desiredSpeed = throttleMin+(motorSet.motorSet[0].motActualVmax*0.5f)   * convertedThrottle;
        cmdRoll = consignVector.x;
        cmdPitch = consignVector.z;
        cmdYaw = consignVector.y;
        motorCmdFrontLeft = -cmdRoll * rollRate  + cmdYaw * yawRate + cmdPitch * pitchRate ;
        motorCmdFrontRight = cmdRoll * rollRate - cmdYaw * yawRate + cmdPitch * pitchRate ;
        motorCmdRearLeft = -cmdRoll * rollRate - cmdYaw * yawRate - cmdPitch * pitchRate ;
        motorCmdRearRight = cmdRoll * rollRate  + cmdYaw * yawRate - cmdPitch * pitchRate ;
        motorSet.motorSet[0].motCmdSpeed = desiredSpeed + motorCmdFrontLeft * rcRate;
        motorSet.motorSet[1].motCmdSpeed = desiredSpeed + motorCmdFrontRight * rcRate;
        motorSet.motorSet[2].motCmdSpeed = desiredSpeed + motorCmdRearLeft * rcRate;
        motorSet.motorSet[3].motCmdSpeed = desiredSpeed + motorCmdRearRight * rcRate;

    }

    void FixedUpdate()
    {
        FloorEffect();
        // ClampVelocity();
    }

    void ClampVelocity()
    {
        rgChassi.velocity = new Vector3(Mathf.Clamp(rgChassi.velocity.x, -10f, 10f), Mathf.Clamp(rgChassi.velocity.y, -9.81f, 10f), Mathf.Clamp(rgChassi.velocity.z, -10f, 10f));
    }

    float FloorDistance()
    {
        RaycastHit hit;
        if (Physics.Raycast(rgChassi.position, -Vector3.up, out hit))
        {
            Debug.Log(hit.transform.name+ " : " +hit.distance);
            return hit.distance;
        }
        else
            return 0;
    }

    void FloorEffect()
    {
        floorDistance = FloorDistance();
        if (floorDistance > 0 & floorDistance < 0.4f)
        {
            Debug.Log("Floor Effect " + floorDistance);
            floorEffect = convertedThrottle+1.1f;
            rgChassi.AddRelativeForce(Vector3.up* floorEffect, ForceMode.Acceleration);
        }
    }


    public void Restart()
    {
        SceneManager.LoadScene("scene1");
    }
}