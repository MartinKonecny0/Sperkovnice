using System;
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

    public int[,] worldsMap =
    {
        // Engineer's world
        {
            0, 0, 0,
            0,    0,
            0, 0, 0,
        },
        // Hedwig's world
        {
            0, 0, 0,
            0,    0,
            0, 0, 1,
        },
        // Priest's world
        {
            0, 2, 2,
            0,    2,
            2, 2, 2,
        },
        // Shaman's world
        {
            3, 3, 3,
            3,    3,
            3, 3, 2,
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
            0, 0, 0,
            0,    0,
            0, 0, 0,
        },
        // Hedwig's hiders
        {
            2, 1, 1,
            1,    1,
            1, 1, 1,
        },
        // Priest's hiders
        {
            2, 2, 2,
            2,    2,
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
    }

    public void Load()
    {
        SceneData loadedSceneData = SaveManager.Load();
        if (loadedSceneData == null)
        {
            Debug.Log("Load file does not exist -> empty load");
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
                OpenHider(spawnedObject.transform.parent.GetComponent<Room>().roomPositionIndex);
            }
            allCharacterItems.Add(spawnedObject.GetComponent<CharacterItem>());
            i++;
        }

        // setting up pickup items
        i = 0;
        foreach (pickupItemSave item in loadedSceneData.allPickupItems)
        {
            GameObject gameObjectToSpawn = GetItemPrefabByName(item.name);
            GameObject spawnedObject = Instantiate(gameObjectToSpawn);
            spawnedObject.GetComponent<PickupItem>().id = item.id;
            spawnedObject.transform.parent = GetRoomByName(item.roomName).transform;
            spawnedObject.transform.position = new Vector2(item.position[0], item.position[1]);
            allItems.Add(spawnedObject.GetComponent<PickupItem>());
            if (spawnedObject.GetComponent<PickupItem>().id == loadedSceneData.playerData.playerCurrentItemId)
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
