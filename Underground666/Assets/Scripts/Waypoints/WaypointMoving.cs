using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMoving : MonoBehaviour
{
    [SerializeField] List<GameObject> roomWaypoints = new List<GameObject>();
    [SerializeField] List<BoxCollider> roomColliders = new List<BoxCollider>();
    [SerializeField] List<GameObject> corridorWaypoints = new List<GameObject>();
    [SerializeField] List<GameObject> waypointsToCalculate = new List<GameObject>();
    [SerializeField] List<GameObject> nearWaypoints = new List<GameObject>();               //Waypoints that are visible to the actual static waypoint
    //List<GameObject> nearWaypointsBackup = new List<GameObject>();                        //Solution Temporaire
    [SerializeField] List<WayPointComponents> allWaypointComponents = new List<WayPointComponents>();
    private GameObject waypointEnemyOrPlayer;
    List<Ray> rayToCastInRoom;
    //bool isInARoom;
    //bool roomElementDeleteBool = true;

    public List<GameObject> RoomWaypoints
    {
        get { return roomWaypoints; }
        set { roomWaypoints = value; }
    }

    /*public bool IsInARoom
    {
        get { return isInARoom; }
        set { isInARoom = value; }
    }*/

    public List<GameObject> NearWaypoints
    {
        get { return nearWaypoints; }
        set { nearWaypoints = value; }
    }

    public List<WayPointComponents> AllWaypointComponents
    {
        get { return allWaypointComponents; }
        set { allWaypointComponents = value; }
    }

    public List<GameObject> CorridorWaypoints
    {
        get { return corridorWaypoints; }
        set { corridorWaypoints = value; }
    }

    public GameObject EnemyOrPlayer
    {
        get { return this.waypointEnemyOrPlayer; }
        set { this.waypointEnemyOrPlayer = value; }
    }

    private void Start()
    {
        //corridorWaypoints = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().corridorWaypoints;
        //allWaypointComponents = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().allWaypointComponents;
    }


    public void UpdateWaypointDetection()
    {
       //Draw Line between dynamic waypoints and static waypoints that are visible and belong to the same room as the actual dynamic waypoint
        foreach (GameObject element in nearWaypoints)
        {
            Debug.DrawLine(transform.position, element.transform.position, Color.black);
        }
        if (nearWaypoints.Count != 0)
        {
            /*nearWaypointsBackup.Clear();
            foreach (GameObject element in nearWaypoints)
            {
                nearWaypointsBackup.Add(element);
            }*/
        }
        //Clear the lists
        nearWaypoints.Clear();
        waypointsToCalculate.Clear();

        //Remove the dynamic waypoint to the static waypoints list
        foreach(WayPointComponents element in allWaypointComponents)
        {
            element.NearWayPoints.Remove(gameObject);
        }

        //Selects all the Waypoints who's visibillity needs to be tested
        /*if (isInARoom)      //Check if the dynamic waypoint is in a room
        {
            //roomElementDeleteBool = true;

            //Add all the static waypoints belonging to the room detected in the WaypointsToCalculate list
            foreach (GameObject element in roomWaypoints)
            {
                waypointsToCalculate.Add(element);
            }
        }*/
        //else                //Check if not in a room, dynamic waypoint is in a corridor
        {
            /*if (roomElementDeleteBool)
            {
                roomElementDeleteBool = false;
            }*/
            foreach (GameObject element in corridorWaypoints)
            {
                waypointsToCalculate.Add(element);
            }
            foreach (GameObject element in roomWaypoints)
            {
                waypointsToCalculate.Add(element);
            }
        }

        waypointsToCalculate.Add(waypointEnemyOrPlayer); //Add the player waypoint and the enemy waypoint to the waypoint 

        //Check and add to the nearwaypoint list the waypoints that are visible to the actual dynamic waypoint
        foreach (GameObject element in waypointsToCalculate)
        {
            //Cast a ray in all the waypoints to calculate direction
            RaycastHit hit;
            if (Physics.Raycast(transform.position, element.transform.position - transform.position, out hit, Vector3.Distance(transform.position, element.transform.position)))
            {
                if (hit.collider.tag == "WayPointRoom" || hit.collider.tag == "WayPointCorridor")   //Check if it hits a static Waypoint
                {
                    nearWaypoints.Add(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<WayPointComponents>().NearWayPoints.Add(gameObject);   //Adding the actual dynamic Waypoint to the Static Waypoint's Nearwaypoint list
                }
                else if (hit.collider.tag == waypointEnemyOrPlayer.tag)                             //Check if it hits a dynamic Waypoint
                {
                    nearWaypoints.Add(hit.collider.gameObject);
                    hit.collider.gameObject.GetComponent<WaypointMoving>().NearWaypoints.Add(gameObject);       //Adding the actual dynamic Waypoint to the Dynamic Waypoint's Nearwaypoint list
                }
            }
        }

        if (nearWaypoints.Count == 0)
        {
            /*foreach (GameObject element in nearWaypointsBackup)
            {
                nearWaypoints.Add(element);
            }*/
        }
    }

    
    /*private void FixedUpdate()
    {

        foreach (GameObject element in nearWaypoints)
        {
            Debug.DrawLine(transform.position, element.transform.position, Color.black);
        }
        nearWaypoints.Clear();
        if (isInARoom)
        {
            roomElementDeleteBool = true;
            waypointsToCalculate = roomWaypoints;
            Debug.Log("lol1");
            Debug.Log(roomWaypoints);
        }
        else
        {
            if (roomElementDeleteBool)
            {
                roomElementDeleteBool = false;
            }
            waypointsToCalculate = corridorWaypoints;
            Debug.Log("lol2");
        }
        waypointsToCalculate.Add(waypointEnemyOrPlayer);

        foreach (GameObject element in waypointsToCalculate)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, element.transform.position - transform.position, out hit, Vector3.Distance(transform.position, element.transform.position)))
            {
                if (hit.collider.tag == "Wall")
                {
                    break;
                }
                else if (hit.collider.tag == "WayPointRoom" || hit.collider.tag == "WayPointCorridor" || hit.collider.tag == waypointEnemyOrPlayer.tag)
                {
                    nearWaypoints.Add(hit.collider.gameObject);
                }
            }
        }
    }*/
}
