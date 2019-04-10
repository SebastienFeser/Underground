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

    #region LISTS SQUAREROOMS, CORRIDORS, CELLS
    private SquareRoom mapSquare = new SquareRoom(new Vector2(30, 30), new Vector2(0, 0));
    private List<SquareRoom> mapSquareCut = new List<SquareRoom>();
    private List<SquareRoom> quadriSquares = new List<SquareRoom>();
    private List<SquareRoom> quadriSquaresCut = new List<SquareRoom>();
    private List<SquareRoom> rooms = new List<SquareRoom>();
    private List<Corridor> corridors;
    private Cell[,] cellMap = new Cell[36, 36];
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
    #endregion

    #region BSP
    List<SquareRoom> CutSquareRoom(int randomMin, int randomMax, SquareRoom squareToCut, bool cutVertical, bool isCorridor)
    {
        List<SquareRoom> cutSquareRoom = new List<SquareRoom>();
        int random = Random.Range(randomMin, randomMax);
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
                corridors.Add(new Corridor(new Vector2(corridorSize, squareToCut.Size.y), new Vector2(squareToCut.Position.x + random, squareToCut.Position.y)));
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
    void CutNormalSquares()
    {
        foreach (SquareRoom element in quadriSquaresCut)
        {
            if (element.IsTheBiggest)
            {
                if (element.WasCutHorizontal)
                {
                    rooms.AddRange(CutSquareRoom(6, (int)element.Size.x - 6, element, true, false));
                }
                else
                {
                    rooms.AddRange(CutSquareRoom(6, (int)element.Size.y - 6, element, false, false));
                }
            }
            else
            {
                rooms.Add(element);
            }
        }
    }

    void AddNormalSquaresToQuadriSquares()
    {
        int i = 0;
        foreach (SquareRoom element in quadriSquaresCut)
        {
            List<SquareRoom> elementList = new List<SquareRoom>();
            elementList.Add(rooms[i]);
            elementList.Add(rooms[i + 1]);
            elementList.Add(rooms[i + 2]);
            element.RoomChildren = elementList;
            i += 3;
        }
    }
    #endregion

    #region OBJECTS PLACEMENT
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


    #endregion
}
