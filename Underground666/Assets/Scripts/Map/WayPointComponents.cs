using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointComponents : MonoBehaviour
{
    [SerializeField] private LevelGeneration.WayPoint objectWaypoint;

    public LevelGeneration.WayPoint ObjectWaypoint
    {
        get { return objectWaypoint; }
        set { objectWaypoint = value; }
    }

    List<LevelGeneration.WayPoint> wayPointsToCheck;

    public List<LevelGeneration.WayPoint> WayPointsToCheck
    {
        get { return wayPointsToCheck; }
        set { wayPointsToCheck = value; }
    }
    LayerMask wayPointLayer;

    private void Start()
    {
        wayPointLayer = gameObject.GetComponent<LayerMask>();
    }

    //STEP 1: check the nearest waypoints in Corridors
    void CheckNearWaypointsInCorridor()
    {
        foreach (LevelGeneration.WayPoint element in wayPointsToCheck)
        {

        }
    }

    void CheckPosition(LevelGeneration.WayPoint element, Vector3 direction)
    {
        //Ray ray = new Ray(element.Position, direction);
        RaycastHit hit;
        if (Physics.Raycast(element.Position, direction, out hit, Mathf.Infinity, wayPointLayer))
        {
            element.NearWayPoints.Add(hit.collider.gameObject.GetComponent<WayPointComponents>().ObjectWaypoint);
        }
    }

}
