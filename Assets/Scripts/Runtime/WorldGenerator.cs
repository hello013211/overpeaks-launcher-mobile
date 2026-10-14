using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Dirt,
    Stone,
    Wood,
    Lantern,
    HealPotion,
    Pickaxe,
    Torch,
    Seed
}

public class ItemDefinition
{
    public string Id;
    public string DisplayName;
    public ItemType Type;
    public bool Placeable;
    public bool Usable;
    public Color Tint;

    public ItemDefinition(string id, string displayName, ItemType type, bool placeable, bool usable, Color tint)
    {
        Id = id;
        DisplayName = displayName;
        Type = type;
        Placeable = placeable;
        Usable = usable;
        Tint = tint;
    }
}

public class InventorySystem
{
    private readonly List<ItemDefinition> inventory = new List<ItemDefinition>();

    public IReadOnlyList<ItemDefinition> Inventory => inventory;

    public void AddDefaultInventory()
    {
        inventory.Clear();
        inventory.Add(new ItemDefinition("dirt", "Dirt", ItemType.Dirt, true, false, Color.Lerp(Color.yellow, Color.brown, 0.35f)));
        inventory.Add(new ItemDefinition("stone", "Stone", ItemType.Stone, true, false, Color.gray));
        inventory.Add(new ItemDefinition("wood", "Wood", ItemType.Wood, true, false, new Color(0.56f, 0.32f, 0.12f)));
        inventory.Add(new ItemDefinition("torch", "Torch", ItemType.Torch, true, false, new Color(1f, 0.7f, 0.15f)));
        inventory.Add(new ItemDefinition("potion", "Heal Potion", ItemType.HealPotion, false, true, new Color(0.6f, 0.8f, 1f)));
        inventory.Add(new ItemDefinition("pickaxe", "Pickaxe", ItemType.Pickaxe, false, true, new Color(0.5f, 0.5f, 0.5f)));
    }

    public ItemDefinition GetCurrentItem(int index)
    {
        if (index < 0 || index >= inventory.Count)
            return null;

        return inventory[index];
    }

    public int Count => inventory.Count;
}
