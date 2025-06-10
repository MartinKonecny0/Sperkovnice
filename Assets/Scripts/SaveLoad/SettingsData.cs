using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class SettingsData
{
    [System.Serializable]
    public struct soundSave
    {
        public float soundVolume;
    }

    [System.Serializable]
    public struct bindSave
    {
        public BinderElement.binderType binderType;
        public string bindingString;
    }

    public soundSave soundData;
    public List<bindSave> bindingData;

    public SettingsData(Dictionary<int, string> currBindDictionary, float soundVolume)
    {
        soundData = new soundSave();
        bindingData = new List<bindSave>();
        soundData.soundVolume = soundVolume;

        foreach (KeyValuePair<int, string> bind in currBindDictionary)
        {
            bindSave newSave = new bindSave();
            newSave.binderType = (BinderElement.binderType)bind.Key;
            newSave.bindingString = bind.Value;
            bindingData.Add(newSave);
        }
    }
}