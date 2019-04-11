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
        public Waypoint(GameObject waypointConstruct, GameObject actualWaypoint, GameObject playerWaypoint, Waypoint parentConstruct)
        {
            waypoint = waypointConstruct;
            parent = parentConstruct;
            CalculateCost(waypoint, actualWaypoint, playerWaypoint, parentConstruct);

        }

        public void CalculateCost(GameObject waypointConstruct, GameObject actualWaypoint, GameObject playerWaypoint, Waypoint parent)
        {

            float distancePlayer;
            float distanceEnemy;
            distancePlayer = Vector3.Distance(waypoint.transform.position, playerWaypoint.transform.position);
            distanceEnemy = Vector3.Distance(waypoint.transform.position, actualWaypoint.transform.position);
            cost = distancePlayer + distanceEnemy + parent.cost;
        }

        public float Cost
        {
            get { return cost; }
            set { cost = value; }
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

    List<Waypoint> closedList = new List<Waypoint>(); //List where the waypoints already passed
    List<Waypoint> openList = new List<Waypoint>(); //List of the accessible waypoints
    List<GameObject> finalList = new List<GameObject>();
    GameObject enemy;
    GameObject player;
    [SerializeField] GameObject enemyWaypointObject;
    [SerializeField] GameObject playerWaypointObject;
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

    Waypoint CalculateOptimalWaypoint(List<Waypoint> listToCalculate, List<Waypoint> elementsToBan)
    {
        Waypoint optimalWaypoint = new Waypoint(listToCalculate[0].WaypointGameobject, enemy, player);
        optimalWaypoint.Cost = 100000000000f;
        foreach(Waypoint element in listToCalculate)
        {
            bool calculate = true;
            foreach (Waypoint element2 in elementsToBan)
            {
                if (element.WaypointGameobject == element2.WaypointGameobject)
                {
                    calculate = false;
                }
            }
            if (element.Cost < optimalWaypoint.Cost && calculate)
            {
                optimalWaypoint = element;
            }
        }
        return optimalWaypoint;
    }

    public List<GameObject> AddToList()
    {
        closedList.Clear();
        openList.Clear();
        finalList.Clear();
        enemyWaypoint = (new Waypoint(enemyWaypointObject, enemy, player));
        foreach (GameObject element in enemyWaypointScript.NearWaypoints)
        {
            openList.Add(new Waypoint(element, enemy, player, enemyWaypoint));
        }
        closedList.Add(enemyWaypoint);
        bool pathIsDone = false;
        Waypoint actualwaypoint = CalculateOptimalWaypoint(openList, closedList);
        int failSafe = 0;
        while (!pathIsDone && failSafe++ < 500)
        {
            closedList.Add(actualwaypoint);
            if (actualwaypoint.WaypointGameobject.tag == "Player")
            {
                //Debug.Log(actualwaypoint.Parent);
                pathIsDone = true;
            }
            else
            {
                foreach (GameObject element in actualwaypoint.WaypointGameobject.GetComponent<WayPointComponents>().NearWayPoints)
                {
                    bool canAdd = true;
                    foreach(Waypoint element2 in closedList)
                    {
                        if (element2.WaypointGameobject == element)
                        {
                            canAdd = false;
                        }
                    }
                    foreach(Waypoint element2 in openList)
                    {
                        if (element2.WaypointGameobject == element)
                        {
                            canAdd = false;
                        }
                    }
                    if (canAdd == true)
                    {
                        openList.Add(new Waypoint(element, actualwaypoint.WaypointGameobject, player, actualwaypoint));
                    }
                }
                //Debug.Log("exit while");
                actualwaypoint = CalculateOptimalWaypoint(openList, closedList);
                /*Debug.Log("Beguin check closed");
                foreach (Waypoint element in closedList)
                {
                    Debug.Log(element.WaypointGameobject.name);
                }
                Debug.Log("endcheck");*/
                /*Debug.Log("Beguin check open");
                foreach (Waypoint element in openList)
                {
                    Debug.Log(element.WaypointGameobject.name);
                }
                Debug.Log("endcheck");*/

            }
        }
        if (failSafe >= 500)
        {
            Debug.LogError("failSafe out of range");
        }
        finalList = GeneratePath(actualwaypoint);
        return finalList;
    }

    List<GameObject> GeneratePath(Waypoint endpath)
    {
        //Debug.Log("Generate Path");
        bool listCompleted = false;
        List<GameObject> path = new List<GameObject>();
        Waypoint actualWaypoint = endpath;
        int failSafe = 0;
        while (!listCompleted && failSafe++ < 500)
        {
            //Debug.Log(actualWaypoint.Parent.WaypointGameobject.name);
            //Debug.Log(actualWaypoint.WaypointGameobject);
            if (actualWaypoint.WaypointGameobject.tag == "Enemy")
            {
                listCompleted = true;
            }
            else
            {
                path.Add(actualWaypoint.WaypointGameobject);
                //Debug.Log(actualWaypoint.Parent.WaypointGameobject.name + "is the Parent of" + actualWaypoint.WaypointGameobject.name);
                actualWaypoint = actualWaypoint.Parent;
            }
        }
        if (failSafe >= 500)
        {
            Debug.LogError("failSafe out of range");
        }
        foreach (GameObject element in path)
        {
            //Debug.Log(element);
        }
        return path;
    }
}
