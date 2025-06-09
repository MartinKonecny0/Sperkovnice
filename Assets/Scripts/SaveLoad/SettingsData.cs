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
    public bindSave[] bindingData;

    public SettingsData(Dictionary<int, string> allBindingString, float soundVolume)
    {
        soundData = new soundSave();
        bindingData = new bindSave[allBindingString.Count];
        soundData.soundVolume = soundVolume;

        for(int i = 0; i < allBindingString.Count; i++)
        {
            bindSave newSave = new bindSave();
            newSave.bindingString = allBindingString[i];
            newSave.binderType = (BinderElement.binderType)i;
            bindingData[i] = newSave;
        }
    }
}