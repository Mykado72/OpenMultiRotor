using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controls : MonoBehaviour {

    public int rcMode;
    public enum flightModes { Angle, Horizon, Accro };
    public flightModes flightMode;
    public int pitchAngleMax = 45;
    public int rollAngleMax = 45;
    public bool joystick;
    private Mapping mapping;
    public float throttle;
    public float throttleMin;
    public float throttleRate;
    public float rollRate;
    public float rollExpo;
    public float pitchRate;
    public float pitchExpo;
    public float yawRate;
    public float yawExpo;
    public float rcRate;
    public MotorSet motorSet;
    public Vector3 consignVector;
    public Vector3 AngularConsignVector;
    public Vector3 ActualRotationVector;
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
            string joyname = joynames[0];
            joyname =joyname.Replace(" ", "");
            mapping = (Mapping) AssetDatabase.LoadAssetAtPath("Assets/" + joyname + ".asset", typeof(Mapping));
            if (mapping == null)
            {
                joystick = false;
            }
            else
            {
                joystick = true;
                // Debug.Log(mapping.pitchAxisName);
            }
//            if (joynames[0] == "FrSky Taranis Joystick")
//               Debug.Log("Cool RcTransmitter !!!");
        }
        else
        {
            joystick = false;
        }
    }

    // Update is called once per frame
    void Update () {
        
        float Delta = Time.deltaTime;
        ActualRotationVector = rgChassi.transform.eulerAngles; //  - new Vector3(180, 0, 180);
        if (ActualRotationVector.x >= 180)
            ActualRotationVector.x = -(360 - ActualRotationVector.x);
        if (ActualRotationVector.y >= 180)
            ActualRotationVector.y = -(360 - ActualRotationVector.y);
        if (ActualRotationVector.z >= 180)
            ActualRotationVector.z = -(360 - ActualRotationVector.z);
        switch (rcMode)
        {
        case 1:
                if (joystick)
                {
                    if (mapping.rollAxisInversor == true)
                        consignVector.x = -Expo(Input.GetAxis("axis" + mapping.rollAxisNb.ToString())/rollRate, rollExpo, 0.00002f);    // Roll
                    else
                        consignVector.x = Expo(Input.GetAxis("axis" + mapping.rollAxisNb.ToString()) / rollRate, rollExpo, 0.00002f);    // Roll
                    if (mapping.yawAxisInversor == true)
                        consignVector.y = -Expo(Input.GetAxis("axis" + mapping.yawAxisNb.ToString()) /yawRate, yawExpo, 0.0002f);     // Yaw
                    else
                        consignVector.y = Expo(Input.GetAxis("axis" + mapping.yawAxisNb.ToString()) / yawRate, yawExpo, 0.0002f);     // Yaw
                    if (mapping.pitchAxisInversor == true)
                        consignVector.z = Expo(Input.GetAxis("axis" + mapping.pitchAxisNb.ToString()) /pitchRate, pitchExpo, 0.00002f); // Pitch 
                    else
                        consignVector.z = -Expo(Input.GetAxis("axis" + mapping.pitchAxisNb.ToString()) / pitchRate, pitchExpo, 0.00002f); // Pitch 
                    if (mapping.throttleAxisInversor == true)
                        throttle = Input.GetAxis("axis" + mapping.throttleAxisNb.ToString()); 
                    else
                        throttle = -Input.GetAxis("axis" + mapping.throttleAxisNb.ToString()); 
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
        desiredSpeed = throttleMin + (throttleRate *1000)* convertedThrottle; // + (motorSet.motorSet[0].motActualVmax*0.5f)   * convertedThrottle;

        switch (flightMode)
        {
            case flightModes.Angle:
                AngularConsignVector.x = consignVector.x * rollAngleMax;
                AngularConsignVector.z = consignVector.z * pitchAngleMax;
                //ActualRotationVector.x= ActualRotationVector.x;
                ActualRotationVector.z = -ActualRotationVector.z;
                // AngularConsignVector.z = consignVector.z * pitchAngleMax;
                cmdPitch = -(ActualRotationVector.x*0.25f - AngularConsignVector.z) / 25;
                cmdRoll = - (ActualRotationVector.z*0.25f - AngularConsignVector.x) / 25;
                // cmdPitch = consignVector.z;
                cmdYaw = consignVector.y;
                break;
            case flightModes.Horizon:
                cmdRoll = consignVector.x;
                cmdPitch = consignVector.z;
                cmdYaw = consignVector.y;
                break;
            case flightModes.Accro:
                cmdRoll = consignVector.x;
                cmdPitch = consignVector.z;
                cmdYaw = consignVector.y;
                break;
            default:
                cmdRoll = consignVector.x;
                cmdPitch = consignVector.z;
                cmdYaw = consignVector.y;
                break;
        }
        
        //motorCmdFrontLeft = -cmdRoll * rollRate  + cmdYaw * yawRate + cmdPitch * pitchRate ;
        motorCmdFrontLeft = -cmdRoll  + cmdPitch ;

        //motorCmdFrontRight = cmdRoll * rollRate - cmdYaw * yawRate + cmdPitch * pitchRate ;
        motorCmdFrontRight = cmdRoll  + cmdPitch ;

        //motorCmdRearLeft = -cmdRoll * rollRate - cmdYaw * yawRate - cmdPitch * pitchRate ;
        motorCmdRearLeft = -cmdRoll - cmdPitch ;

        //motorCmdRearRight = cmdRoll * rollRate  + cmdYaw * yawRate - cmdPitch * pitchRate ;
        motorCmdRearRight = cmdRoll  - cmdPitch ;

        motorSet.motorSet[0].motCmdSpeed = motorCmdFrontLeft * rcRate* throttleRate;
        motorSet.motorSet[1].motCmdSpeed = motorCmdFrontRight * rcRate* throttleRate;
        motorSet.motorSet[2].motCmdSpeed = motorCmdRearLeft * rcRate* throttleRate;
        motorSet.motorSet[3].motCmdSpeed = motorCmdRearRight * rcRate* throttleRate;
    }

    void FixedUpdate()
    {
        rgChassi.AddRelativeTorque(new Vector3(0, cmdYaw, 0), ForceMode.VelocityChange);
        ClampVelocity();
    }

    void ClampVelocity()
    {
        rgChassi.velocity = Vector3.ClampMagnitude(rgChassi.velocity, 10f);
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