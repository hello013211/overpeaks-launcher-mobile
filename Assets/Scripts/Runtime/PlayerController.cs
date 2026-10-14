using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    private readonly List<GameObject> blocks = new List<GameObject>();
    private readonly Dictionary<Vector3Int, GameObject> blockMap = new Dictionary<Vector3Int, GameObject>();

    public IReadOnlyCollection<GameObject> Blocks => blocks;

    public void BuildStarterWorld()
    {
        CreateGround();
        CreateStarterStructures();
    }

    private void CreateGround()
    {
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.SetParent(transform);
        ground.transform.localScale = new Vector3(40f, 1f, 40f);
        ground.transform.position = new Vector3(0f, -0.5f, 0f);
        ground.GetComponent<Renderer>().material.color = new Color(0.38f, 0.68f, 0.34f);
        ground.tag = "Ground";
    }

    private void CreateStarterStructures()
    {
        CreateBlock(new Vector3Int(0, 1, 0), ItemType.Stone);
        CreateBlock(new Vector3Int(1, 1, 0), ItemType.Stone);
        CreateBlock(new Vector3Int(-1, 1, 0), ItemType.Dirt);
        CreateBlock(new Vector3Int(0, 2, 0), ItemType.Wood);
        CreateBlock(new Vector3Int(3, 1, 2), ItemType.Torch);
        CreateBlock(new Vector3Int(-3, 1, -2), ItemType.Wood);
        CreateBlock(new Vector3Int(2, 1, -3), ItemType.Stone);
        CreateBlock(new Vector3Int(-2, 1, 5), ItemType.Dirt);
        CreateBlock(new Vector3Int(5, 2, 5), ItemType.Wood);
        CreateBlock(new Vector3Int(8, 1, -4), ItemType.Stone);
    }

    public void CreateBlock(Vector3Int pos, ItemType type)
    {
        if (blockMap.ContainsKey(pos))
            return;

        var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = "Block_" + type;
        block.transform.SetParent(transform);
        block.transform.position = pos;
        block.tag = "Block";

        var color = GetColorForType(type);
        block.GetComponent<Renderer>().material.color = color;

        blocks.Add(block);
        blockMap[pos] = block;
    }

    public bool RemoveBlockAt(Vector3 position)
    {
        Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));

        if (blockMap.TryGetValue(gridPos, out var target))
        {
            blocks.Remove(target);
            blockMap.Remove(gridPos);
            Destroy(target);
            return true;
        }

        return false;
    }

    public bool TryPlaceBlock(Vector3 position, ItemType type)
    {
        Vector3Int gridPos = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), Mathf.RoundToInt(position.z));
        if (blockMap.ContainsKey(gridPos))
            return false;

        CreateBlock(gridPos, type);
        return true;
    }

    private Color GetColorForType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Dirt:
                return new Color(0.48f, 0.29f, 0.15f);
            case ItemType.Stone:
                return Color.gray;
            case ItemType.Wood:
                return new Color(0.56f, 0.32f, 0.12f);
            case ItemType.Torch:
                return new Color(1f, 0.7f, 0.15f);
            case ItemType.HealPotion:
                return new Color(0.6f, 0.8f, 1f);
            default:
                return Color.white;
        }
    }
}
