﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{

    int cellMapSize = 36;
    int positionCorrection = 2;
    float wayPointDoorCenterCorrection = 0.5f;
    float wayPointDoorCorridorCorrection = 1.5f;
    float wayPointDoorRoomCorrection = 0.5f;

    float rayDetectionLength = Mathf.Infinity;

    enum CellType       //Cells represent each square of size (1,1) of the map
    {
        NORMAL,
        NORMAL_SMALLEST,
        WALL_UP,
        WALL_DOWN,
        WALL_LEFT,
        WALL_RIGHT,
        CORNER_LEFT_UP,
        CORNER_LEFT_DOWN,
        CORNER_RIGHT_UP,
        CORNER_RIGHT_DOWN,
        DOOR_UP,
        DOOR_DOWN,
        DOOR_LEFT,
        DOOR_RIGHT,
        WINDOW_UP,
        WINDOW_DOWN,
        WINDOW_LEFT,
        WINDOW_RIGHT,
        WINDOW_END_NORTH_LEFT,
        WINDOW_END_NORTH_RIGHT,
        WINDOW_END_SOUTH_LEFT,
        WINDOW_END_SOUTH_RIGHT,
        WINDOW_END_EAST_LEFT,
        WINDOW_END_EAST_RIGHT,
        WINDOW_END_WEST_LEFT,
        WINDOW_END_WEST_RIGHT,
        CORRIDOR,
    }

    enum RoomPosition
    {
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    [SerializeField] GameObject normalCell;
    [SerializeField] GameObject cornerCell;
    [SerializeField] GameObject normalSmallestCell;
    [SerializeField] GameObject doorCell;
    [SerializeField] GameObject windowsEnd;
    [SerializeField] GameObject windows;
    [SerializeField] GameObject wallUp;
    [SerializeField] GameObject wallDown;
    [SerializeField] GameObject wallLeft;
    [SerializeField] GameObject wallRight;
    [SerializeField] GameObject cornerLeftUp;
    [SerializeField] GameObject cornerLeftDown;
    [SerializeField] GameObject cornerRightUp;
    [SerializeField] GameObject cornerRightDown;
    [SerializeField] GameObject doorUp;
    [SerializeField] GameObject doorDown;
    [SerializeField] GameObject doorLeft;
    [SerializeField] GameObject doorRight;
    [SerializeField] GameObject player;
    [SerializeField] GameObject wayPointCorridor;
    [SerializeField] GameObject wayPointRoom;


    public class WayPoint
    {
        public WayPoint(Vector2 positionConstruct)
        {
            position = positionConstruct;
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public List<WayPoint> NearWayPoints
        {
            get { return nearWayPoints; }
            set { nearWayPoints = value; }
        }

        Vector2 position;
        List<WayPoint> nearWayPoints = new List<WayPoint>();
    }

    class Cell
    {
        public Cell(Vector2 positionConstruct, CellType typeConstruct)
        {
            position = positionConstruct;
            type = typeConstruct;
        }


        public CellType Type
        {
            get { return type; }
        }

        public Vector2 Position
        {
            get { return position; }
        }


        Vector2 position;
        CellType type;
    }

    class Corridor
    {

        public Corridor(Vector2 sizeConstruct, Vector2 centerConstruct)
        {
            size = sizeConstruct;
            position = new Vector2(centerConstruct.x, centerConstruct.y);
        }

        public Vector2 Position
        {
            get { return position; }

        }

        public Vector2 Size
        {
            get { return size; }
        }

        private Vector2 position;
        private Vector2 size;
    }

    class SquareRoom
    {
        public SquareRoom(Vector2 sizeConstruct, Vector2 centerConstruct)
        {
            size = sizeConstruct;
            position = new Vector2(centerConstruct.x, centerConstruct.y);
        }

        public SquareRoom(Vector2 sizeConstruct, Vector2 centerConstruct, bool isTheSmallestConstruct, bool isRoomNorthConstruct, bool isRoomSouthConstruct, bool isRoomEastConstruct, bool isRoomWestConstruct)
        {
            size = sizeConstruct;
            position = new Vector2(centerConstruct.x, centerConstruct.y);
            isTheSmallest = isTheSmallestConstruct;
            isRoomNorth = isRoomNorthConstruct;
            isRoomSouth = isRoomSouthConstruct;
            isRoomEast = isRoomEastConstruct;
            isRoomWest = isRoomWestConstruct;
            isUnCutRoom = isTheSmallestConstruct;



        }

        public SquareRoom(Vector2 sizeConstruct, Vector2 centerConstruct, bool isTheSmallestConstruct, bool isRoomNorthConstruct, bool isRoomSouthConstruct, bool isRoomEastConstruct, bool isRoomWestConstruct, bool isUnCutRoomConstruct)
        {
            size = sizeConstruct;
            position = new Vector2(centerConstruct.x, centerConstruct.y);
            isTheSmallest = isTheSmallestConstruct;
            isRoomNorth = isRoomNorthConstruct;
            isRoomSouth = isRoomSouthConstruct;
            isRoomEast = isRoomEastConstruct;
            isRoomWest = isRoomWestConstruct;
            isUnCutRoom = isUnCutRoomConstruct;



        }

        public Vector2 Position
        {
            get { return position; }

        }

        public Vector2 Size
        {
            get { return size; }
        }

        public List<Cell> Cells
        {
            get { return cells; }
            set { cells = value; }
        }

        public bool IsTheSmallest
        {
            get { return isTheSmallest; }
            set { isTheSmallest = value; }
        }

        public bool IsRoomNorth
        {
            get { return isRoomNorth; }
        }

        public bool IsRoomSouth
        {
            get { return isRoomSouth; }
        }

        public bool IsRoomEast
        {
            get { return isRoomEast; }
        }

        public bool IsRoomWest
        {
            get { return isRoomWest; }
        }

        public bool IsUnCutRoom
        {
            get { return isUnCutRoom; }
        }

        private Vector2 position;
        private Vector2 size;
        private List<Cell> cells = new List<Cell>();
        private bool isTheSmallest = false;
        private bool isRoomNorth = false;
        private bool isRoomSouth = false;
        private bool isRoomEast = false;
        private bool isRoomWest = false;
        private bool isUnCutRoom = false;

    }

    





    //STEP 1: create a MAP 30*30 (with lines 0 to 30), a MAINSQUARES list, a QUADRISQUARES list, a NORMALSQUARES list, a ROOMS list
    Vector2 map = new Vector2(30, 30);
    List<SquareRoom> mainSquares = new List<SquareRoom>();
    List<SquareRoom> quadriSquares = new List<SquareRoom>();
    List<SquareRoom> normalSquares = new List<SquareRoom>();
    List<SquareRoom> rooms = new List<SquareRoom>();
    List<Corridor> corridors = new List<Corridor>();
    List<WayPoint> corridorsWaypoints = new List<WayPoint>();
    List<WayPoint> roomWaypoints = new List<WayPoint>();
    List<GameObject> corridorWaypointsGameobjects = new List<GameObject>();
    List<GameObject> roomWaypointsGameobjects = new List<GameObject>();
    [SerializeField] GameObject cube;
    [SerializeField] GameObject corridorObject;
    int corridorDifference = 2;
    int wayPointCorrection = 1;

    bool canSpace = true;

    public List<WayPoint> CorridorWaypoints
    {
        get { return corridorsWaypoints; }
        set { corridorsWaypoints = value; }
    }

    public List<WayPoint> RoomWaypoints
    {
        get { return roomWaypoints; }
        set { roomWaypoints = value; }
    }

    private void Start()
    {
        StartCoroutine("BuildMap");
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && canSpace)
        {
            StartCoroutine("BuildMap");
        }

        /*foreach (WayPoint element in corridorsWaypoints)
        {
            Debug.DrawRay(new Vector3 (element.Position.x, 1.2f, element.Position.y), Vector3.back * 20);
            Debug.DrawRay(new Vector3 (element.Position.x, 1.2f, element.Position.y), Vector3.forward * 20);
            Debug.DrawRay(new Vector3 (element.Position.x, 1.2f, element.Position.y), Vector3.left * 20);
            Debug.DrawRay(new Vector3 (element.Position.x, 1.2f, element.Position.y), Vector3.right * 20);
        }*/
    }

    IEnumerator BuildMap()
    {
        canSpace = false;
        foreach (Transform child in gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        mainSquares.Clear();
        quadriSquares.Clear();
        normalSquares.Clear();
        rooms.Clear();
        corridors.Clear();
        corridorsWaypoints.Clear();
        corridorWaypointsGameobjects.Clear();
        roomWaypoints.Clear();
        roomWaypointsGameobjects.Clear();
        for (int i = 0; i < cellMapSize; i++)
        {
            for (int j = 0; j < cellMapSize; j++)
            {
                cell[i, j] = null;
            }
        }
        CutMap();
        CutMainSquares();
        AddCorridorsAround();
        yield return new WaitForSeconds(0.5f);
        CutQuadriSquares();
        CutNormalSquares();
        TransfromRoomsInCells();
        TransformCorridorsInCells();
        SpawnDoorsInSmallRoom();
        SpawnDoorsLinkedToCorridor();
        SpawnWindows();
        foreach (Cell element in cell)
        {
            if (element.Type == CellType.CORNER_RIGHT_UP)
            {
                GameObject actualGameObject = Instantiate(cornerRightUp);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.CORNER_RIGHT_DOWN)
            {
                GameObject actualGameObject = Instantiate(cornerRightDown);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.CORNER_LEFT_UP)
            {
                GameObject actualGameObject = Instantiate(cornerLeftUp);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.CORNER_LEFT_DOWN)
            {
                GameObject actualGameObject = Instantiate(cornerLeftDown);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.WALL_UP)
            {
                GameObject actualGameObject = Instantiate(wallUp);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.WALL_DOWN)
            {
                GameObject actualGameObject = Instantiate(wallDown);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.WALL_LEFT)
            {
                GameObject actualGameObject = Instantiate(wallLeft);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.WALL_RIGHT)
            {
                GameObject actualGameObject = Instantiate(wallRight);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.NORMAL)
            {
                GameObject actualGameObject = Instantiate(normalCell);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.CORRIDOR)
            {
                GameObject actualGameObject = Instantiate(corridorObject);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.NORMAL_SMALLEST)
            {
                {
                    GameObject actualGameObject = Instantiate(normalSmallestCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
                }
            }
            else if (element.Type == CellType.DOOR_UP)
            {
                GameObject actualGameObject = Instantiate(doorUp);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.DOOR_DOWN)
            {
                GameObject actualGameObject = Instantiate(doorDown);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.DOOR_LEFT)
            {
                GameObject actualGameObject = Instantiate(doorLeft);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.DOOR_RIGHT)
            {
                GameObject actualGameObject = Instantiate(doorRight);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.WINDOW_END_NORTH_LEFT || element.Type == CellType.WINDOW_END_NORTH_RIGHT || element.Type == CellType.WINDOW_END_SOUTH_LEFT || element.Type == CellType.WINDOW_END_SOUTH_RIGHT ||
                element.Type == CellType.WINDOW_END_EAST_LEFT || element.Type == CellType.WINDOW_END_EAST_RIGHT || element.Type == CellType.WINDOW_END_WEST_LEFT || element.Type == CellType.WINDOW_END_WEST_RIGHT)
            {
                GameObject actualGameObject = Instantiate(windowsEnd);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else if (element.Type == CellType.WINDOW_UP || element.Type == CellType.WINDOW_DOWN || element.Type == CellType.WINDOW_LEFT || element.Type == CellType.WINDOW_RIGHT)
            {
                GameObject actualGameObject = Instantiate(windows);
                actualGameObject.transform.parent = gameObject.transform;
                actualGameObject.transform.position = new Vector3(element.Position.x, 0, element.Position.y);
            }
            else
            {
                
            }
            

        }
        foreach (WayPoint element in corridorsWaypoints)
        {
            GameObject actualGameObject = Instantiate(wayPointCorridor);
            actualGameObject.GetComponent<WayPointComponents>().ObjectWaypoint = element;
            actualGameObject.transform.parent = gameObject.transform;
            actualGameObject.transform.position = new Vector3(element.Position.x, 1.2f, element.Position.y);
            corridorWaypointsGameobjects.Add(actualGameObject);
        }
        foreach (WayPoint element in roomWaypoints)
        {
            GameObject actualGameObject = Instantiate(wayPointRoom);
            actualGameObject.GetComponent<WayPointComponents>().ObjectWaypoint = element;
            actualGameObject.transform.parent = gameObject.transform;
            actualGameObject.transform.position = new Vector3(element.Position.x, 1.2f, element.Position.y);
            roomWaypointsGameobjects.Add(actualGameObject);
        }
        yield return new WaitForFixedUpdate();
        CorridorNearWayPointDetection();
        
        canSpace = true;
    }

    //STEP 2: Cut the MAP vertically between line 12 and 18 to get the MAIN SQUARES
    void CutMap()
    {
        int random = Random.Range(12, 19);
        mainSquares.Add(new SquareRoom(new Vector2(map.x, random), new Vector2(0, 0)));//Magic numbers
        mainSquares.Add(new SquareRoom(new Vector2(map.x, map.y - random), new Vector2(mainSquares[0].Position.x, mainSquares[0].Position.y + random + corridorDifference)));
        corridors.Add(new Corridor(new Vector2(map.x + corridorDifference, corridorDifference), new Vector2(mainSquares[0].Position.x, random)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(mainSquares[0].Position.x + wayPointCorrection, random + wayPointCorrection + corridorDifference)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(mainSquares[0].Position.x + map.x - wayPointCorrection + corridorDifference * 3, random +  wayPointCorrection  + corridorDifference)));
    }

    //STEP 3: Cut the the MAIN SQUARES horizontally between line 12 and 18 to get the QUADRI SQUARES
    void CutMainSquares()
    {
        foreach (SquareRoom element in mainSquares)
        {
            int random = Random.Range(12, 19);
            quadriSquares.Add(new SquareRoom(new Vector2(random, element.Size.y), new Vector2(element.Position.x, element.Position.y)));
            quadriSquares.Add(new SquareRoom(new Vector2(element.Size.x - random, element.Size.y), new Vector2(element.Position.x + random + corridorDifference, element.Position.y)));
            corridors.Add(new Corridor(new Vector2(corridorDifference, element.Size.y), new Vector2(element.Position.x + random, element.Position.y)));

            corridorsWaypoints.Add(new WayPoint(new Vector2(random + wayPointCorrection + corridorDifference, element.Position.y + wayPointCorrection)));
            corridorsWaypoints.Add(new WayPoint(new Vector2(random + wayPointCorrection + corridorDifference, element.Position.y  + element.Size.y + wayPointCorrection + corridorDifference)));
        }
        //Check if 1 waypoint == another
        WayPoint wayPointToRemove = null;
        foreach (WayPoint element1 in corridorsWaypoints)
        {
            foreach (WayPoint element2 in corridorsWaypoints)
            {
                if(((int)element1.Position.x == (int)element2.Position.x) && ((int)element1.Position.y == (int)element2.Position.y) && (element1 != element2))
                {
                    wayPointToRemove = element2;
                }
            }
        }
        corridorsWaypoints.Remove(wayPointToRemove);
    }

    //STEP 3.5: Add corridors to corners
    void AddCorridorsAround()
    {
        corridors.Add(new Corridor(new Vector2(corridorDifference, map.x + corridorDifference * 3), new Vector2(0 - corridorDifference, 0 - corridorDifference)));
        corridors.Add(new Corridor(new Vector2(corridorDifference, map.x + corridorDifference * 3), new Vector2(map.x + corridorDifference, 0 - corridorDifference)));
        corridors.Add(new Corridor(new Vector2(map.x + corridorDifference, corridorDifference), new Vector2(0, 0 - corridorDifference)));
        corridors.Add(new Corridor(new Vector2(map.x + corridorDifference, corridorDifference), new Vector2(0, map.y + corridorDifference)));

        corridorsWaypoints.Add(new WayPoint(new Vector2(wayPointCorrection, wayPointCorrection)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(corridorDifference * 3 - wayPointCorrection + map.x, wayPointCorrection)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(wayPointCorrection, corridorDifference * 3 - wayPointCorrection + map.y)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(corridorDifference * 3 - wayPointCorrection + map.x, corridorDifference * 3 - wayPointCorrection + map.y)));
    }

    //STEP 4: Cut the QUADRI SQUARES horizontally or vertically to form the NORMAL SQUARES, there must be at least 6 lines away from the sides of the QUADRI SQUARES
    void CutQuadriSquares()                                     //room detection working
    {
        foreach (SquareRoom element in quadriSquares)
        {
            int cutHorizontal = Random.Range(0, 2);
            if (cutHorizontal > 0)
            {
                int random = Random.Range(6, (int)element.Size.x - 6);
                normalSquares.Add(new SquareRoom(new Vector2(random, element.Size.y), new Vector2(element.Position.x, element.Position.y), false, false, true, false, false, true));
                normalSquares.Add(new SquareRoom(new Vector2(element.Size.x - random, element.Size.y), new Vector2(element.Position.x + random, element.Position.y), false, true, false, false, false, true));
            }
            else
            {
                int random = Random.Range(6, (int)element.Size.y - 6);
                normalSquares.Add(new SquareRoom(new Vector2(element.Size.x, random), new Vector2(element.Position.x, element.Position.y), false, false, false, true, false, true));
                normalSquares.Add(new SquareRoom(new Vector2(element.Size.x, element.Size.y - random), new Vector2(element.Position.x, element.Position.y + random), false, false, false, false, true, true));
            }
        }

    }

    //STEP 5: Cut the biggest NORMAL SQUARES in each QUADRISQUARES horizontally or vertically to form the ROOMS. there must be at least 6 lines away from the sides of the NORMAL SQUARES.
    void CutNormalSquares()                                 //room detection not working
    {
        int j = 0;
        foreach (SquareRoom element in quadriSquares)
        {
            float area1;
            float area2;
            area1 = normalSquares[j].Size.x * normalSquares[j].Size.y;
            area2 = normalSquares[j + 1].Size.x * normalSquares[j + 1].Size.y;
            if (area1 > area2)
            {
                if (normalSquares[j].Size.x > normalSquares[j].Size.y)
                {
                    int random = Random.Range(6, (int)normalSquares[j].Size.x - 6);
                    if (random - 6 < ((int)normalSquares[j].Size.x - 6) - random)
                    {
                        rooms.Add(new SquareRoom(new Vector2(random, normalSquares[j].Size.y), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y), true, false, true, normalSquares[j].IsRoomEast, normalSquares[j].IsRoomWest));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x - random, normalSquares[j].Size.y), new Vector2(normalSquares[j].Position.x + random, normalSquares[j].Position.y), false, true, false, normalSquares[j].IsRoomEast, normalSquares[j].IsRoomWest));
                    }
                    else
                    {
                        rooms.Add(new SquareRoom(new Vector2(random, normalSquares[j].Size.y), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y), false, false, true, normalSquares[j].IsRoomEast, normalSquares[j].IsRoomWest));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x - random, normalSquares[j].Size.y), new Vector2(normalSquares[j].Position.x + random, normalSquares[j].Position.y), true, true, false, normalSquares[j].IsRoomEast, normalSquares[j].IsRoomWest));
                    }
                    rooms.Add(normalSquares[j + 1]);
                }
                /*else if (normalSquares[j].Position.x < normalSquares[j].Position.y)
                {
                    int random = Random.Range(6, (int)normalSquares[j].Size.y);
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x, random), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y)));
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x, normalSquares[j].Size.y - random), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y + random)));
                    rooms.Add(normalSquares[j + 1]);
                }*/
                else
                {
                    int random = Random.Range(6, (int)normalSquares[j].Size.y - 6);
                    if (random - 6 < ((int)normalSquares[j].Size.y - 6) - random)
                    {
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x, random), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y), true, normalSquares[j].IsRoomNorth, normalSquares[j].IsRoomSouth, true, false));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x, normalSquares[j].Size.y - random), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y + random), false, normalSquares[j].IsRoomNorth, normalSquares[j].IsRoomSouth, false, true));
                    }
                    else
                    {
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x, random), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y), false, normalSquares[j].IsRoomNorth, normalSquares[j].IsRoomSouth, true, false));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j].Size.x, normalSquares[j].Size.y - random), new Vector2(normalSquares[j].Position.x, normalSquares[j].Position.y + random), true, normalSquares[j].IsRoomNorth, normalSquares[j].IsRoomSouth, false, true));
                    }
                    rooms.Add(normalSquares[j + 1]);
                }
            }
            /*else if (area1 < area2)
            {
                if (normalSquares[j + 1].Position.x > normalSquares[j + 1].Position.y)
                {
                    int random = Random.Range(6, (int)normalSquares[j + 1].Size.x);
                    rooms.Add(new SquareRoom(new Vector2(random, normalSquares[j + 1].Size.y), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y)));
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x - random, normalSquares[j + 1].Size.y), new Vector2(normalSquares[j + 1].Position.x + random, normalSquares[j + 1].Position.y)));
                    rooms.Add(normalSquares[j]);
                }
                else if (normalSquares[j].Position.x < normalSquares[j].Position.y)
                {
                    int random = Random.Range(6, (int)normalSquares[j + 1].Size.y);
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y)));
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, normalSquares[j + 1].Size.y - random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y + random)));
                    rooms.Add(normalSquares[j]);
                }
                else
                {
                    int random = Random.Range(6, (int)normalSquares[j + 1].Size.y);
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y)));
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, normalSquares[j + 1].Size.y - random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y + random)));
                    rooms.Add(normalSquares[j]);
                }
            }*/
            else
            {
                if (normalSquares[j + 1].Size.x > normalSquares[j + 1].Size.y)
                {
                    int random = Random.Range(6, (int)normalSquares[j + 1].Size.x - 6);
                    if (random - 6 < ((int)normalSquares[j + 1].Size.x - 6) - 6)
                    {
                        rooms.Add(new SquareRoom(new Vector2(random, normalSquares[j + 1].Size.y), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y), true, false, true, normalSquares[j + 1].IsRoomEast, normalSquares[j + 1].IsRoomWest));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x - random, normalSquares[j + 1].Size.y), new Vector2(normalSquares[j + 1].Position.x + random, normalSquares[j + 1].Position.y), false, true, false, normalSquares[j + 1].IsRoomEast, normalSquares[j + 1].IsRoomWest));
                    }
                    else
                    {
                        rooms.Add(new SquareRoom(new Vector2(random, normalSquares[j + 1].Size.y), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y), false, false, true, normalSquares[j + 1].IsRoomEast, normalSquares[j + 1].IsRoomWest));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x - random, normalSquares[j + 1].Size.y), new Vector2(normalSquares[j + 1].Position.x + random, normalSquares[j + 1].Position.y), true, true, false, normalSquares[j + 1].IsRoomEast, normalSquares[j + 1].IsRoomWest));
                    }
                    rooms.Add(normalSquares[j]);
                }
                /*else if (normalSquares[j].Position.x < normalSquares[j].Position.y)
                {
                    int random = Random.Range(6, (int)normalSquares[j + 1].Size.y);
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y)));
                    rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, normalSquares[j + 1].Size.y - random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y + random)));
                    rooms.Add(normalSquares[j]);
                }*/
                else
                {
                    int random = Random.Range(6, (int)normalSquares[j + 1].Size.y - 6);
                    if (random - 6 < ((int)normalSquares[j + 1].Size.y - 6) - 6)
                    {
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y), true, normalSquares[j + 1].IsRoomNorth, normalSquares[j + 1].IsRoomSouth, true, false));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, normalSquares[j + 1].Size.y - random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y + random), false, normalSquares[j + 1].IsRoomNorth, normalSquares[j + 1].IsRoomSouth, false, true));
                    }
                    else
                    {
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y), false, normalSquares[j + 1].IsRoomNorth, normalSquares[j + 1].IsRoomSouth, true, false));
                        rooms.Add(new SquareRoom(new Vector2(normalSquares[j + 1].Size.x, normalSquares[j + 1].Size.y - random), new Vector2(normalSquares[j + 1].Position.x, normalSquares[j + 1].Position.y + random), true, normalSquares[j + 1].IsRoomNorth, normalSquares[j + 1].IsRoomSouth, false, true));
                    }
                    rooms.Add(normalSquares[j]);
                }
            }

            j += 2;

        }

    }
    /* OBJECTS PLACEMENT IN ROOM */

    //STEP 1: Create an array of Cells int[36,36]
    Cell[,] cell = new Cell[36, 36];

    //STEP 2: Place every walls in every Cells
    void TransfromRoomsInCells()
    {
        foreach (SquareRoom element in rooms)
        {
            for (int i = 0; i < element.Size.x; i++)
            {
                for (int j = 0; j < element.Size.y; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.CORNER_LEFT_UP);
                    }
                    else if (i == (int)element.Size.x - 1 && j == 0)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.CORNER_RIGHT_UP);
                    }
                    else if (i == 0 && j == (int)element.Size.y - 1)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.CORNER_LEFT_DOWN);

                    }
                    else if (i == (int)element.Size.x - 1 && j == (int)element.Size.y - 1)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.CORNER_RIGHT_DOWN);
                    }
                    else if (i == 0)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.WALL_UP);
                    }
                    else if (j == 0)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.WALL_LEFT);
                    }
                    else if (i == (int)element.Size.x - 1)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.WALL_DOWN);
                    }
                    else if (j == (int)element.Size.y - 1)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.WALL_RIGHT);
                    }
                    else if (element.IsTheSmallest)
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.NORMAL_SMALLEST);
                    }
                    else
                    {
                        cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.NORMAL);
                    }
                    element.Cells.Add(cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j]);
                }
            }
        }

    }

    //STEP 3: Make the difference between rooms and corridors
    void TransformCorridorsInCells()
    {
        foreach (Corridor element in corridors)
        {
            for (int i = 0; i < element.Size.x; i++)
            {
                for (int j = 0; j < element.Size.y; j++)
                {
                    cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + j), CellType.CORRIDOR);
                }
            }
        }
    }

        //STEP 4: Spawn two doors in the smallest room that are linked to the other rooms
    void SpawnDoorsInSmallRoom()
    {
        foreach (SquareRoom element in rooms)
        {
            if (element.IsTheSmallest)
            {
                if (element.IsRoomWest)
                {
                    int random = Random.Range(1, (int)element.Size.x - 1);
                    cell[(int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection] = new Cell(new Vector2((int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection), CellType.DOOR_UP);
                    cell[(int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection - 1] = new Cell(new Vector2((int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection - 1), CellType.DOOR_DOWN);
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection + random + wayPointDoorCenterCorrection, (int)element.Position.y + positionCorrection + wayPointDoorCenterCorrection + wayPointDoorRoomCorrection)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection + random + wayPointDoorCenterCorrection, (int)element.Position.y + positionCorrection - 1 + wayPointDoorCenterCorrection - wayPointDoorRoomCorrection)));


                }
                if (element.IsRoomEast)
                {
                    int random = Random.Range(1, (int)element.Size.x - 1);
                    cell[(int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1] = new Cell(new Vector2((int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1), CellType.DOOR_DOWN);
                    cell[(int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection + (int)element.Size.y] = new Cell(new Vector2((int)element.Position.x + positionCorrection + random, (int)element.Position.y + positionCorrection + (int)element.Size.y), CellType.DOOR_UP);
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection + random + wayPointDoorCenterCorrection, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1 + wayPointDoorCenterCorrection - wayPointDoorRoomCorrection)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection + random + wayPointDoorCenterCorrection, (int)element.Position.y + positionCorrection + (int)element.Size.y + wayPointDoorCenterCorrection + wayPointDoorRoomCorrection)));

                }
                if (element.IsRoomNorth)
                {
                    int random = Random.Range(1, (int)element.Size.y - 1);
                    cell[(int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + random] = new Cell(new Vector2((int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + random), CellType.DOOR_LEFT);
                    cell[(int)element.Position.x + positionCorrection - 1, (int)element.Position.y + positionCorrection + random] = new Cell(new Vector2((int)element.Position.x + positionCorrection - 1, (int)element.Position.y + positionCorrection + random), CellType.DOOR_RIGHT);
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection + wayPointDoorCenterCorrection + wayPointDoorRoomCorrection, (int)element.Position.y + positionCorrection + random + wayPointDoorCenterCorrection)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection - 1 + wayPointDoorCenterCorrection - wayPointDoorRoomCorrection, (int)element.Position.y + positionCorrection + random + wayPointDoorCenterCorrection)));


                }
                if (element.IsRoomSouth)
                {
                    int random = Random.Range(1, (int)element.Size.y - 1);
                    cell[(int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + random] = new Cell(new Vector2((int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + random), CellType.DOOR_RIGHT);
                    cell[(int)element.Position.x + positionCorrection + (int)element.Size.x, (int)element.Position.y + positionCorrection + random] = new Cell(new Vector2((int)element.Position.x + positionCorrection + (int)element.Size.x, (int)element.Position.y + positionCorrection + random), CellType.DOOR_LEFT);
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection + (int)element.Size.x - 1 + wayPointDoorCenterCorrection - wayPointDoorRoomCorrection, (int)element.Position.y + positionCorrection + random + wayPointDoorCenterCorrection)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)element.Position.x + positionCorrection + (int)element.Size.x + wayPointDoorCenterCorrection + wayPointDoorRoomCorrection, (int)element.Position.y + positionCorrection + random + wayPointDoorCenterCorrection)));

                }
            }
        }
    }
        //STEP 5: Spawn two doors in each other rooms linked to the corridors

    void SpawnDoorsLinkedToCorridor()
    {
       for (int i = 0; i < rooms.Count; i++)
        {
            if (!rooms[i].IsTheSmallest && !rooms[i].IsUnCutRoom)
            {
                GameObject corridorWaypoint;
                GameObject roomWaypoint;
                RoomPosition random = RandomiseDoorPosition(rooms[i].IsRoomNorth, rooms[i].IsRoomSouth, rooms[i].IsRoomEast, rooms[i].IsRoomWest);
                if (random == RoomPosition.NORTH)
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.y - 1);
                    cell[(int)rooms[i].Position.x + positionCorrection, (int)rooms[i].Position.y + positionCorrection + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection, (int)rooms[i].Position.y + positionCorrection + newRandom), CellType.DOOR_LEFT);

                    corridorsWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection + wayPointDoorCenterCorrection - wayPointDoorCorridorCorrection, (int)rooms[i].Position.y + newRandom + positionCorrection + wayPointDoorCenterCorrection)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection + wayPointDoorCenterCorrection + wayPointDoorRoomCorrection, (int)rooms[i].Position.y + newRandom + positionCorrection + wayPointDoorCenterCorrection)));

                    //cell[(int)rooms[i].Position.x + positionCorrection - 1, (int)rooms[i].Position.y + positionCorrection + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection - 1, (int)rooms[i].Position.y + positionCorrection + newRandom), CellType.DOOR_RIGHT);
                }
                else if(random == RoomPosition.SOUTH)
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.y - 1);
                    cell[(int)rooms[i].Position.x + positionCorrection + (int)rooms[i].Size.x - 1, (int)rooms[i].Position.y + positionCorrection + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + (int)rooms[i].Size.x - 1, (int)rooms[i].Position.y + positionCorrection + newRandom), CellType.DOOR_RIGHT);

                    corridorsWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection - wayPointDoorCenterCorrection  + wayPointDoorCorridorCorrection + (int)rooms[i].Size.x, (int)rooms[i].Position.y + positionCorrection + wayPointDoorCenterCorrection + newRandom)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection - wayPointDoorCenterCorrection - wayPointDoorRoomCorrection + (int)rooms[i].Size.x, (int)rooms[i].Position.y + positionCorrection + wayPointDoorCenterCorrection + newRandom)));

                    //cell[(int)rooms[i].Position.x + positionCorrection + (int)rooms[i].Size.x, (int)rooms[i].Position.y + positionCorrection + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + (int)rooms[i].Size.x, (int)rooms[i].Position.y + positionCorrection + newRandom), CellType.DOOR_LEFT);
                }
                else if(random == RoomPosition.EAST)
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.x - 1);
                    cell[(int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + (int)rooms[i].Size.y - 1] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + (int)rooms[i].Size.y - 1), CellType.DOOR_DOWN);

                    corridorsWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection + wayPointDoorCenterCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + wayPointDoorCenterCorrection + wayPointDoorCorridorCorrection + (int)rooms[i].Size.y - 1)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection + wayPointDoorCenterCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + wayPointDoorCenterCorrection - wayPointDoorRoomCorrection + (int)rooms[i].Size.y - 1)));

                    //cell[(int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + (int)rooms[i].Size.y] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + (int)rooms[i].Size.y), CellType.DOOR_UP);

                }
                else
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.x - 1);
                    cell[(int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection), CellType.DOOR_UP);

                    corridorsWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection + wayPointDoorCenterCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + wayPointDoorCenterCorrection - wayPointDoorCorridorCorrection)));
                    roomWaypoints.Add(new WayPoint(new Vector2((int)rooms[i].Position.x + positionCorrection + wayPointDoorCenterCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + wayPointDoorCenterCorrection + wayPointDoorRoomCorrection)));

                    //cell[(int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection - 1] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection - 1), CellType.DOOR_DOWN);
                }

            }
            else if (!rooms[i].IsTheSmallest)
            {
                if (!rooms[i - 1].IsTheSmallest)
                {
                    if (rooms[i].IsRoomNorth)
                    {
                        if (rooms[i - 1].Position.x > rooms[i].Position.x)
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.x - 1);
                        }
                        else
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.x - 1);
                        }
                    }
                    else if (rooms[i].IsRoomSouth)
                    {
                        if (rooms[i - 1].Position.x > rooms[i].Position.x)
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.x - 1);
                        }
                        else
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.x - 1);
                        }
                    }
                    else if (rooms[i].IsRoomEast)
                    {
                        if (rooms[i - 1].Position.y > rooms[i].Position.y)
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.y - 1);
                        }
                        else
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.y - 1);
                        }
                    }
                    else if (rooms[i].IsRoomWest)
                    {
                        if (rooms[i - 1].Position.y > rooms[i].Position.y)
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.y - 1);
                        }
                        else
                        {
                            int random = Random.Range(1, (int)rooms[i - 2].Position.y - 1);
                        }
                    }
                    //Step 1 Find where the small room is compared to the big room
                    //Step 2 The room must have a higher or lower
                }
                else if (!rooms[i - 2].IsTheSmallest)
                {

                }
            }

        }
    }

    RoomPosition RandomiseDoorPosition(bool positionNorth, bool positionSouth, bool positionEast, bool positionWest)
    {
        RoomPosition doorPosition;
        List<RoomPosition> doors = new List<RoomPosition>();
        if (!positionNorth)
        {
            doors.Add(RoomPosition.NORTH);
        }
        if (!positionSouth)
        {
            doors.Add(RoomPosition.SOUTH);
        }
        if (!positionEast)
        {
            doors.Add(RoomPosition.EAST);
        }
        if (!positionWest)
        {
            doors.Add(RoomPosition.WEST);
        }
        int random = Random.Range(0, doors.Count);
        doorPosition = doors[random];
        return doorPosition;
    }

    //STEP 6: Spawn items in every rooms

    //STEP 7: Spawn windows in small room
    void SpawnWindows()
    {
        foreach (SquareRoom element in rooms)
        {
            if (element.IsTheSmallest)
            {
                if(!element.IsRoomWest)
                {
                    for (int i = 1; i < (int)element.Size.x - 1; i++)
                    {
                        if (i == 1)
                        {
                            cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection), CellType.WINDOW_END_NORTH_LEFT);
                        }
                        else if (i == (int)element.Size.x -2)
                        {
                            cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection), CellType.WINDOW_END_NORTH_RIGHT);
                        }
                        else
                        {
                            cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection), CellType.WINDOW_UP);
                        }
                    }

                }
                if (!element.IsRoomEast)
                {
                    for (int i = 1; i < (int)element.Size.x - 1; i++)
                    {
                        if (i == 1)
                        {
                            cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1), CellType.WINDOW_END_NORTH_LEFT);
                        }
                        else if (i == (int)element.Size.x - 2)
                        {
                            cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1), CellType.WINDOW_END_NORTH_RIGHT);
                        }
                        else
                        {
                            cell[(int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1] = new Cell(new Vector2((int)element.Position.x + positionCorrection + i, (int)element.Position.y + positionCorrection + (int)element.Size.y - 1), CellType.WINDOW_UP);
                        }
                    }
                }
                if(!element.IsRoomSouth)
                {
                    for (int i = 1; i < (int)element.Size.y - 1; i++)
                    {
                        if (i == 1)
                        {
                            cell[(int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + i] = new Cell(new Vector2((int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + i), CellType.WINDOW_END_NORTH_LEFT);
                        }
                        else if (i == (int)element.Size.y - 2)
                        {
                            cell[(int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + i] = new Cell(new Vector2((int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + i), CellType.WINDOW_END_NORTH_RIGHT);
                        }
                        else
                        {
                            cell[(int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + i] = new Cell(new Vector2((int)element.Position.x + positionCorrection + (int)element.Size.x - 1, (int)element.Position.y + positionCorrection + i), CellType.WINDOW_UP);
                        }
                    }
                }
                if(!element.IsRoomNorth)
                {
                    for (int i = 1; i < (int)element.Size.y - 1; i++)
                    {
                        if (i == 1)
                        {
                            cell[(int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + i] = new Cell(new Vector2((int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + i), CellType.WINDOW_END_NORTH_LEFT);
                        }
                        else if (i == (int)element.Size.y - 2)
                        {
                            cell[(int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + i] = new Cell(new Vector2((int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + i), CellType.WINDOW_END_NORTH_RIGHT);
                        }
                        else
                        {
                            cell[(int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + i] = new Cell(new Vector2((int)element.Position.x + positionCorrection, (int)element.Position.y + positionCorrection + i), CellType.WINDOW_UP);
                        }
                    }
                }
            }
        }
    }
                                                                //Way Points
    //STEP 1: Detect the nearest waypoint for each corridor waypoints

    void CorridorNearWayPointDetection()
    {
        foreach(GameObject element in corridorWaypointsGameobjects)
        {
            //Il y a une cinquantaine d'éléments dans corridorWaypoints
            Ray leftRay = new Ray(element.transform.position, Vector3.left);
            Ray rightRay = new Ray(element.transform.position, Vector3.right);
            Ray backRay = new Ray(element.transform.position, Vector3.back);
            Ray forwardRay = new Ray(element.transform.position, Vector3.forward);
            if (GetNearWayPoint(leftRay) != null)
            {
                Debug.Log("test");
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(leftRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(leftRay).transform.position);
            }
            if (GetNearWayPoint(rightRay) != null)
            {
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(rightRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(rightRay).transform.position);
            }
            if (GetNearWayPoint(backRay) != null)
            {
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(backRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(backRay).transform.position);
            }
            if (GetNearWayPoint(forwardRay) != null)
            {
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(forwardRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(forwardRay).transform.position);
            }
        }
    }

    GameObject GetNearWayPoint(Ray ray)
    {
        RaycastHit[] hits;
        GameObject nearWayPoint = null;
        //Chaque Ray s'instensie où il faut, j'ai testé avec un Debug.DrawRay
        hits = Physics.RaycastAll(ray, Mathf.Infinity);

        float hitDistance = 32f;

        //Chacun de ces object a un sphere collider trigger
        foreach (RaycastHit hit in hits)
            {
            if (hit.collider.tag == "Wall")
            {
                break;
            }
            if (hit.collider.tag == "WayPointCorridor")
                {
                if (hit.distance < hitDistance)
                {

                    //Debug.Log(hit.transform.name);
                    nearWayPoint = hit.collider.gameObject;
                    hitDistance = hit.distance;
                 }
                }
            }
        
        return nearWayPoint;
        
    }
    //STEP 2: Detect the nearest Waypoints in each room

    void RoomNearWayPointDetection()
    {
        foreach (GameObject element in roomWaypointsGameobjects)
        {
            //Il y a une cinquantaine d'éléments dans corridorWaypoints
            Ray leftRay = new Ray(element.transform.position, Vector3.left);
            Ray rightRay = new Ray(element.transform.position, Vector3.right);
            Ray backRay = new Ray(element.transform.position, Vector3.back);
            Ray forwardRay = new Ray(element.transform.position, Vector3.forward);
            if (GetNearWayPoint(leftRay) != null)
            {
                Debug.Log("test");
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(leftRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(leftRay).transform.position);
            }
            if (GetNearWayPoint(rightRay) != null)
            {
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(rightRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(rightRay).transform.position);
            }
            if (GetNearWayPoint(backRay) != null)
            {
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(backRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(backRay).transform.position);
            }
            if (GetNearWayPoint(forwardRay) != null)
            {
                element.GetComponent<WayPointComponents>().NearWayPoints.Add(GetNearWayPoint(forwardRay));
                element.GetComponent<WayPointComponents>().NearWayPointsInspector.Add(GetNearWayPoint(forwardRay).transform.position);
            }
        }
    }


}


