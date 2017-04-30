using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;
using UnityEngine.SceneManagement;

public class Inputs : MonoBehaviour {

    public AutoCam autoCam;
    public Transform player;
    
    public void Restart()
    {
        SceneManager.LoadScene("scene1");
    }

    public void StartRace()
    {
        autoCam.SetTarget(player);
    }

    public void ChangeFlightMode(int value)
    {
        switch (value)
        {
            case 0:
                player.GetComponent<Controls>().flightMode = Controls.FlightModes.Angle;
                break;
            case 1:
                player.GetComponent<Controls>().flightMode = Controls.FlightModes.Accro;
                break;
            case 2:
                player.GetComponent<Controls>().flightMode = Controls.FlightModes.Accro;
                break;
            default:
                player.GetComponent<Controls>().flightMode = Controls.FlightModes.Angle;
                break;
        }
    }
}
