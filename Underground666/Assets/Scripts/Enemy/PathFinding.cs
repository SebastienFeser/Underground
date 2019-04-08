using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    class Waypoint
    {
        public Waypoint(GameObject waypointConstruct, GameObject enemyWaypoint, GameObject playerWaypoint)
        {
            waypoint = waypointConstruct;

        }
        public Waypoint(GameObject waypointConstruct, GameObject enemyWaypoint, GameObject playerWaypoint, Waypoint parentConstruct)
        {
            waypoint = waypointConstruct;
            parent = parentConstruct;

        }

        public void CalculateCost(GameObject waypointConstruct, GameObject enemyWaypoint, GameObject playerWaypoint)
        {

            float distancePlayer;
            float distanceEnemy;
            distancePlayer = Vector3.Distance(waypoint.transform.position, playerWaypoint.transform.position);
            distanceEnemy = Vector3.Distance(waypoint.transform.position, enemyWaypoint.transform.position);
            cost = distancePlayer + distanceEnemy;
        }

        public float Cost
        {
            get { return cost; }
        }

        public Waypoint Parent
        {
            get { return parent; }
            set { parent = value;}
        }

        public GameObject WaypointGameobject
        {
            get { return waypoint; }
        }

        GameObject waypoint;
        Waypoint parent;
        float cost;
    }

    [SerializeField] List<GameObject> allWaypointGameObject = new List<GameObject>();
    List<Waypoint> allWaypoint = new List<Waypoint>();
    List<Waypoint> closedList = new List<Waypoint>(); //List where the waypoints already passed
    List<Waypoint> openList = new List<Waypoint>(); //List of the accessible waypoints
    List<GameObject> finalList = new List<GameObject>();
    GameObject enemy;
    GameObject player;
    [SerializeField] WaypointMoving enemyWaypointScript;
    WaypointMoving playerWaypointScript;
    Waypoint enemyWaypoint;


    private void Start()
    {
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");
        //enemyWaypointScript = enemy.GetComponent<WaypointMoving>();
        playerWaypointScript = enemy.GetComponent<WaypointMoving>();
        enemyWaypoint = new Waypoint(enemy, enemy, player);
    }

    Waypoint CalculateOptimalWaypoint(List<Waypoint> listToCalculate, Waypoint parent)
    {
        Waypoint optimalWaypoint = listToCalculate[0];
        foreach(Waypoint element in listToCalculate)
        {
            if (element.Cost < optimalWaypoint.Cost)
            {
                optimalWaypoint = element;
            }
        }
        optimalWaypoint.Parent = parent;
        return optimalWaypoint;
    }

    public List<GameObject> AddToList()
    {
        foreach (GameObject element in allWaypointGameObject)
        {
            allWaypoint.Add(new Waypoint(element, enemy, player));
        }
        foreach (GameObject element in enemyWaypointScript.NearWaypoints)
        {
            openList.Add(new Waypoint(element, enemy, player, enemyWaypoint));
        }
        bool pathIsDone = false;
        Waypoint actualwaypoint = CalculateOptimalWaypoint(openList, enemyWaypoint);
        while (!pathIsDone)
        {
            closedList.Add(new Waypoint(actualwaypoint.WaypointGameobject, enemy, player));
            foreach(GameObject element in actualwaypoint.WaypointGameobject.GetComponent<WayPointComponents>().NearWayPoints)
            {
                openList.Add(new Waypoint(element, enemy, player, actualwaypoint));
            }
            if (actualwaypoint.WaypointGameobject.tag == "Player")
            {
                pathIsDone = true;
            }
            else
            {
                actualwaypoint = CalculateOptimalWaypoint(openList, actualwaypoint);
            }

        }
        finalList = GeneratePath(actualwaypoint);
        return finalList;
    }

    List<GameObject> GeneratePath(Waypoint endpath)
    {
        bool listCompleted = false;
        List<GameObject> path = new List<GameObject>();
        Waypoint actualWaypoint = endpath;
        while (!listCompleted)
        {
            if (actualWaypoint.WaypointGameobject.tag == "Enemy")
            {
                listCompleted = true;
            }
            else
            {
                path.Add(actualWaypoint.WaypointGameobject);
                actualWaypoint = actualWaypoint.Parent;
            }
        }
        return path;
    }
}
