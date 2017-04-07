using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour {

    private WaypointSystem waypointSystem;
    private Controls controls;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag== "MultiRotor")
        {
            controls = other.GetComponentInParent<Controls>();
            waypointSystem = controls.waypointSystem;
            if (controls.targetWaypoint.name == this.name)
            {
                Debug.Log(other.name);
                controls.waypointnb++;
                if (controls.waypointnb>waypointSystem.waypoints.Length-1) // retour au debut
                    controls.waypointnb=0;
                controls.targetWaypoint = waypointSystem.waypoints[controls.waypointnb];
            }
        }
    }

}
