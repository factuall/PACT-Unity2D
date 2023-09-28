using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFloor : MonoBehaviour
{

    public GameObject roomPrefab;
    public GameplayManager GM;

    const float ROOM_WIDTH = 30, ROOM_HEIGHT = 16.5f;
    const int floorSize = 20;
    public int connectiorRoomCap = 3;
    public int roomCap = 10;
    public int roomProb = 5;
    public GameRoom[,] floorRooms = new GameRoom[floorSize, floorSize];

    // Start is called before the first frame update
    void Start()
    {
        GM = transform.parent.GetComponent<GameplayManager>();
        for (int lY = 0; lY < floorRooms.GetLength(1); lY++)
        {
            for (int lX = 0; lX < floorRooms.GetLength(0); lX++)
            {
                floorRooms[lX, lY] = new GameRoom(lX, lY);
            }
        }

        int roomCount = 0;
        int connectorRoomCount = 0;
        floorRooms[10, 10].InitRoom(this); roomCount++;

        while(roomCount < roomCap)
        {
            for (int lY = 0; lY < floorRooms.GetLength(1); lY++)
            {
                for (int lX = 0; lX < floorRooms.GetLength(0); lX++)
                {
                    if (!floorRooms[lX, lY].occupied) continue;
                    foreach (GameRoom neighbor in GetNeighbors(floorRooms[lX, lY]))
                    {
                        int occupiedAround = 0;
                        foreach (var subNeighbor in GetNeighbors(neighbor)) { if (subNeighbor.occupied) occupiedAround++;}
                        if (neighbor.occupied ||
                            roomCount > roomCap ||
                            roomProb < UnityEngine.Random.Range(0, 10)) continue;

                        if(occupiedAround >= 2)
                        {
                            if (connectorRoomCount > connectiorRoomCap) continue;
                            connectorRoomCount++;
                        }   
                        neighbor.InitRoom(this); roomCount++;
                    }
                    
                }
            }
        }
        GM.GameplayReady();

    }

    public GameRoom[] GetNeighbors(GameRoom origin)
    {
        var neighbors = new List<GameRoom>();
        if(origin.roomX > 0)              neighbors.Add(floorRooms[origin.roomX - 1, origin.roomY]);
        if(origin.roomX < floorSize - 1)  neighbors.Add(floorRooms[origin.roomX + 1, origin.roomY]);
        if (origin.roomY > 0)             neighbors.Add(floorRooms[origin.roomX, origin.roomY - 1]);
        if (origin.roomY < floorSize - 1) neighbors.Add(floorRooms[origin.roomX, origin.roomY + 1]);
        return neighbors.ToArray();
    }

    public void MaterializeRoom(GameRoom room)
    {
        floorRooms[room.roomX,room.roomY].roomInstance = Instantiate(roomPrefab, new Vector3(room.roomX*ROOM_WIDTH, room.roomY*ROOM_HEIGHT, 0), this.transform.rotation, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class GameRoom
{
    public int roomX = 0, roomY = 0;

    public bool spreaded = false;
    public bool occupied = false;
    public GameFloor parentFloor;
    public GameObject roomObject;
    public GameObject roomInstance;

    public GameRoom(int X, int Y)
    {
        roomX = X;
        roomY = Y;
    }

    public GameRoom(int X, int Y, bool init, GameFloor parent)
    {
        roomX = X;
        roomY = Y;
        occupied = init;
        parentFloor = parent;
    }

    public void InitRoom(GameFloor parent)
    {
        occupied = true;
        parentFloor = parent;
        parent.MaterializeRoom(this);

    }

}
