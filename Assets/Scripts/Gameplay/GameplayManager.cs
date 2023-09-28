using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    public GameRoom currentRoom;
    public GameObject roomPrefab;
    Camera mainCamera;
    bool gameplayReady = false;

    /*******FLOOR GENERATION VARIABLES*******/
    public static float ROOM_WIDTH = 30, ROOM_HEIGHT = 16.5f;
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
        GenerateFloor();
        currentRoom = floorRooms[10, 10];
        gameplayReady = true;
        GetComponentInChildren<PlayerController>().transform.position = new Vector3(
            currentRoom.roomInstance.transform.position.x,
            currentRoom.roomInstance.transform.position.y,
            0);

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
            for (int lY = 0; lY < floorRooms.GetLength(1); lY++)
            {
                for (int lX = 0; lX < floorRooms.GetLength(0); lX++)
                {
                    // skip if the room is occupied 
                    if (!floorRooms[lX, lY].isOccupied()) continue;
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
                    }

                }
            }
        }
    }
}

public class GameRoom
{
    public int X = 0, Y = 0;
    public GameObject roomInstance = null;
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
