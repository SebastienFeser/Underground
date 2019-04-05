using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointComponents : MonoBehaviour
{
    private LevelGeneration.WayPoint objectWaypoint;
    [SerializeField] private Vector2 positionInspector;
    [SerializeField] List<GameObject> nearWayPoints = new List<GameObject>();
    [SerializeField] List<Vector3> nearWaypointsInspector = new List<Vector3>();

    public class WayPoint
    {
        public WayPoint(Vector2 positionConstruct)
        {
            position = positionConstruct;
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public List<WayPoint> NearWayPoints
        {
            get { return nearWayPoints; }
            set { nearWayPoints = value; }
        }

        Vector2 position;
        List<WayPoint> nearWayPoints = new List<WayPoint>();
    }

    private void Update()
    {
        
        foreach (GameObject element in nearWayPoints)
        {
            Debug.DrawLine(transform.position, element.transform.position, Color.black);
        }
        
    }

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

    //Calculate
}


