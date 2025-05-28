using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Runtime.Serialization.Formatters;
using UnityEngine.InputSystem.Interactions;
using System.Text;
using System.IO.Pipes;
using System;

public static class SaveManager
{
    public static Player player;
    public static PickupItem[] allItems;
    public static CharacterItem[] allCharacterItems;

    public static void Save()
    {
        string path = Application.persistentDataPath + "/saveFile.json";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        SceneData sceneData = new SceneData(player, allItems, allCharacterItems);

        string saveString = JsonUtility.ToJson(sceneData, true);
        Debug.Log(saveString);
        byte[] bytesToStore = new UTF8Encoding(true).GetBytes(saveString);

        fileStream.Write(bytesToStore);
        fileStream.Close();
    }

    public static SceneData Load()
    {
        string path = Application.persistentDataPath + "/saveFile.json";

        string fileContent;

        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new StreamReader(fileStream))
        {
            fileContent = reader.ReadToEnd();
        }

        SceneData loadedSceneData = JsonUtility.FromJson<SceneData>(fileContent);
        return loadedSceneData;
    }
}
