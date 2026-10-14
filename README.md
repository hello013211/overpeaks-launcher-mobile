using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class BlockSaveRecord
{
    public int x;
    public int y;
    public int z;
    public string itemId;
}

[Serializable]
public class WorldSaveData
{
    public List<BlockSaveRecord> blocks = new List<BlockSaveRecord>();
    public List<string> npcNames = new List<string>();
}

public static class SaveManager
{
    public static void SaveWorld(WorldGenerator generator)
    {
        var path = Application.persistentDataPath + "/overpeaks_save.json";
        var save = new WorldSaveData();

        foreach (var block in generator.Blocks)
        {
            var transform = block.transform;
            var record = new BlockSaveRecord
            {
                x = Mathf.RoundToInt(transform.position.x),
                y = Mathf.RoundToInt(transform.position.y),
                z = Mathf.RoundToInt(transform.position.z),
                itemId = block.name
            };
            save.blocks.Add(record);
        }

        File.WriteAllText(path, JsonUtility.ToJson(save, true));
        Debug.Log("Saved world to: " + path);
    }

    public static WorldSaveData LoadWorld()
    {
        var path = Application.persistentDataPath + "/overpeaks_save.json";
        if (!File.Exists(path))
            return new WorldSaveData();

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<WorldSaveData>(json);
    }
}
