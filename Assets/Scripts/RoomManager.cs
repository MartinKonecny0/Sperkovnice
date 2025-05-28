using System.Collections.Generic;
using UnityEngine;
using static SceneData;

public class RoomManager : MonoBehaviour
{
    //indexes of rooms
    //   |_0_| 1 |_2_|
    //   |_3_|___|_4_|
    //   |_5_|_6_|_7_|

    public GameObject player;
    public GameObject presentRoom;
    public GameObject[] positionsParent; //one gameObject position that has all rooms of it's position as childs
    public GameObject[] activeRooms = new GameObject[8];
    public int numOfWorlds = 5;
    public int numOfRooms = 8;
    public GameObject[,] allRooms;


    public GameObject[] allItemsPrefabs;
    public PickupItem[] allItems;
    public GameObject[] allCharactersPrefabs;
    public CharacterItem[] allCharacterItems;

    public int[,] worldsMap = 
    { 
        // Hedwig's world
        {0, 0, 0,
         0,    0,
         0, 0, 0,},
        // Engineer's  world
        {1, 0, 1,
         1,    1,
         1, 0, 1,},
        // Shaman's  world
        {2, 2, 1,
         1,    1,
         1, 0, 1,},
    };


    private void Start()
    {
        allRooms = new GameObject[numOfRooms, numOfWorlds];
        for (int roomsCounter = 0; roomsCounter < numOfRooms; roomsCounter++)
        {
            for (int worldsCounter = 0; worldsCounter < numOfWorlds; worldsCounter++)
            {
                if (positionsParent[roomsCounter] != null)
                {
                    if (positionsParent[roomsCounter].transform.childCount > worldsCounter)
                    { 
                        allRooms[roomsCounter, worldsCounter] = positionsParent[roomsCounter].transform.GetChild(worldsCounter).gameObject;
                    }

                }
            }
        }
    }

    private void PrintAllRooms()
    {
        for (int roomsCounter = 0; roomsCounter < numOfRooms; roomsCounter++)
        {
            for (int worldsCounter = 0; worldsCounter < numOfWorlds; worldsCounter++)
            {
                    print(roomsCounter + " " + worldsCounter + " " + allRooms[roomsCounter, worldsCounter]);
            }
        }
    }

    /// <summary>
    /// changes active rooms depending on character
    /// </summary>
    /// <param name="characterType">enum value is used for indexing in worldsMap array </param>
    public void ChangeWorld(CharacterType characterType)
    {
        DeactivateAllRooms();
        int worldIndex = (int)characterType;
        for (int i = 0; i < numOfRooms; i++)
        {
            int indexFromMap = worldsMap[worldIndex, i];
            if (allRooms[i, indexFromMap] != null)
            {
                allRooms[i, indexFromMap].SetActive(true);
                activeRooms[i] = allRooms[i, indexFromMap];
            }
        }
    }
    public void DeactivateAllRooms()
    {
        for (int i = 0; i < numOfRooms; i++)
        {
            if (activeRooms[i] != null)
            { 
                activeRooms[i].SetActive(false);
            }
        }
    }
    /// <summary>
    /// changes hiders of two rooms while changing rooms
    /// </summary>
    public void UpdateHiders(GameObject roomToHide, GameObject roomToShow)
    {
        //TODO: find better way how instead of gameObject.Find()
        roomToHide.transform.Find("hider").GetComponent<SpriteRenderer>().enabled = true;
        roomToShow.transform.Find("hider").GetComponent<SpriteRenderer>().enabled = false;
    }


    public void Save()
    {
        SetupSave();
        SaveManager.Save();
    }
    

    public void SetupSave()
    {
        SaveManager.allItems = allItems;
        SaveManager.allCharacterItems = allCharacterItems;
    }
    public void Load()
    {
        SceneData loadedSceneData = SaveManager.Load();
        ClearScene();



        allItems = new PickupItem[loadedSceneData.allPickupItems.Length];
        int i = 0;
        foreach (pickupItemSave item in loadedSceneData.allPickupItems)
        {
            GameObject gameObjectToSpawn = GetItemPrefabByName(item.name);
            GameObject spawnedObject = Instantiate(gameObjectToSpawn);
            spawnedObject.transform.parent = GetRoomByName(item.roomName).transform;
            spawnedObject.transform.position = new Vector2(item.position[0],  item.position[1]);
            allItems[i] = spawnedObject.GetComponent<PickupItem>();
            i++;
        }



    }
    public void ClearScene()
    {
        foreach (var item in allItems)
        {
            Destroy(item.gameObject);
        }
    }
    public GameObject GetItemPrefabByName(string name)
    {
        foreach (GameObject item in allItemsPrefabs)
        {
            if (item.GetComponent<PickupItem>().itemName == name)
            {
                return item;
            }
        }

        Debug.LogError("Trying to spawn item with missing prefab. Name of item: " + name);
        return null;
    }

    public GameObject GetRoomByName(string name)
    {
        foreach (GameObject room in allRooms)
        {
            if (room != null)
            {
                if (name == room.name)
                {
                    return room;
                }
            }
        }

        Debug.LogError("Trying to get room with invalid name. Name of room: " + name);
        return null;
    }
}
