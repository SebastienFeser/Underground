using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWaypoint : MonoBehaviour
{
    [SerializeField] List<GameObject> roomWaypoints = new List<GameObject>();
    [SerializeField] List<BoxCollider> roomColliders = new List<BoxCollider>();
    [SerializeField] List<GameObject> corridorWaypoints = new List<GameObject>();
    [SerializeField] List<GameObject> waypointsToCalculate = new List<GameObject>();
    [SerializeField] List<GameObject> nearWaypoints = new List<GameObject>();
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

    private void FixedUpdate()
    {

        foreach (GameObject element in nearWaypoints)
        {
            Debug.DrawLine(transform.position, element.transform.position, Color.black);
        }
        nearWaypoints.Clear();
        if(isInARoom)
        {
            roomElementDeleteBool = true;
            //waypointsToCalculate.Clear();
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
            //waypointsToCalculate.Clear();
            waypointsToCalculate = corridorWaypoints;
            Debug.Log("lol2");
        }

        foreach(GameObject element in waypointsToCalculate)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, element.transform.position - transform.position, out hit, Vector3.Distance(transform.position, element.transform.position)))
            {
                if (hit.collider.tag == "Wall")
                {
                    break;
                }
                else if (hit.collider.tag == "WayPointRoom" || hit.collider.tag == "WayPointCorridor")
                {
                    nearWaypoints.Add(hit.collider.gameObject);
                }
            }
        }
    }
}
