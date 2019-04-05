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
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsInside = true;
            other.GetComponent<PlayerWaypoint>().NearWaypoints.Clear();
            //other.GetComponent<PlayerWaypoint>().RoomWaypoints.Clear();
            other.GetComponent<PlayerWaypoint>().RoomWaypoints = roomWaypoints;
        }
        else
        {
            playerIsInside = false;
        }

        if (other.tag == "Enemy")
        {
            enemyIsInside = true;
            other.GetComponent<Enemy>().RoomWaypoints.Clear();
            other.GetComponent<Enemy>().RoomWaypoints = roomWaypoints;
        }
        else
        {
            enemyIsInside = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerWaypoint>().IsInARoom = true;
        }

        if (other.tag == "Enemy")
        {
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
           other.GetComponent<PlayerWaypoint>().IsInARoom = false;
        }

        if (other.tag == "Enemy")
        {
        }
    }
}
