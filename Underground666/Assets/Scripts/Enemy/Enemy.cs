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
    bool afterStart = true;
    bool afterFrame = false;

    //Patrolling
    private void Start()
    {

    }

    private void FixedUpdate()
    {
        //enemyWaypointMoving.ForFixedUpdate();
        if (afterFrame)
        {
            CalculatePathfinding();
            afterFrame = false;
        }
        if(afterStart)
        {
            afterFrame = true;
            afterStart = false;
        }
    }

    //Running
    void CalculatePathfinding()
    {
        WayPointPath = pathfinding.AddToList();
    }

}
