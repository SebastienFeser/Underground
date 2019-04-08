using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomColliders : MonoBehaviour
{
    bool playerIsInside;
    bool enemyIsInside;
    [SerializeField] List<GameObject> roomWaypoints = new List<GameObject>();

    private void Start()
    {
    }

    private void Update()
    {
        if (playerIsInside)
        {

        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            other.GetComponent<WaypointMoving>().IsInARoom = true;
            other.GetComponent<WaypointMoving>().RoomWaypoints = roomWaypoints;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
           other.GetComponent<WaypointMoving>().IsInARoom = false;
        }
    }
}
