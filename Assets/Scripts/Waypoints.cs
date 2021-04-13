using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    private static Waypoint firstWaypoint;
    public static GameObject startPoint;
    void Start()
    {
        startPoint = GameObject.Find("StartPoint");
    }
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
