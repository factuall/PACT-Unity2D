using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }

    public GameRoom currentRoom;
    public GameObject roomPrefab;
    public PlayerController pl;
    Camera mainCamera;
    bool gameplayReady = false;

    /*******FLOOR GENERATION VARIABLES*******/
    public static float ROOM_WIDTH = 16, ROOM_HEIGHT = 9;
    public static int floorSize = 20;
    public int connectiorRoomCap = 3;
    public int roomCap = 20;
    public int roomProb = 5;
    public GameRoom[,] floorRooms = new GameRoom[floorSize, floorSize];
    /*--------------------------------------*/

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        //GenerateFloor();
        //currentRoom = floorRooms[10, 10];
        //gameplayReady = true;
        //GetComponentInChildren<PlayerController>().transform.position = new Vector3(
        //    currentRoom.roomInstance.transform.position.x,
        //    currentRoom.roomInstance.transform.position.y,
        //    0);

    }

    // Update is called once per frame
    void Update()
    {
        if (gameplayReady)
        {
            mainCamera.transform.position = new Vector3(
                currentRoom.roomInstance.transform.position.x,
                currentRoom.roomInstance.transform.position.y,
                -10);

            if (Input.GetKeyDown(KeyCode.RightArrow)) currentRoom = floorRooms[currentRoom.X + 1, currentRoom.Y];
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) currentRoom = floorRooms[currentRoom.X - 1, currentRoom.Y];
            if (Input.GetKeyDown(KeyCode.UpArrow)) currentRoom = floorRooms[currentRoom.X, currentRoom.Y + 1];
            else if (Input.GetKeyDown(KeyCode.DownArrow)) currentRoom = floorRooms[currentRoom.X, currentRoom.Y - 1];


        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            for (int lY = 0; lY < floorRooms.GetLength(1); lY++)
            {
                for (int lX = 0; lX < floorRooms.GetLength(0); lX++)
                {
                    if (floorRooms[lX, lY] != null) 
                    if (floorRooms[lX, lY].isOccupied()) Destroy(floorRooms[lX, lY].roomInstance);
                    floorRooms[lX, lY] = new GameRoom(lX, lY);
                }
            }
            //GenerateFloor();
            GenerateFloor();
            currentRoom = floorRooms[10, 10];
            gameplayReady = true;
            GetComponentInChildren<PlayerController>().transform.position = new Vector3(
                currentRoom.roomInstance.transform.position.x,
                currentRoom.roomInstance.transform.position.y,
                0);
        }
    }

    // Get all existing neighbors of a room
    public GameRoom[] GetNeighbors(GameRoom origin)
    {
        var neighbors = new List<GameRoom>();
        if (origin.X > 0) neighbors.Add(floorRooms[origin.X - 1, origin.Y]);
        if (origin.X < floorSize - 1) neighbors.Add(floorRooms[origin.X + 1, origin.Y]);
        if (origin.Y > 0) neighbors.Add(floorRooms[origin.X, origin.Y - 1]);
        if (origin.Y < floorSize - 1) neighbors.Add(floorRooms[origin.X, origin.Y + 1]);
        return neighbors.ToArray();
    }

    // Generate all rooms on the floor
    public void GenerateFloor()
    {
        // Init all of the cells
        for (int lY = 0; lY < floorRooms.GetLength(1); lY++)
        {
            for (int lX = 0; lX < floorRooms.GetLength(0); lX++)
            {
                floorRooms[lX, lY] = new GameRoom(lX, lY);
            }
        }


        int roomCount = 0;
        int connectorRoomCount = 0;
        // Create central room
        floorRooms[10, 10].roomInstance = Instantiate(roomPrefab, new Vector3(10 * ROOM_WIDTH, 10 * ROOM_HEIGHT, 0), transform.rotation, transform);
        roomCount++;

        while (roomCount < roomCap)
        {
            bool generatedAnything = false;
            for (int lY = 0; lY < floorRooms.GetLength(1); lY++)
            {
                for (int lX = 0; lX < floorRooms.GetLength(0); lX++)
                {
                    // skip if the room is occupied 
                    
                    if (!floorRooms[lX, lY].isOccupied()) continue;
                    if (floorRooms[lX, lY].generatedNeighbors) continue;
                    // for all neighbors of the room
                    foreach (GameRoom neighbor in GetNeighbors(floorRooms[lX, lY]))
                    {
                        // count how many neighbors the neighbor got
                        int occupiedAround = 0;
                        foreach (var subNeighbor in GetNeighbors(neighbor)) { if (subNeighbor.isOccupied()) occupiedAround++; }
                        
                        if (neighbor.isOccupied() || // skip if the neighbor already exists
                            roomCount > roomCap || // skip if we got enough rooms (consider this condision breaking the whoole loop)
                            roomProb < UnityEngine.Random.Range(0, 10)) // 50% chance to skip
                            continue;

                        // checking if it isn't too crowded 
                        if (occupiedAround >= 2)
                        {
                            // allow 4 rooms to skip this check so we'll get a nice, dense center
                            if (connectorRoomCount > connectiorRoomCap) continue;
                            connectorRoomCount++;
                        }

                        // create the room
                        floorRooms[neighbor.X, neighbor.Y].roomInstance = Instantiate(roomPrefab, new Vector3(neighbor.X * ROOM_WIDTH, neighbor.Y * ROOM_HEIGHT, 0), transform.rotation, transform);
                        roomCount++;
                        generatedAnything = true;
                    }
                    floorRooms[lX, lY].generatedNeighbors = true; 
                }
            }
            if (!generatedAnything) break;
        }
    }

    public void ChangeRoom(Direction dir)
    {
        switch (dir)
        {
            case Direction.UP:
                currentRoom = floorRooms[currentRoom.X, currentRoom.Y + 1];
                pl.transform.position = new Vector3(pl.transform.position.x, pl.transform.position.y + 2.5f, pl.transform.position.z);
                break;
            
            case Direction.DOWN:
                currentRoom = floorRooms[currentRoom.X, currentRoom.Y - 1];
                pl.transform.position = new Vector3(pl.transform.position.x, pl.transform.position.y - 2.5f, pl.transform.position.z);
                break;
            
            case Direction.LEFT:
                currentRoom = floorRooms[currentRoom.X - 1, currentRoom.Y];
                pl.transform.position = new Vector3(pl.transform.position.x - 2.5f, pl.transform.position.y, pl.transform.position.z);
                break;
            
            case Direction.RIGHT:
                currentRoom = floorRooms[currentRoom.X + 1, currentRoom.Y];
                pl.transform.position = new Vector3(pl.transform.position.x + 2.5f, pl.transform.position.y, pl.transform.position.z);
                break;
            
        }
    }
}

public class GameRoom
{
    public int X = 0, Y = 0;
    public GameObject roomInstance = null;
    public bool generatedNeighbors = false;
    public bool isOccupied()
    {
        return roomInstance != null;
    }

    public GameRoom(int roomX, int roomY)
    {
        this.X = roomX;
        this.Y = roomY;
    }
}
