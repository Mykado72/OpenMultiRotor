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
    public float rollExpo;
    public float pitchRate;
    public float pitchExpo;
    public float yawRate;
    public float rcRate;
    public MotorSet motorSet;
    public Vector3 consignVector;
    public float cmdRoll;
    public float cmdPitch;
    public float cmdYaw;
    public float convertedThrottle;
    public float desiredSpeed;
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
    public Vector3 spawnPosition;
    public Quaternion spawnRotation;

    // Use this for initialization
    void  Awake () {

    }

    void Start()
    {
        rgChassi.transform.gameObject.SetActive(false);
        rgChassi.transform.gameObject.SetActive(true);
        rgChassi.transform.position = spawnPosition;
        rgChassi.transform.rotation = Quaternion.Euler(Vector3.zero);
        rgChassi.isKinematic = true; // clear forces
        rgChassi.isKinematic = false; 
        rgChassi.velocity = Vector3.zero;
        rgChassi.maxAngularVelocity = 10; // Mathf.Infinity;
        rgChassi.angularVelocity = Vector3.zero;
        rgChassi.ResetInertiaTensor();
        // rgChassi.centerOfMass = new Vector3(0,-0.001f,0); // descendre le centre de gravité rend plus mou

        motorFrontLeft.transform.gameObject.SetActive(false);
        motorFrontLeft.transform.gameObject.SetActive(true);
        motorFrontLeft.isKinematic=true;
        motorFrontLeft.isKinematic = false;
        motorFrontLeft.angularVelocity = Vector3.zero;
        motorFrontLeft.transform.rotation = Quaternion.Euler(Vector3.zero);

        motorFrontRight.transform.gameObject.SetActive(false);
        motorFrontRight.transform.gameObject.SetActive(true);
        motorFrontRight.isKinematic = true;
        motorFrontRight.isKinematic = false;
        motorFrontRight.angularVelocity = Vector3.zero;
        motorFrontRight.transform.rotation = Quaternion.Euler(Vector3.zero);

        motorRearLeft.transform.gameObject.SetActive(false);
        motorRearLeft.transform.gameObject.SetActive(true);
        motorRearLeft.isKinematic = true;
        motorRearLeft.isKinematic = false;
        motorRearLeft.angularVelocity = Vector3.zero;
        motorRearLeft.transform.rotation = Quaternion.Euler(Vector3.zero);

        motorRearRight.transform.gameObject.SetActive(false);
        motorRearRight.transform.gameObject.SetActive(true);
        motorRearRight.isKinematic = true;
        motorRearRight.isKinematic = false;
        motorRearRight.angularVelocity = Vector3.zero;
        motorRearRight.transform.rotation = Quaternion.Euler(Vector3.zero);



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
                    consignVector.x = Expo(Input.GetAxis("axis0"), rollExpo, 0.02f);    // Roll"
                    consignVector.y = Input.GetAxis("axis3");
                    consignVector.z = -Expo(Input.GetAxis("axis1"), pitchExpo, 0.02f); // Pitch (Mode1)
                    throttle = -Input.GetAxis("axis2"); // Input.GetAxis("JoyTrottle(Mode1)-Pitch(Mode2)");
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
        desiredSpeed = throttleMin + (throttleRate *2000)* convertedThrottle; // + (motorSet.motorSet[0].motActualVmax*0.5f)   * convertedThrottle;
        cmdRoll = consignVector.x;
        cmdPitch = consignVector.z;
        cmdYaw = consignVector.y;
        
        //motorCmdFrontLeft = -cmdRoll * rollRate  + cmdYaw * yawRate + cmdPitch * pitchRate ;
        motorCmdFrontLeft = -cmdRoll * rollRate + cmdPitch * pitchRate;

        //motorCmdFrontRight = cmdRoll * rollRate - cmdYaw * yawRate + cmdPitch * pitchRate ;
        motorCmdFrontRight = cmdRoll * rollRate + cmdPitch * pitchRate;

        //motorCmdRearLeft = -cmdRoll * rollRate - cmdYaw * yawRate - cmdPitch * pitchRate ;
        motorCmdRearLeft = -cmdRoll * rollRate - cmdPitch * pitchRate;

        //motorCmdRearRight = cmdRoll * rollRate  + cmdYaw * yawRate - cmdPitch * pitchRate ;
        motorCmdRearRight = cmdRoll * rollRate - cmdPitch * pitchRate;

        motorSet.motorSet[0].motCmdSpeed = motorCmdFrontLeft * rcRate;
        motorSet.motorSet[1].motCmdSpeed = motorCmdFrontRight * rcRate;
        motorSet.motorSet[2].motCmdSpeed = motorCmdRearLeft * rcRate;
        motorSet.motorSet[3].motCmdSpeed = motorCmdRearRight * rcRate;
    }

    void FixedUpdate()
    {
        rgChassi.AddRelativeTorque(new Vector3(0, cmdYaw*yawRate, 0), ForceMode.VelocityChange);
        ClampVelocity();
    }

    void ClampVelocity()
    {
        rgChassi.velocity = Vector3.ClampMagnitude(rgChassi.velocity, 14f);
    }


    public void Restart()
    {
        SceneManager.LoadScene("scene1");
    }

    public float Expo(float value, float expo, float deadband)
    {  // expo allant de 0 à 1 
        if (Mathf.Abs(value) > deadband)
            return value*Mathf.Pow(Mathf.Abs(value), expo);
        else
            return 0f;
    }
}