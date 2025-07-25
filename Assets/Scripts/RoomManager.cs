using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneData;
using static UnityEditor.Progress;

public class RoomManager : MonoBehaviour
{
    //indexes of rooms
    //   |_0_|_1_|_2_|
    //   |_3_|___|_4_|
    //   |_5_|_6_|_7_|

    public Player player;
    public GameObject presentRoom;
    public GameObject[] positionsParent; //one gameObject position that has all rooms of it's position as childs
    public GameObject[] activeRooms = new GameObject[8];
    public int numOfWorlds = 5;
    public int numOfRooms = 8;
    public GameObject[,] allRooms;
    public GameObject[] allHiders;

    public SpriteRenderer inventoryIcon;
    public GameObject playerPrefab;
    public GameObject[] allItemsPrefabs;
    public List<PickupItem> allItems;
    public GameObject[] allCharactersPrefabs;
    public List<CharacterItem> allCharacterItems;
    public List<TaskObject> allTaskItems;
    public List<ItemWithRequirement> allItemsWithRequirements;

    public int[,] worldsMap =
    {
        // Engineer's world
        {
            0, 0, 2,
            0,    0,
            0, 0, 0,
        },
        // Hedwig's world
        {
            1, 1, 0,
            1,    1,
            0, 1, 0,
        },
        // Priest's world
        {
            1, 2, 1,
            2,    1,
            1, 2, 1,
        },
        // Shaman's world
        {
            2, 3, 2,
            3,    2,
            2, 3, 1,
        },
        // Aunt's world
        {
            4, 4, 4,
            4,    4,
            3, 4, 4,
        },
    };

    public int[,] hidersMap =
    {
        // Engineer's hiders
        {
            0, 0, 3,
            0,    0,
            0, 0, 0,
        },
        // Hedwig's hiders
        {
            1, 1, 1,
            1,    1,
            1, 1, 0,
        },
        // Priest's hiders
        {
            2, 2, 2,
            2,    1,
            2, 2, 2,
        },
        // Shaman's hiders
        {
            3, 3, 3,
            3,    3,
            3, 3, 3,
        },
        // Aunt's hiders
        {
            4, 4, 4,
            4,    4,
            4, 4, 4,
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
                        if (allRooms[roomsCounter, worldsCounter] != null)
                        {
                            allRooms[roomsCounter, worldsCounter].GetComponent<Room>().roomPositionIndex = roomsCounter;
                        }
                    }

                }
            }
        }
        
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        Load();
        OpenCurrentHider();
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

        for (int i = 0; i < allHiders.Length; i++)
        {
            int hiderType = hidersMap[(int)characterType, i];
            allHiders[i].GetComponentInChildren<Animator>().SetFloat("position", hiderType);
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

    public void OpenHider(int roomPositionIndex)
    {
        if (roomPositionIndex < allHiders.Length)
        {
            if (allHiders[roomPositionIndex] != null)
            {
                allHiders[roomPositionIndex].GetComponentInChildren<Animator>().SetBool("isOpen", true);
            }
        }
    }

    public void OpenCurrentHider()
    {
        GameObject character = player.transform.parent.gameObject;
        int index = character.transform.parent.GetComponent<Room>().roomPositionIndex;
        OpenHider(index);
    }

    public void CloseHider(int roomPositionIndex)
    {
        if (roomPositionIndex < allHiders.Length)
        {
            if (roomPositionIndex < allHiders.Length)
            {
                allHiders[roomPositionIndex].GetComponentInChildren<Animator>().SetBool("isOpen", false);

            }
        }
    }

    public void CloseAllHiders()
    {
        for (int i = 0; i < allHiders.Length; i++)
        {
            CloseHider(i);
        }
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
        SaveManager.allTaskItems = allTaskItems;
        SaveManager.allItemsWithRequirements = allItemsWithRequirements;
    }

    public void Load()
    {
        SceneData loadedSceneData = SaveManager.Load();
        if (loadedSceneData == null)
        {
            Debug.Log("Load file does not exist -> empty load");
            ChangeWorld(CharacterType.Hedwig);
            return;
        }

        ClearScene();

        // setting up player and characters
        int i = 0;
        foreach (characterSave character in loadedSceneData.allCharacters)
        {
            GameObject gameObjectToSpawn = GetCharacterPrefabByType(character.character);
            GameObject spawnedObject = Instantiate(gameObjectToSpawn);
            
            spawnedObject.transform.parent = GetRoomByName(character.roomName).transform;
            spawnedObject.transform.position = new Vector2(character.position[0], character.position[1]);
            if (character.isWalkingToDefault)
            {
                spawnedObject.GetComponent<CharacterItem>().StartWalkToDefault();
            }
            if (spawnedObject.GetComponent<CharacterItem>().characterType == loadedSceneData.playerData.character)
            {
                GameObject spawnedPlayer = Instantiate(playerPrefab, spawnedObject.transform);
                player = spawnedPlayer.GetComponent<Player>();
                player.inventoryIcon = inventoryIcon;
                player.roomManager = this;
                player.ChangePlayerToCharacter(null, spawnedObject);
                CloseAllHiders();
                presentRoom = spawnedObject.transform.parent.gameObject;
            }
            allCharacterItems.Add(spawnedObject.GetComponent<CharacterItem>());
            i++;
        }

        // setting up pickup items
        i = 0;

        PickupItem.PickupItemType playerItemType = loadedSceneData.playerData.playerCurrentItemType;
        int playerItemId = loadedSceneData.playerData.playerCurrentItemId;

        foreach (pickupItemSave item in loadedSceneData.allPickupItems)
        {
            GameObject gameObjectToSpawn = GetItemPrefabByType(item.itemType);
            GameObject spawnedObject = Instantiate(gameObjectToSpawn);
            spawnedObject.SetActive(item.isActive);
            spawnedObject.GetComponent<PickupItem>().id = item.id;
            spawnedObject.transform.parent = GetRoomByName(item.roomName).transform;
            spawnedObject.transform.position = new Vector2(item.position[0], item.position[1]);
            allItems.Add(spawnedObject.GetComponent<PickupItem>());
            if (spawnedObject.GetComponent<PickupItem>().itemType == playerItemType 
                && spawnedObject.GetComponent<PickupItem>().id == playerItemId)
            {
                player.PickUpItem(spawnedObject);
            }
            i++;
        }

        foreach (taskItemSave task in loadedSceneData.allTaskItems)
        {
            TaskObject taskItem = GetTaskInstanceByType(task.itemType);
            taskItem.gameObject.SetActive(task.isActive);
            taskItem.isCompleted = task.isCompleted;
            taskItem.itemsLeft.Clear();
            foreach (PickupItem.PickupItemType requiredItem in task.remainingRequiredItems)
            {
                taskItem.itemsLeft.Add(requiredItem);
            }
        }

        foreach (itemWithRequirement item in loadedSceneData.allItemsWithRequirement)
        {
            ItemWithRequirement itemWithRequirement = GetItemWithRequirementByType(item.itemType);
            itemWithRequirement.gameObject.SetActive(item.isActive);
            itemWithRequirement.isCompleted = item.isCompleted;
            itemWithRequirement.isNecessaryDiscovered = item.isNecessaryDiscovered;
        }
    }

    public void ClearScene()
    {
        foreach (var item in allItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        } 
        allItems.Clear();
        
        foreach (var character in allCharacterItems)
        {
            if (character != null)
            {
                Destroy(character.gameObject);
            }
        }
        allCharacterItems.Clear();

        inventoryIcon.sprite = null;
    }

    public GameObject GetItemPrefabByType(PickupItem.PickupItemType type)
    {
        foreach (GameObject item in allItemsPrefabs)
        {
            if (item.GetComponent<PickupItem>().itemType == type)
            {
                return item;
            }
        }

        Debug.LogError("Trying to get item with missing prefab. Type of item: " + type);
        return null;
    }
    public PickupItem GetItemInstanceById(PickupItem.PickupItemType typeOfItem, int idToFind)
    {
        foreach (PickupItem item in allItems)
        {
            if (item.itemType == typeOfItem && item.id == idToFind)
            {
                return item;
            }
        }

        Debug.LogError("Trying to get item missing instance. Type of item: " + typeOfItem + " ID of item instance: " + idToFind);
        return null;
    }
    public CharacterItem GetCharacterInstanceByType(CharacterType type)
    {
        foreach (CharacterItem character in allCharacterItems)
        {
            if (character.characterType == type)
            {
                return character;
            }
        }

        Debug.LogError("Trying to get character missing instance. Type of character: " + type);
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

        Debug.LogError("Trying to get character with missing prefab. Type of character: " + type);
        return null;
    }
    public TaskObject GetTaskInstanceByType(TaskObject.TaskObjectType taskType)
    {
        foreach (TaskObject task in allTaskItems)
        {
            if (task.itemType == taskType)
            {
                return task;
            }
        }
        Debug.LogError("Trying to get task missing type. Type of task: " + taskType);
        return null;
    }
    public ItemWithRequirement GetItemWithRequirementByType(ItemWithRequirement.ItemWithRequirementType taskType)
    {
        foreach (ItemWithRequirement item in allItemsWithRequirements)
        {
            if (item.itemType == taskType)
            {
                return item;
            }
        }
        Debug.LogError("Trying to get task missing type. Type of task: " + taskType);
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

    public void UpdateNecessaryItems(PickupItem.PickupItemType itemType)
    {
        foreach (var item in allItemsWithRequirements)
        {
            if (item != null)
            {
                if (item.necessaryItem == itemType)
                {
                    item.NecessaryItemDiscovered();
                }
            }
        }
    }
}
