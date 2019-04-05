using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float enemySpeed;
    [SerializeField] float enemyRunSpeedMultiplayer;
    [SerializeField] float enemyWalkSpeedMultiplayer;
    List<GameObject> waypoints = new List<GameObject>();
    // Start is called before the first frame update

    [SerializeField] List<GameObject> roomWaypoints = new List<GameObject>();

    public List<GameObject> RoomWaypoints
    {
        get { return roomWaypoints; }
        set { roomWaypoints = value; }
    }

    void Start()
    {
        waypoints = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().waypoints;
    }

    //Patrol

    void Patrol()
    {
        foreach(GameObject element in waypoints)
        {
            
        }
    }
}
