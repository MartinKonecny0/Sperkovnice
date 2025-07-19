using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

public static class SaveManager
{
    public static int saveSlot;
    public static Player player;
    public static List<PickupItem> allItems;
    public static List<CharacterItem> allCharacterItems;
    public static List<TaskObject> allTaskItems;
    public static List<ItemWithRequirement> allItemsWithRequirements;

    public static void Save()
    {
        string path = Application.persistentDataPath + "/saveFile" + saveSlot + ".json";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        SceneData sceneData = new SceneData(player, allItems, allCharacterItems, allTaskItems, allItemsWithRequirements);

        string saveString = JsonUtility.ToJson(sceneData, true);
        byte[] bytesToStore = new UTF8Encoding(true).GetBytes(saveString);

        fileStream.Write(bytesToStore);
        fileStream.Close();
        Debug.Log("Saved successful - into file " + path);
    }
    /// <summary>
    /// Method finds save file and gets saved data from loaded json
    /// </summary>
    /// <returns>scene data or null if there is no such file</returns>
    public static SceneData Load()
    {
        string path = Application.persistentDataPath + "/saveFile" + saveSlot + ".json";

        string fileContent;
        if (File.Exists(path))
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                fileContent = reader.ReadToEnd();
            }
            SceneData loadedSceneData = JsonUtility.FromJson<SceneData>(fileContent);
            return loadedSceneData;
        }
        return null;
    }
}
