using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProceduralRoomGenerator : MonoBehaviour
{
    // TODO: add randomizer to sizes
    public int numRooms = 10;
    private List<Bounds> roomBounds = new List<Bounds>();
    public Vector2 roomSizeMinMax = new Vector2(3,6);
    public Vector2 bigRoomSize = new Vector2(100, 100);
    public GameObject wallPrefab, doorPrefab, WindowPrefab;
    void Start()
    {
        GenerateBigRoom();
        GenerateRooms();
    }
    
    void Update()
    {
        
    }
    void GenerateBigRoom()
    {
        GameObject bigRoom = new GameObject("BigRoom");
        GenerateWalls(bigRoom, Vector3.zero, bigRoomSize.x, bigRoomSize.y,10);
    }
    public void GenerateRooms()
    {
        for (int i = 0; i < numRooms; i++)
        {
            bool roomPlaced = false;
            int maxAttempts = 200; // Avoid infinite loops
            int attempts = 0;
            while (!roomPlaced && attempts < maxAttempts)
            {
                Debug.Log("Attemps = "+ attempts);
                attempts++;
                float roomWidth = Random.Range(roomSizeMinMax.x, roomSizeMinMax.y);
                float roomHeight = Random.Range(roomSizeMinMax.x, roomSizeMinMax.y);

                float posX = Random.Range(-bigRoomSize.x/2 + roomWidth/2, bigRoomSize.x / 2 - roomWidth/2);
                float posZ = Random.Range(-bigRoomSize.y/2 + roomWidth/2, bigRoomSize.y / 2 - roomWidth/2);

                Bounds newRoomBounds = new Bounds(new Vector3(posX, 0, posZ), new Vector3(roomWidth, 1, roomHeight));
                if (!IsOverlapping(newRoomBounds, roomBounds))
                {
                    roomBounds.Add(newRoomBounds);
                    roomPlaced = true;
                    GameObject room = new GameObject("Room_" + i);
                    room.transform.position = new Vector3(posX, 0, posZ);
                    GenerateWalls(room, room.transform.position, roomWidth, roomHeight, 2);
                    Debug.Log("HEre");
                }
            }
            if(i == 0)
            {
                GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                player.name = "Player";
                player.transform.position = new Vector3(1, 0.5f, 1);
                player.AddComponent<Rigidbody>().freezeRotation = true;
                player.AddComponent<PlayerController>();

            }

            roomPlaced = false;
            
        }
    }
    private bool IsOverlapping(Bounds newBounds, List<Bounds> existingBounds)
    {
        foreach (Bounds bounds in existingBounds)
        {
            if (bounds.Intersects(newBounds))
            {
                return true;
            }
        }
        return false;
    }
    private void GenerateWalls(GameObject parent, Vector3 position, float width, float height, float wallHeight)
    {
        Vector3[] wallPosition = 
        {
            new Vector3(position.x, wallHeight / 2, position.z + height / 2),
            new Vector3(position.x, wallHeight / 2, position.z - height / 2),
            new Vector3(position.x + width / 2, wallHeight / 2,  position.z),
            new Vector3(position.x - width / 2, wallHeight / 2,  position.z)
        };

        Vector3[] wallScales = {
            new Vector3(width, wallHeight, 0.1f),
            new Vector3(width, wallHeight, 0.1f),
            new Vector3(0.1f, wallHeight, height),
            new Vector3(0.1f, wallHeight, height)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = wallPosition[i];
            wall.transform.localScale = wallScales[i];
            wall.transform.parent = parent.transform;
        }

        GenerateDoorAndWindows(parent, position, width, height, wallHeight);

    }

    public void GenerateDoorAndWindows(GameObject parent, Vector3 position, float width, float height, float wallHeight)
    {
        Vector3 doorPosition = new Vector3(position.x, 1, position.z - height / 2);
        GameObject door = GameObject.CreatePrimitive(PrimitiveType.Cube);
        door.transform.position = doorPosition;
        door.transform.localScale = new Vector3(1, 2, 0.1f);
        door.GetComponent<Renderer>().material.color = Color.blue;
        door.transform.parent = parent.transform;

        Vector3 windowPosition = new Vector3(position.x + width / 2, 1.5f, position.z);
        GameObject window = GameObject.CreatePrimitive(PrimitiveType.Cube);
        window.transform.position = windowPosition;
        window.transform.localScale = new Vector3(0.1f, 1, 1);
        window.GetComponent<Renderer>().material.color = Color.red;
        window.transform.parent = parent.transform;
    }
}
