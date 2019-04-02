﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointComponents : MonoBehaviour
{
    private LevelGeneration.WayPoint objectWaypoint;
    [SerializeField] private Vector2 positionInspector;
    List<GameObject> nearWayPoints = new List<GameObject>();
    [SerializeField] List<Vector3> nearWaypointsInspector = new List<Vector3>();

    /*private void Update()
    {
        
        foreach (GameObject element in nearWayPoints)
        {
            nearWaypointsInspector.Add(element.transform.position);
        }
        positionInspector = objectWaypoint.Position;
        
    }*/

    public LevelGeneration.WayPoint ObjectWaypoint
    {
        get { return objectWaypoint; }
        set { objectWaypoint = value; }
    }

    public List<GameObject> NearWayPoints
    {
        get { return nearWayPoints; }
        set { nearWayPoints = value; }
    }

    public List<Vector3> NearWayPointsInspector
    {
        get { return nearWaypointsInspector; }
        set { nearWaypointsInspector = value; }
    }
}


