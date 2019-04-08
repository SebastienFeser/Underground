using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMoving : MonoBehaviour
{
    [SerializeField] List<GameObject> roomWaypoints = new List<GameObject>();
    [SerializeField] List<BoxCollider> roomColliders = new List<BoxCollider>();
    [SerializeField] List<GameObject> corridorWaypoints = new List<GameObject>();
    [SerializeField] List<GameObject> waypointsToCalculate = new List<GameObject>();
    [SerializeField] List<GameObject> nearWaypoints = new List<GameObject>();
    [SerializeField] GameObject waypointEnemyOrPlayer;
    List<Ray> rayToCastInRoom;
    bool isInARoom;
    bool roomElementDeleteBool = true;

    public List<GameObject> RoomWaypoints
    {
        get { return roomWaypoints; }
        set { roomWaypoints = value; }
    }

    public bool IsInARoom
    {
        get { return isInARoom; }
        set { isInARoom = value; }
    }

    public List<GameObject> NearWaypoints
    {
        get { return nearWaypoints; }
        set { nearWaypoints = value; }
    }


    private void Start()
    {
        corridorWaypoints = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().corridorWaypoints;
    }


    public void ForFixedUpdate()
    {
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
                        hit.collider.gameObject.GetComponent<WayPointComponents>().NearWayPoints.Add(gameObject);
                    }
                }
            }
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
