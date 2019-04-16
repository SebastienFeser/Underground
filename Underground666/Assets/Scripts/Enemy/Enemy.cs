using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField] PathFinding pathfinding;
    [SerializeField] WaypointMoving enemyWaypointMoving;
    [SerializeField] List<GameObject> WayPointPath;
    [SerializeField] Rigidbody enemyRigidBody;
    [SerializeField] float speed;
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
        if(afterFrame)
        {
            CalculatePathfinding();
        }
        enemyWaypointMoving.UpdateWaypointDetection();
        if (afterStart)
        {
            afterFrame = true;
            afterStart = false;
        }
        if (WayPointPath.Count > 0)
        {
            transform.position = Vector3.MoveTowards(new Vector3 (transform.position.x, transform.position.y, transform.position.z), new Vector3(WayPointPath[WayPointPath.Count - 1].transform.position.x, transform.position.y, WayPointPath[WayPointPath.Count - 1].transform.position.z), Time.deltaTime * speed);
        }
        
    }

    //Running
    void CalculatePathfinding()
    {
        WayPointPath = pathfinding.AddToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SceneManager.LoadScene("JumpScare");
        }
    }

}
