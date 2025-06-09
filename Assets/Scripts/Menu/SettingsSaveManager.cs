using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class SettingsSaveManager
{
    public static Dictionary<int, string> allBindingStrings;
    public static float soundValue;
    public static void Save()
    {
        string path = Application.persistentDataPath + "/settingsSave.json";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        SettingsData sceneData = new SettingsData(allBindingStrings, soundValue);

        string saveString = JsonUtility.ToJson(sceneData, true);
        byte[] bytesToStore = new UTF8Encoding(true).GetBytes(saveString);

        fileStream.Write(bytesToStore);
        fileStream.Close();
        Debug.Log("Settings saved successfully - into file " + path);
    }
    /// <summary>
    /// Method finds save file and gets saved data from loaded json
    /// </summary>
    /// <returns>scene data or null if there is no such file</returns>
    public static SettingsData Load()
    {
        string path = Application.persistentDataPath + "/settingsSave.json";

        string fileContent;
        if (File.Exists(path))
        {
            using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fileStream))
            {
                fileContent = reader.ReadToEnd();
            }
            SettingsData loadedSceneData = JsonUtility.FromJson<SettingsData>(fileContent);
            return loadedSceneData;
        }
        return null;
    }
}