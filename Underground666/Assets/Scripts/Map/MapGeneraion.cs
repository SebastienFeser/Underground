using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneraion : MonoBehaviour
{
    #region CLASSES
    public class SquareRoom
    {
        public enum RoomsPosition
        {
            NORTH,
            SOUTH,
            EAST,
            WEST

        }
        public enum RoomType
        {
            DEFAULT,
            NORMAL,
            OBJECTIVE_ROOM,
            UNCUT,
        }
        private Vector2 position;
        private Vector2 size;
        private List<RoomsPosition> whereAreRooms = new List<RoomsPosition>();
        private RoomType type = RoomType.DEFAULT;
        private List<SquareRoom> roomChildren = new List<SquareRoom>();
        private bool wasCutHorizontal;
        private bool isTheBiggest;
        private Cell[,] roomCells;

        public Vector2 Position
        {
            get { return position; }

        }

        public Vector2 Size
        {
            get { return size; }
        }

        public List<RoomsPosition> WhereAreRooms
        {
            get { return whereAreRooms; }
        }

        public RoomType Type
        {
            get { return type; }
            set { type = value; }
        }

        public List<SquareRoom> RoomChildren
        {
            get { return roomChildren; }
            set { roomChildren = value; }
        }

        public bool WasCutHorizontal
        {
            get { return wasCutHorizontal; }
        }

        public bool IsTheBiggest
        {
            get { return isTheBiggest; }
        }

        public SquareRoom(Vector2 sizeConstruct, Vector2 positionConstruct)
        {
            size = sizeConstruct;
            position = positionConstruct;
        }

        public SquareRoom(Vector2 sizeConstruct, Vector2 positionConstruct, bool wasCutHorizontalConstruct, bool isTheBiggestConstruct)
        {
            size = sizeConstruct;
            position = positionConstruct;
            wasCutHorizontal = wasCutHorizontalConstruct;
            isTheBiggest = isTheBiggestConstruct;
        
        }

        public SquareRoom(Vector2 sizeConstruct, Vector2 positionConstruct, RoomType typeConstruct, List<RoomsPosition> whereAreRoomsConstruct)
        {
            size = sizeConstruct;
            position = positionConstruct;
            type = typeConstruct;
            whereAreRooms = whereAreRoomsConstruct;
        }

    }

    public class Corridor
    {
        private Vector2 position;
        private Vector2 size;

        public Vector2 Position
        {
            get { return position; }
        }

        public Vector2 Size
        {
            get { return size; }
        }

        public Corridor(Vector2 sizeConstruct, Vector2 positionConstruct)
        {
            size = sizeConstruct;
            position = positionConstruct;
        }
    }

    public class Waypoint
    {

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
            get { return type;}
        }

        public Vector2 Position
        {
            get { return position; }
        }

        Vector2 position;
        CellType type;
        
    }
    #endregion

    #region SERIALIZED GAMEOBJECTS
    //Cells Gameobjects that will be instantiated like tiles
    [SerializeField] GameObject normalCell;
    [SerializeField] GameObject cornerCell;
    [SerializeField] GameObject normalSmallestCell;
    [SerializeField] GameObject doorCell;
    [SerializeField] GameObject wallUpCell;
    [SerializeField] GameObject wallDownCell;
    [SerializeField] GameObject wallLeftCell;
    [SerializeField] GameObject wallRightCell;
    [SerializeField] GameObject cornerLeftUpCell;
    [SerializeField] GameObject cornerLeftDownCell;
    [SerializeField] GameObject cornerRightUpCell;
    [SerializeField] GameObject cornerRightDownCell;
    [SerializeField] GameObject doorUpCell;
    [SerializeField] GameObject doorDownCell;
    [SerializeField] GameObject doorLeftCell;
    [SerializeField] GameObject doorRightCell;
    [SerializeField] GameObject corridorCell;


    [SerializeField] GameObject player;
    [SerializeField] GameObject wayPointCorridor;
    [SerializeField] GameObject wayPointRoom;
    #endregion

    #region SQUAREROOMS, CELLS & CORRIDORS INFO
    int corridorSize = 2;
    enum CellType
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
    int cellsPositionCorrection = 2;
    int cellMapSizeXY = 36;
    #endregion

    #region LISTS SQUAREROOMS, CORRIDORS, CELLS
    private SquareRoom mapSquare = new SquareRoom(new Vector2(30, 30), new Vector2(0, 0));
    private List<SquareRoom> mapSquareCut = new List<SquareRoom>();
    private List<SquareRoom> quadriSquares = new List<SquareRoom>();
    private List<SquareRoom> quadriSquaresCut = new List<SquareRoom>();
    private List<SquareRoom> rooms = new List<SquareRoom>();
    private List<Corridor> corridors = new List<Corridor>();
    private Cell[,] cellMap;
    #endregion
    
    #region START, UPDATE, FIXED UPDATE
    private void Start()
    {
        cellMap = new Cell[cellMapSizeXY, cellMapSizeXY];
        StartCoroutine("BuildMap");
    }
    #endregion

    #region COROUTINE BUILDING MAP
    IEnumerator BuildMap()
    {
        GenerateMap();
        Debug.Log("Done");
        yield return new WaitForEndOfFrame();
        ObjectsPlacement();
        Debug.Log("Done");
        yield return new WaitForEndOfFrame();
        CellsSpawn();
        yield return new WaitForEndOfFrame();
        Debug.Log("Done");


    }

    #endregion

    #region BSP

    void GenerateMap()
    {
        //Clearing all lists
        mapSquareCut.Clear();
        quadriSquares.Clear();
        quadriSquaresCut.Clear();
        rooms.Clear();
        corridors.Clear();

        //Call every functions to build the map
        CutMapSquare();
        CutMapSquareCut();
        CutQuadriSquares();
        CutQuadriSquaresCut();
        AddNormalSquaresToQuadriSquares();
        AddCorridorsAround();
        CheckRoomPositions();
    }

    List<SquareRoom> CutSquareRoom(int randomMin, int randomMax, SquareRoom squareToCut, bool cutVertical, bool isCorridor)
    {
        List<SquareRoom> cutSquareRoom = new List<SquareRoom>();
        int random = Random.Range(randomMin, randomMax);
        /*List<SquareRoom.RoomsPosition> whereAreRooms1 = new List<SquareRoom.RoomsPosition>();
        List<SquareRoom.RoomsPosition> whereAreRooms2 = new List<SquareRoom.RoomsPosition>();*/
        //SquareRoom squareRoom1;
        //SquareRoom squareRoom2;
        bool isTheBiggest;

        if (random - randomMin < randomMax - random)
        {
            isTheBiggest = false;
        }
        else
        {
            isTheBiggest = true;
        }
        int corridorBetweenSize = 0;
        if (isCorridor)
        {
            corridorBetweenSize = corridorSize;
        }
        if (cutVertical)
        {
            cutSquareRoom.Add(new SquareRoom(new Vector2(random, squareToCut.Size.y), new Vector2(squareToCut.Position.x, squareToCut.Position.y), false, isTheBiggest));
            cutSquareRoom.Add(new SquareRoom(new Vector2(squareToCut.Size.x - random, squareToCut.Size.y), new Vector2(squareToCut.Position.x + random + corridorBetweenSize, squareToCut.Position.y), false, !isTheBiggest));
            if (isCorridor)
            {
                corridors.Add(new Corridor(new Vector2(corridorSize, squareToCut.Size.y + corridorSize), new Vector2(squareToCut.Position.x + random, squareToCut.Position.y)));
            }
        }
        else
        {
            cutSquareRoom.Add(new SquareRoom(new Vector2(squareToCut.Size.x, random), new Vector2(squareToCut.Position.x, squareToCut.Position.y), true, isTheBiggest));
            cutSquareRoom.Add(new SquareRoom(new Vector2(squareToCut.Size.x, squareToCut.Size.y - random), new Vector2(squareToCut.Position.x, squareToCut.Position.y + random + corridorBetweenSize), true, !isTheBiggest));
            if (isCorridor)
            {
                corridors.Add(new Corridor(new Vector2(squareToCut.Size.x, corridorSize), new Vector2(squareToCut.Position.x, squareToCut.Position.y + random)));
            }
        }

        return cutSquareRoom;
    }
    //STEP 1: Cut map in two
    void CutMapSquare()
    {

        mapSquareCut.AddRange(CutSquareRoom(12, 19, mapSquare, true, true));
    }

    //STEP 2: Cut the MAP SQUARE CUT 

    void CutMapSquareCut()
    {
        foreach (SquareRoom element in mapSquareCut)
        {
            quadriSquares.AddRange(CutSquareRoom(12, 19, element, false, true));
        }
        mapSquare.RoomChildren = quadriSquares;
    }

    //STEP 3: Add CORRIDORS to corners
    void AddCorridorsAround()
    {
        corridors.Add(new Corridor(new Vector2(corridorSize, mapSquare.Size.x + corridorSize * 3), new Vector2(0 - corridorSize, 0 - corridorSize)));
        corridors.Add(new Corridor(new Vector2(corridorSize, mapSquare.Size.x + corridorSize * 3), new Vector2(mapSquare.Size.x + corridorSize, 0 - corridorSize)));
        corridors.Add(new Corridor(new Vector2(mapSquare.Size.x + corridorSize, corridorSize), new Vector2(0, 0 - corridorSize)));
        corridors.Add(new Corridor(new Vector2(mapSquare.Size.x + corridorSize, corridorSize), new Vector2(0, mapSquare.Size.y + corridorSize)));

        /*corridorsWaypoints.Add(new WayPoint(new Vector2(wayPointCorrection, wayPointCorrection)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(corridorDifference * 3 - wayPointCorrection + mapSquare.Size.x, wayPointCorrection)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(wayPointCorrection, corridorDifference * 3 - wayPointCorrection + mapSquare.Size.y)));
        corridorsWaypoints.Add(new WayPoint(new Vector2(corridorDifference * 3 - wayPointCorrection + mapSquare.Size.x, corridorDifference * 3 - wayPointCorrection + mapSquare.Size.y)));*/
    }

    //STEP 4: Cut the QUADRISQUARES horizontally or vertically

    void CutQuadriSquares()
    {
        foreach (SquareRoom element in quadriSquares)
        {
            int cutVertical = Random.Range(0, 2);
            if (cutVertical > 0)
            {
                quadriSquaresCut.AddRange(CutSquareRoom(6, (int)element.Size.x - 6, element, true, false));
            }
            else
            {
                quadriSquaresCut.AddRange(CutSquareRoom(6, (int)element.Size.y - 6, element, false, false));
            }
        }
    }

    // STEP 5: Cut the biggest NORMAL SQUARE in each QUADRISQUARE
    void CutQuadriSquaresCut()
    {
        foreach (SquareRoom element in quadriSquaresCut)
        {
            SquareRoom actualElement;
            List<SquareRoom> elementsToCalculate = new List<SquareRoom>();
            if (element.IsTheBiggest)
            {
                SquareRoom.RoomType type1 = SquareRoom.RoomType.NORMAL;
                SquareRoom.RoomType type2 = SquareRoom.RoomType.NORMAL;
                int random = Random.Range(0, 2);
                if (random > 0)
                {
                    type1 = SquareRoom.RoomType.OBJECTIVE_ROOM;
                }
                else
                {
                    type2 = SquareRoom.RoomType.OBJECTIVE_ROOM;
                }
                if (element.WasCutHorizontal)
                {
                    elementsToCalculate.AddRange(CutSquareRoom(6, (int)element.Size.x - 6, element, true, false));
                    elementsToCalculate[0].Type = type1;
                    elementsToCalculate[1].Type = type2;
                    rooms.AddRange(elementsToCalculate);
                }
                else
                {
                    elementsToCalculate.AddRange(CutSquareRoom(6, (int)element.Size.y - 6, element, false, false));
                    elementsToCalculate[0].Type = type1;
                    elementsToCalculate[1].Type = type2;
                    rooms.AddRange(elementsToCalculate);
                }
            }
            else
            {
                actualElement = element;
                actualElement.Type = SquareRoom.RoomType.UNCUT;
                rooms.Add(actualElement);
            }
        }
    }

    void AddNormalSquaresToQuadriSquares()
    {
        int i = 0;
        foreach (SquareRoom element in quadriSquares)
        {
            List<SquareRoom> elementList = new List<SquareRoom>();
            elementList.Add(rooms[i]);          
            elementList.Add(rooms[i + 1]);
            elementList.Add(rooms[i + 2]);
            element.RoomChildren = elementList;
            i += 3;
        }
    }

    void CheckRoomPositions()
    {
        foreach (SquareRoom element in quadriSquares)
        {
            SquareRoom unCutRoom = null;
            SquareRoom normalRoom = null;
            SquareRoom objectiveRoom = null;
            foreach (SquareRoom element2 in element.RoomChildren)
            {
                //Debug.Log(element2.Type);
                if (element2.Type == SquareRoom.RoomType.UNCUT)
                {
                    unCutRoom = element2;
                }
                else if (element2.Type == SquareRoom.RoomType.OBJECTIVE_ROOM)
                {
                    objectiveRoom = element2;
                }
                else
                {
                    normalRoom = element2;
                }
            }

            SetNearRoomForUnCutRoom(unCutRoom, normalRoom, objectiveRoom);
            SetNearRoomForNonUnCutRoom(unCutRoom, normalRoom, objectiveRoom);
            foreach (SquareRoom element2 in element.RoomChildren)
            {
                if (element2.Type == SquareRoom.RoomType.OBJECTIVE_ROOM)
                {
                    Debug.Log(element2.Position);
                    foreach(SquareRoom.RoomsPosition element3 in element2.WhereAreRooms)
                    {
                        Debug.Log(element3);
                    }
                }
            }
        }

        void SetNearRoomForUnCutRoom (SquareRoom unCutRoom, SquareRoom normalRoom, SquareRoom objectiveRoom)
        {
            if ((int)unCutRoom.Position.x == (int)normalRoom.Position.x || (int)unCutRoom.Position.x == (int)objectiveRoom.Position.x)
            {
                if (unCutRoom.Position.y > normalRoom.Position.y)
                {
                    unCutRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.WEST); 
                }
                else
                {
                    unCutRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.EAST); 
                }
            }
            else
            {
                if (unCutRoom.Position.x > normalRoom.Position.x)
                {
                    unCutRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.NORTH); 
                }
                else
                {
                    unCutRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.SOUTH); 
                }
            }
        }
        void SetNearRoomForNonUnCutRoom (SquareRoom unCutRoom, SquareRoom normalRoom, SquareRoom objectiveRoom)
        {
            switch (unCutRoom.WhereAreRooms[0])
            {
                case SquareRoom.RoomsPosition.NORTH:
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.SOUTH);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.SOUTH);
                    break;
                case SquareRoom.RoomsPosition.SOUTH:
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.NORTH);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.NORTH);
                    break;
                case SquareRoom.RoomsPosition.EAST:
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.WEST);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.WEST);
                    break;
                case SquareRoom.RoomsPosition.WEST:
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.EAST);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.EAST);
                    break;
            }

            if (unCutRoom.WhereAreRooms[0] == SquareRoom.RoomsPosition.NORTH || unCutRoom.WhereAreRooms[0] == SquareRoom.RoomsPosition.SOUTH)
            {
                if (normalRoom.Position.y > objectiveRoom.Position.y)
                {
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.WEST);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.EAST);
                }
                else
                {
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.EAST);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.WEST);
                }
            }
            else
            {
                if (normalRoom.Position.x > objectiveRoom.Position.x)
                {
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.NORTH);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.SOUTH);
                }
                else
                {
                    normalRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.SOUTH);
                    objectiveRoom.WhereAreRooms.Add(SquareRoom.RoomsPosition.NORTH);
                }
            }
        }
    }
    #endregion

    #region OBJECTS PLACEMENT
    void ObjectsPlacement()
    {
        TransfromRoomsInCells();
        TransformCorridorsInCells();
        SpawnDoorsInSmallRoom();
        SpawnDoorsLinkedToCorridor();
    }
    //STEP 1: Transform the SquareRooms in Cells
    void TransfromRoomsInCells()
    {
        foreach (SquareRoom element in rooms)
        {
            Cell[,] actualRoomList = new Cell[(int)element.Size.x, (int)element.Size.y]; // This is the list that will be stored in each SquareRoom
            for (int i = 0; i < element.Size.x; i++)
            {
                for (int j = 0; j < element.Size.y; j++)
                {
                    Cell actualCell;
                    if (i == 0 && j == 0)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.CORNER_LEFT_UP);
                    }
                    else if (i == (int)element.Size.x - 1 && j == 0)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.CORNER_RIGHT_UP);
                    }
                    else if (i == 0 && j == (int)element.Size.y - 1)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.CORNER_LEFT_DOWN);

                    }
                    else if (i == (int)element.Size.x - 1 && j == (int)element.Size.y - 1)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.CORNER_RIGHT_DOWN);
                    }
                    else if (i == 0)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.WALL_UP);
                    }
                    else if (j == 0)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.WALL_LEFT);
                    }
                    else if (i == (int)element.Size.x - 1)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.WALL_DOWN);
                    }
                    else if (j == (int)element.Size.y - 1)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.WALL_RIGHT);
                    }
                    else if (element.Type == SquareRoom.RoomType.OBJECTIVE_ROOM)
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.NORMAL_SMALLEST);
                    }
                    else
                    {
                        actualCell = cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.NORMAL);
                    }
                    actualRoomList[i, j] = actualCell;
                    Debug.Log(actualCell.Type);
                }
            }
            
        }

    }

    //STEP 2: Transform the Corridors in Cells
    void TransformCorridorsInCells()
    {
        foreach (Corridor element in corridors)
        {
            for (int i = 0; i < element.Size.x; i++)
            {
                for (int j = 0; j < element.Size.y; j++)
                {
                    cellMap[(int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + i, (int)element.Position.y + cellsPositionCorrection + j), CellType.CORRIDOR);
                }
            }
        }
    }

    //STEP 3: Spawn two doors in the smallest room that are linked to other rooms
    void SpawnDoorsInSmallRoom()
    {
        foreach (SquareRoom element in rooms)
        {
            if (element.Type == SquareRoom.RoomType.OBJECTIVE_ROOM)
            {
                foreach(SquareRoom.RoomsPosition element2 in element.WhereAreRooms)
                {
                    int random;
                    switch (element2)
                        {
                        case SquareRoom.RoomsPosition.NORTH:
                            random = Random.Range(1, (int)element.Size.y - 1);
                            cellMap[(int)element.Position.x + cellsPositionCorrection, (int)element.Position.y + cellsPositionCorrection + random] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection, (int)element.Position.y + cellsPositionCorrection + random), CellType.DOOR_LEFT);
                            cellMap[(int)element.Position.x + cellsPositionCorrection - 1, (int)element.Position.y + cellsPositionCorrection + random] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection - 1, (int)element.Position.y + cellsPositionCorrection + random), CellType.DOOR_RIGHT);
                            break;
                        case SquareRoom.RoomsPosition.SOUTH:
                            random = Random.Range(1, (int)element.Size.y - 1);
                            cellMap[(int)element.Position.x + cellsPositionCorrection + (int)element.Size.x - 1, (int)element.Position.y + cellsPositionCorrection + random] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + (int)element.Size.x - 1, (int)element.Position.y + cellsPositionCorrection + random), CellType.DOOR_RIGHT);
                            cellMap[(int)element.Position.x + cellsPositionCorrection + (int)element.Size.x, (int)element.Position.y + cellsPositionCorrection + random] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + (int)element.Size.x, (int)element.Position.y + cellsPositionCorrection + random), CellType.DOOR_LEFT);
                            break;
                        case SquareRoom.RoomsPosition.EAST:
                            random = Random.Range(1, (int)element.Size.x - 1);
                            cellMap[(int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection + (int)element.Size.y - 1] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection + (int)element.Size.y - 1), CellType.DOOR_DOWN);
                            cellMap[(int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection + (int)element.Size.y] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection + (int)element.Size.y), CellType.DOOR_UP);
                            break;
                        case SquareRoom.RoomsPosition.WEST:
                            random = Random.Range(1, (int)element.Size.x - 1);
                            cellMap[(int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection), CellType.DOOR_UP);
                            cellMap[(int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection - 1] = new Cell(new Vector2((int)element.Position.x + cellsPositionCorrection + random, (int)element.Position.y + cellsPositionCorrection - 1), CellType.DOOR_DOWN);
                            break;
                    }
                }
            }
        }
    }

    //STEP 4: Spawn 1 door in each other room linked to the corridor
    void SpawnDoorsLinkedToCorridor()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].Type != SquareRoom.RoomType.OBJECTIVE_ROOM)
            {
                GameObject corridorWaypoint;
                GameObject roomWaypoint;
                SquareRoom.RoomsPosition random = RandomiseDoorPosition(rooms[i].WhereAreRooms);
                if (random == SquareRoom.RoomsPosition.NORTH)
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.y - 1);
                    cellMap[(int)rooms[i].Position.x + corridorSize, (int)rooms[i].Position.y + corridorSize + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + corridorSize, (int)rooms[i].Position.y + corridorSize + newRandom), CellType.DOOR_LEFT);

                    //cell[(int)rooms[i].Position.x + positionCorrection - 1, (int)rooms[i].Position.y + positionCorrection + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection - 1, (int)rooms[i].Position.y + positionCorrection + newRandom), CellType.DOOR_RIGHT);
                }
                else if (random == SquareRoom.RoomsPosition.SOUTH)
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.y - 1);
                    cellMap[(int)rooms[i].Position.x + corridorSize + (int)rooms[i].Size.x - 1, (int)rooms[i].Position.y + corridorSize + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + corridorSize + (int)rooms[i].Size.x - 1, (int)rooms[i].Position.y + corridorSize + newRandom), CellType.DOOR_RIGHT);

                    //cell[(int)rooms[i].Position.x + positionCorrection + (int)rooms[i].Size.x, (int)rooms[i].Position.y + positionCorrection + newRandom] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + (int)rooms[i].Size.x, (int)rooms[i].Position.y + positionCorrection + newRandom), CellType.DOOR_LEFT);
                }
                else if (random == SquareRoom.RoomsPosition.EAST)
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.x - 1);
                    cellMap[(int)rooms[i].Position.x + corridorSize + newRandom, (int)rooms[i].Position.y + corridorSize + (int)rooms[i].Size.y - 1] = new Cell(new Vector2((int)rooms[i].Position.x + corridorSize + newRandom, (int)rooms[i].Position.y + corridorSize + (int)rooms[i].Size.y - 1), CellType.DOOR_DOWN);
                    //cell[(int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + (int)rooms[i].Size.y] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection + (int)rooms[i].Size.y), CellType.DOOR_UP);

                }
                else
                {
                    int newRandom = Random.Range(1, (int)rooms[i].Size.x - 1);
                    cellMap[(int)rooms[i].Position.x + corridorSize + newRandom, (int)rooms[i].Position.y + corridorSize] = new Cell(new Vector2((int)rooms[i].Position.x + corridorSize + newRandom, (int)rooms[i].Position.y + corridorSize), CellType.DOOR_UP);
                    //cell[(int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection - 1] = new Cell(new Vector2((int)rooms[i].Position.x + positionCorrection + newRandom, (int)rooms[i].Position.y + positionCorrection - 1), CellType.DOOR_DOWN);
                }

            }
            

        }
    }

    SquareRoom.RoomsPosition RandomiseDoorPosition(List<SquareRoom.RoomsPosition> whereAreRooms)
    {
        SquareRoom.RoomsPosition doorPosition;
        List<SquareRoom.RoomsPosition> doors = new List<SquareRoom.RoomsPosition>();
        doors.Add(SquareRoom.RoomsPosition.NORTH);
        doors.Add(SquareRoom.RoomsPosition.SOUTH);
        doors.Add(SquareRoom.RoomsPosition.EAST);
        doors.Add(SquareRoom.RoomsPosition.WEST);
        foreach (SquareRoom.RoomsPosition element in whereAreRooms)
        {
            if (element == SquareRoom.RoomsPosition.NORTH)
            {
                doors.Remove(SquareRoom.RoomsPosition.NORTH);
            }
            else if (element == SquareRoom.RoomsPosition.SOUTH)
            {
                doors.Remove(SquareRoom.RoomsPosition.SOUTH);
            }
            else if (element == SquareRoom.RoomsPosition.EAST)
            {
                doors.Remove(SquareRoom.RoomsPosition.EAST);
            }
            else if (element == SquareRoom.RoomsPosition.WEST)
            {
                doors.Remove(SquareRoom.RoomsPosition.WEST);
            }
        }
        int random = Random.Range(0, doors.Count);
        doorPosition = doors[random];
        return doorPosition;
    }


    #endregion

    #region SPAWN FUNCTIONS

    void CellsSpawn()
    {
        for (int i = 0; i < cellMapSizeXY; i++)
        {
            for (int j = 0; j < cellMapSizeXY; j++)
            {
                GameObject actualGameObject;
                //Debug.Log(cellMap[i,j]);
                
                switch (cellMap[i, j].Type)
            {
                case CellType.CORNER_RIGHT_UP:
                    actualGameObject = Instantiate(cornerRightUpCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.CORNER_RIGHT_DOWN:
                    actualGameObject = Instantiate(cornerRightDownCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.CORNER_LEFT_UP:
                    actualGameObject = Instantiate(cornerLeftUpCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);

                    break;
                case CellType.CORNER_LEFT_DOWN:
                    actualGameObject = Instantiate(cornerLeftDownCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.WALL_UP:
                    actualGameObject = Instantiate(wallUpCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.WALL_DOWN:
                    actualGameObject = Instantiate(wallDownCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.WALL_LEFT:
                    actualGameObject = Instantiate(wallLeftCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.WALL_RIGHT:
                    actualGameObject = Instantiate(wallRightCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.NORMAL:
                    actualGameObject = Instantiate(normalCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.CORRIDOR:
                    actualGameObject = Instantiate(corridorCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.NORMAL_SMALLEST:
                    actualGameObject = Instantiate(normalSmallestCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.DOOR_UP:
                    actualGameObject = Instantiate(doorUpCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.DOOR_DOWN:
                    actualGameObject = Instantiate(doorDownCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.DOOR_LEFT:
                    actualGameObject = Instantiate(doorLeftCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
                case CellType.DOOR_RIGHT:
                    actualGameObject = Instantiate(doorRightCell);
                    actualGameObject.transform.parent = gameObject.transform;
                    actualGameObject.transform.position = new Vector3(cellMap[i, j].Position.x, 0, cellMap[i, j].Position.y);
                    break;
            }
            }
        }
    }
    #endregion
}
