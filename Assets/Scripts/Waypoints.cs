using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public static Waypoint firstWaypoint;
    public static Waypoint GetFirstWaypoint()
    {
        if (firstWaypoint != null)
        {
            return firstWaypoint;
        }
        var waypoint = GameObject.FindObjectOfType<Waypoint>();
        while (waypoint.previousWaypoint!=null)
        {
            waypoint = waypoint.previousWaypoint;
        }
        return firstWaypoint = waypoint;
    }
}
