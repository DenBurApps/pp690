using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class PetDataSaveSystem
{
    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "PetData.json");

    public static void Save(List<PetData> petDataList)
    {
        PetDataListWrapper wrapper = new PetDataListWrapper(petDataList);
        string json = JsonConvert.SerializeObject(wrapper, Formatting.Indented);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Pet data saved to: {SavePath}");
    }

    public static List<PetData> Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found, returning empty list.");
            return new List<PetData>();
        }

        var json = File.ReadAllText(SavePath);
        var wrapper = JsonConvert.DeserializeObject<PetDataListWrapper>(json);
        Debug.Log("Pet data loaded successfully.");
        return wrapper.PetDataList;
    }

    [System.Serializable]
    private class PetDataListWrapper
    {
        public List<PetData> PetDataList;

        public PetDataListWrapper(List<PetData> petDataList)
        {
            PetDataList = petDataList;
        }
    }
}
