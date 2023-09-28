using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    public GameFloor currentFloor;
    public GameRoom currentRoom;
    public GameObject floorPrefab;
    Camera mainCamera;

    bool gameplayReady = false;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        if (currentFloor == null) Instantiate(floorPrefab, this.transform);
        currentFloor = transform.GetComponentInChildren<GameFloor>();
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


            if (Input.GetKeyDown(KeyCode.RightArrow)) currentRoom = currentFloor.floorRooms[currentRoom.roomX + 1, currentRoom.roomY];
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) currentRoom = currentFloor.floorRooms[currentRoom.roomX - 1, currentRoom.roomY];
            if (Input.GetKeyDown(KeyCode.UpArrow)) currentRoom = currentFloor.floorRooms[currentRoom.roomX, currentRoom.roomY + 1];
            else if (Input.GetKeyDown(KeyCode.DownArrow)) currentRoom = currentFloor.floorRooms[currentRoom.roomX, currentRoom.roomY - 1];
        }
    }

    public void GameplayReady()
    {
        currentRoom = currentFloor.floorRooms[10, 10];
        gameplayReady = true;

        GetComponentInChildren<PlayerController>().transform.position = new Vector3(currentRoom.roomInstance.transform.position.x,
            currentRoom.roomInstance.transform.position.y,
            0 );
    }
}
