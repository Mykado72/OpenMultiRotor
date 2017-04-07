using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Controls : MonoBehaviour {

    public Transform cg;
    public bool AIControl;
    private float delta;
    public PIDSet pidSet;
    public PIDSet AIControlPID;
    public int rcMode;
    public enum flightModes { Angle, Horizon, Accro };
    public Dropdown flightMode;
    // public flightModes flightMode;
    public int pitchAngleMax = 45;
    public int rollAngleMax = 45;
    public float stabspeed = 0.25f;
    public bool joystick;
    public AxisMap axisMap;
    public Mapping mapping;
    public float throttle;
    public float minimalSpeed;
    public float minimalRotationSpeed;
    public float throttleRate;
    public float rollRate;
    public float rollExpo;
    public float pitchRate;
    public float pitchExpo;
    public float yawRate;
    public float yawExpo;
    public float accroRate;
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
    public Transform spawnTransform;
//     public Quaternion spawnRotation;
    public Button mapbutton;
    public WaypointSystem waypointSystem;
    public int waypointnb;
    public Transform targetWaypoint;
    public Vector3 RelativeWaypointPosition;
    public Vector3 RelativeWaypointRotation;
    public Quaternion QRelativeWaypointRotation;
    public Quaternion rotationDelta;
    public Vector3 rotationDeltaEuler;

    // Use this for initialization
    void  Awake () {
        waypointnb = 0;
        targetWaypoint= waypointSystem.waypoints[waypointnb];
    }

    void Start()
    {   
        if (axisMap.joystickDetected==true)
        {
            mapping = axisMap.mapping;
            joystick = true;
        }
        else
        {
            joystick = false;
        }       
        rgChassi.transform.gameObject.SetActive(false);
        rgChassi.transform.gameObject.SetActive(true);
        rgChassi.transform.position = spawnTransform.position;
        rgChassi.transform.rotation = spawnTransform.rotation;
        rgChassi.isKinematic = true; // clear forces
        rgChassi.isKinematic = false; 
        rgChassi.velocity = Vector3.zero;
        rgChassi.maxAngularVelocity = 10; // Mathf.Infinity;
        rgChassi.angularVelocity = Vector3.zero;
        rgChassi.ResetInertiaTensor();
        // rgChassi.centerOfMass = cg.position; // descendre le centre de gravité rend plus mou

        /* motorFrontLeft.transform.gameObject.SetActive(false);
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
        motorRearRight.transform.rotation = Quaternion.Euler(Vector3.zero);   */
    }

    // Update is called once per frame
    void Update () {
        delta = Time.deltaTime;
        RotationCorrection();
        GetControls();
        convertedThrottle = (1 + throttle) / 2 ;
        // desiredSpeed = (throttleRate)* convertedThrottle*100; // + (motorSet.motorSet[0].motActualVmax*0.5f)   * convertedThrottle;
        //FlightMode();
        if (AIControl == true)
        {
            SendCmdToMotors();
        }
        else
        {
            SendCmdToMotors();
        }       
    }

    void FixedUpdate()
    {
        FlightMode();
        if (Mathf.Abs(cmdYaw)>0)
            rgChassi.AddRelativeTorque(new Vector3(0, cmdYaw, 0), ForceMode.VelocityChange);
        ClampVelocity();
    }

    void RotationCorrection()
    {
        ActualRotationVector = rgChassi.transform.localEulerAngles;
        if (ActualRotationVector.x >= 180)
            ActualRotationVector.x = -(360 - ActualRotationVector.x);
        if (ActualRotationVector.y >= 180)
            ActualRotationVector.y = -(360 - ActualRotationVector.y);
        if (ActualRotationVector.z >= 180)
            ActualRotationVector.z = -(360 - ActualRotationVector.z);      
    }

    void GetControls()
    {
        if (AIControl == true)
        {
            Vector3 targetDir = targetWaypoint.position - rgChassi.transform.position;
            targetDir.y = 0;
            Quaternion rotation = Quaternion.LookRotation(targetDir);
            //rgChassi.MoveRotation(Quaternion.Slerp(rgChassi.rotation, Quaternion.Euler(rgChassi.rotation.eulerAngles.x, rotation.eulerAngles.y, rgChassi.rotation.z), delta));
            // rgChassi.MoveRotation(Quaternion.Slerp(rgChassi.rotation, Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, rotation.eulerAngles.z), delta*4));
            RelativeWaypointPosition = rgChassi.transform.InverseTransformPoint(targetWaypoint.position);
            //float angle = Quaternion.Angle(rgChassi.rotation, QRelativeWaypointRotation); // angle de la cible par rapport au monde
            // float relangle = Quaternion.Angle(rgChassi.rotation, Quaternion.identity );  // angle du chassi par rapport au monde
            consignVector.y = Mathf.Clamp(AIControlPID.yawPID.Update(rotation.eulerAngles.y, rgChassi.rotation.eulerAngles.y, stabspeed * delta), -1f, +1f);
            consignVector.z = (consignVector.y*0.95f)+Mathf.Clamp(AIControlPID.rollPID.Update(rgChassi.position.x+ RelativeWaypointPosition.x, rgChassi.position.x, stabspeed * delta), -1f, +1f);
            consignVector.x = Mathf.Clamp(AIControlPID.pitchPID.Update(rgChassi.position.z+RelativeWaypointPosition.z, rgChassi.position.z, stabspeed * delta),-1f,+1f);

            throttle = Mathf.Clamp(AIControlPID.throttlePID.Update(targetWaypoint.position.y, rgChassi.position.y, stabspeed * delta), -1f, +1f);
        }
        else // c'est un joueur
        {
            if (axisMap.joystickDetected == true)
            {
                mapping = axisMap.mapping;
                joystick = true;
                mapbutton.interactable = true;
            }
            else 
            {
                joystick = false;
                mapbutton.interactable = false;
            }
            if (joystick)
            {
                if (mapping.rollAxisInversor == true)
                    consignVector.z = -Expo(Input.GetAxis("axis" + mapping.rollAxisNb.ToString()), rollExpo, 0.0002f);    // Roll
                else
                    consignVector.z = Expo(Input.GetAxis("axis" + mapping.rollAxisNb.ToString()), rollExpo, 0.0002f);    // Roll
                if (mapping.yawAxisInversor == true)
                    consignVector.y = -Expo(Input.GetAxis("axis" + mapping.yawAxisNb.ToString()), yawExpo, 0.0002f);     // Yaw
                else
                    consignVector.y = Expo(Input.GetAxis("axis" + mapping.yawAxisNb.ToString()), yawExpo, 0.0002f);     // Yaw
                if (mapping.pitchAxisInversor == true)
                    consignVector.x = Expo(Input.GetAxis("axis" + mapping.pitchAxisNb.ToString()), pitchExpo, 0.0002f); // Pitch 
                else
                    consignVector.x = -Expo(Input.GetAxis("axis" + mapping.pitchAxisNb.ToString()), pitchExpo, 0.0002f); // Pitch 
                if (mapping.throttleAxisInversor == true)
                    throttle = Input.GetAxis("axis" + mapping.throttleAxisNb.ToString());
                else
                    throttle = -Input.GetAxis("axis" + mapping.throttleAxisNb.ToString());
            }
            else
            {
                consignVector.z = Expo(Input.GetAxis("KeyRoll"), rollExpo, 0.0002f);
                consignVector.y = Expo(Input.GetAxis("KeyYaw"), yawExpo, 0.0002f);
                consignVector.x = Expo(Input.GetAxis("KeyPitch(Mode1)-Trottle(Mode2)"), pitchExpo, 0.0002f);
                throttle = Input.GetAxis("KeyTrottle(Mode1)-Pitch(Mode2)");
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
    void FlightMode()
    {
        float fixeddelta = Time.fixedDeltaTime;
        switch (flightMode.value)
        {
            case 0:  // angle
                AngularConsignVector.x = consignVector.x * pitchAngleMax;
                AngularConsignVector.z = consignVector.z * rollAngleMax;
                AngularConsignVector.y = consignVector.y * yawRate;
                //ActualRotationVector.x = Mathf.Round(ActualRotationVector.x);
                ActualRotationVector.z = -ActualRotationVector.z;
                    cmdPitch = pidSet.pitchPID.Update(AngularConsignVector.x, ActualRotationVector.x, stabspeed * fixeddelta);
                    cmdRoll = pidSet.rollPID.Update(AngularConsignVector.z, ActualRotationVector.z, stabspeed * fixeddelta);
                    cmdYaw = consignVector.y * yawRate;
                    // cmdYaw = Mathf.Round(pidSet.yawPID.Update(AngularConsignVector.y, ActualRotationVector.y, stabspeed * delta));  
                break;
            case 2: // horizon
                cmdRoll = consignVector.z* accroRate * rollRate; 
                cmdPitch = consignVector.x* accroRate * pitchRate; 
                cmdYaw = consignVector.y *yawRate;
                break;
            case 1: // acro
                cmdRoll = (consignVector.z* rollRate * accroRate); 
                cmdPitch = (consignVector.x* pitchRate* accroRate);
                cmdYaw = consignVector.y* yawRate;
                break;
            default:
                cmdRoll = consignVector.z * accroRate * rollRate;
                cmdPitch = consignVector.x * accroRate * pitchRate;
                cmdYaw = consignVector.y * yawRate;
                break;
        }
    }
    void SendCmdToMotors()
    {
        if (AIControl == true)
        {
            motorCmdFrontRight = (cmdRoll + cmdPitch) * throttleRate;
            motorCmdRearLeft = (-cmdRoll - cmdPitch) * throttleRate;
            motorCmdRearRight = (cmdRoll - cmdPitch) * throttleRate;
            desiredSpeed = (minimalSpeed + throttle) * throttleRate;
        }
        else
        {
            motorCmdFrontRight = (cmdRoll + cmdPitch) * throttleRate;
            motorCmdRearLeft = (-cmdRoll - cmdPitch) * throttleRate;
            motorCmdRearRight = (cmdRoll - cmdPitch) * throttleRate;
            desiredSpeed = (minimalSpeed + throttle) * throttleRate;
        }
        motorSet.motorSet[0].hoverSpeed = desiredSpeed;
        motorSet.motorSet[1].hoverSpeed = desiredSpeed;
        motorSet.motorSet[2].hoverSpeed = desiredSpeed;
        motorSet.motorSet[3].hoverSpeed = desiredSpeed;
        motorSet.motorSet[0].motCmdSpeed = motorCmdFrontLeft;// + motorCmdFrontLeft* motorSet.motorSet[0].motActualAcc*0.5f;// *  throttleRate;
        motorSet.motorSet[1].motCmdSpeed = motorCmdFrontRight;// + motorCmdFrontRight* motorSet.motorSet[1].motActualAcc*0.5f; // * throttleRate;
        motorSet.motorSet[2].motCmdSpeed = motorCmdRearLeft;// + motorCmdRearLeft* motorSet.motorSet[2].motActualAcc*0.5f; // * throttleRate;
        motorSet.motorSet[3].motCmdSpeed = motorCmdRearRight;// + motorCmdRearRight* motorSet.motorSet[3].motActualAcc*0.5f; // * throttleRate;     
    }
    void ClampVelocity()
    {
        // rgChassi.velocity = Vector3.ClampMagnitude(rgChassi.velocity, 10f);
        rgChassi.velocity = Vector3.ClampMagnitude(rgChassi.velocity, 25f);
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
    public static float FindDegree(float x, float y)
    {
        float value = (float)((Mathf.Atan2(x, y) / Math.PI) * 180f);
        if (value < 0) value += 360f;
        return value;
    }
}