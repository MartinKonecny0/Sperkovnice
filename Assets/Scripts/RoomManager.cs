using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneData;
using static UnityEditor.Progress;

public class RoomManager : MonoBehaviour
{
    //indexes of rooms
    //   |_0_| 1 |_2_|
    //   |_3_|___|_4_|
    //   |_5_|_6_|_7_|

    public Player player;
    public GameObject presentRoom;
    public GameObject[] positionsParent; //one gameObject position that has all rooms of it's position as childs
    public GameObject[] activeRooms = new GameObject[8];
    public int numOfWorlds = 5;
    public int numOfRooms = 8;
    public GameObject[,] allRooms;

    public SpriteRenderer inventoryIcon;
    public GameObject playerPrefab;
    public GameObject[] allItemsPrefabs;
    public PickupItem[] allItems;
    public GameObject[] allCharactersPrefabs;
    public CharacterItem[] allCharacterItems;

    public int[,] worldsMap =
    {
        // Hedwig's world
        {
            0, 0, 0,
            0, 0,
            0, 0, 0,
        },
        // Engineer's  world
        {
            1, 0, 1,
            1, 1,
            1, 0, 1,
        },
        // Shaman's  world
        {
            2, 2, 1,
            1, 1,
            1, 0, 1,
        },
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
                        allRooms[roomsCounter, worldsCounter] = positionsParent[roomsCounter].transform
                            .GetChild(worldsCounter).gameObject;
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
    public void SwapHiders(GameObject roomToHide, GameObject roomToShow)
    {
        //TODO: find better way how instead of gameObject.Find()
        roomToHide.transform.Find("hider").GetComponent<SpriteRenderer>().enabled = true;
        roomToShow.transform.Find("hider").GetComponent<SpriteRenderer>().enabled = false;
    }

    public void CloseAllHiders()
    {
        foreach (GameObject room in activeRooms)
        {
            if (room != null)
            {
                //TODO: room component required?
                room.transform.Find("hider").GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    private void OpenHider(GameObject room)
    {
        //TODO: room component required?
        room.transform.Find("hider").GetComponent<SpriteRenderer>().enabled = false;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene(1);
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

        // setting up player and characters
        allCharacterItems = new CharacterItem[loadedSceneData.allCharacters.Length];
        int i = 0;
        foreach (characterSave character in loadedSceneData.allCharacters)
        {
            GameObject gameObjectToSpawn = GetCharacterPrefabByType(character.character);
            GameObject spawnedObject = Instantiate(gameObjectToSpawn);
            
            spawnedObject.transform.parent = GetRoomByName(character.roomName).transform;
            spawnedObject.transform.position = new Vector2(character.position[0], character.position[1]);
            if (spawnedObject.GetComponent<CharacterItem>().characterType == loadedSceneData.playerData.character)
            {
                GameObject spawnedPlayer = Instantiate(playerPrefab, spawnedObject.transform);
                player = spawnedPlayer.GetComponent<Player>();
                player.inventoryIcon = inventoryIcon;
                player.roomManager = this;
                player.ChangePlayerToCharacter(null, spawnedObject);

                CloseAllHiders();
                OpenHider(spawnedObject.transform.parent.gameObject);
            }
            allCharacterItems[i] = spawnedObject.GetComponent<CharacterItem>();
            i++;
        }

        // setting up pickup items
        allItems = new PickupItem[loadedSceneData.allPickupItems.Length];
        i = 0;
        foreach (pickupItemSave item in loadedSceneData.allPickupItems)
        {
            GameObject gameObjectToSpawn = GetItemPrefabByName(item.name);
            GameObject spawnedObject = Instantiate(gameObjectToSpawn);
            spawnedObject.transform.parent = GetRoomByName(item.roomName).transform;
            spawnedObject.transform.position = new Vector2(item.position[0], item.position[1]);
            allItems[i] = spawnedObject.GetComponent<PickupItem>();
            if (spawnedObject.GetComponent<PickupItem>().itemName == loadedSceneData.playerData.playerCurrentItem)
            {
                player.PickUpItem(spawnedObject);
            }
            i++;
        }


    }

    public void ClearScene()
    {
        foreach (var item in allItems)
        {
            Destroy(item.gameObject);
        }

        foreach (var character in allCharacterItems)
        {
            Destroy(character.gameObject);
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

        Debug.LogError("Trying to get item with missing prefab. Name of item: " + name);
        return null;
    }

    public GameObject GetCharacterPrefabByType(CharacterType type)
    {
        foreach (GameObject character in allCharactersPrefabs)
        {
            if (character.GetComponent<CharacterItem>().characterType == type)
            {
                return character;
            }
        }

        Debug.LogError("Trying to get character with missing prefab. Name of character: " + name);
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
