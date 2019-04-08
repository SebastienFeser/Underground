using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] PathFinding pathfinding;
    [SerializeField] WaypointMoving enemyWaypointMoving;
    [SerializeField] List<GameObject> WayPointPath;
    public enum Behaviour
    {
        PATROLLING,
        RUNNING,
        SEARCHING,
        FIND_PATROL_PATH
    }

    //Patrolling

    private void FixedUpdate()
    {
        enemyWaypointMoving.ForFixedUpdate();
        CalculatePathfinding();
    }
    //Running
    void CalculatePathfinding()
    {
        WayPointPath = pathfinding.AddToList();
    }



}
