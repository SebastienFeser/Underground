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

        public SquareRoom(Vector2 sizeConstruct, Vector2 positionConstruct)
        {
            size = sizeConstruct;
            position = positionConstruct;
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
    #endregion

    #region LISTS SQUAREROOMS & CORRIDORS
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

    List<SquareRoom> CutSquareRoom(int randomMin, int randomMax, SquareRoom squareToCut, bool cutVertical, bool isCorridor)
    {
        List<SquareRoom> cutSquareRoom = new List<SquareRoom>();
        int random = Random.Range(randomMin, randomMax);
        int corridorBetweenSize = 0;
        if (isCorridor)
        {
            corridorBetweenSize = corridorSize;
        }
        if (cutVertical)
        {
            cutSquareRoom.Add(new SquareRoom(new Vector2(random, squareToCut.Size.y), new Vector2(squareToCut.Position.x, squareToCut.Position.y)));
            cutSquareRoom.Add(new SquareRoom(new Vector2(squareToCut.Size.x - random, squareToCut.Size.y), new Vector2(squareToCut.Position.x + random + corridorBetweenSize, squareToCut.Position.y)));
            if (isCorridor)
            {
                corridors.Add(new Corridor(new Vector2(corridorSize, squareToCut.Size.y), new Vector2(squareToCut.Position.x + random, squareToCut.Position.y)));
            }
        }
        else
        {
            cutSquareRoom.Add(new SquareRoom(new Vector2(squareToCut.Size.x, random), new Vector2(squareToCut.Position.x, squareToCut.Position.y)));
            cutSquareRoom.Add(new SquareRoom(new Vector2(squareToCut.Size.x, squareToCut.Size.y - random), new Vector2(squareToCut.Position.x, squareToCut.Position.y + random + corridorBetweenSize)));
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
}
