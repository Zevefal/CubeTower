using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadService
{
    private string saveFilePath => Path.Combine(Application.persistentDataPath, "tower_save.json");

    public void Save(TowerData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
    }

    public TowerData Load()
    {
        if (!File.Exists(saveFilePath)) return null;

        string json = File.ReadAllText(saveFilePath);
        return JsonUtility.FromJson<TowerData>(json);
    }
}

[System.Serializable]
public class TowerData
{
    public List<Vector2> cubePositions;
    public List<Color> cubeColors;
}