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

    public class Cell
    {

    }
    #endregion

    #region LISTS SQUAREROOMS, CORRIDORS, CELLS
    private SquareRoom mapSquare = new SquareRoom(new Vector2(30, 30), new Vector2(0, 0));
    private List<SquareRoom> mapSquareCut = new List<SquareRoom>();
    private List<SquareRoom> quadriSquares = new List<SquareRoom>();
    private List<SquareRoom> quadriSquaresCut = new List<SquareRoom>();
    private List<SquareRoom> rooms = new List<SquareRoom>();
    private List<Corridor> corridors;
    #endregion

    #region SQUAREROOMS & CORRIDORS INFO
    int corridorSize = 2;
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




    #endregion
}
