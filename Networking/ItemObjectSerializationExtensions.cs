using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;

public static class ItemObjectSerializationExtensions
{
    public static void WriteValueSafe(this FastBufferWriter writer, in ItemObject itemObject)
    {
        if (itemObject == null)
            return;

        writer.WriteValueSafe(itemObject.ID);
        writer.WriteValueSafe(itemObject.itemObject.Count);
        foreach (var obj in itemObject.itemObject)
        {
            writer.WriteValueSafe(obj.name); // Assuming object names are unique and sufficient
        }
        writer.WriteValueSafe(itemObject.description);
        writer.WriteValueSafe(itemObject.icon.name); // Assuming icon names are unique and sufficient
        writer.WriteValueSafe(itemObject.isDefaultItem);
        writer.WriteValueSafe(itemObject.stackSize);
        writer.WriteValueSafe((int)itemObject.itemType);
        writer.WriteValueSafe((int)itemObject.itemRarity);
        writer.WriteValueSafe((int)itemObject.itemSubType);
    }

    public static void ReadValueSafe(this FastBufferReader reader, out ItemObject itemObject)
    {
        itemObject = ScriptableObject.CreateInstance<ItemObject>();
        if (itemObject == null)
            return;
        reader.ReadValueSafe(out itemObject.ID);
        int itemCount;
        reader.ReadValueSafe(out itemCount);
        itemObject.itemObject = new List<GameObject>();
        for (int i = 0; i < itemCount; i++)
        {
            string objName;
            reader.ReadValueSafe(out objName);
            // Load the GameObject by name or handle it appropriately
            GameObject obj = Resources.Load<GameObject>(objName); // Assuming resources are properly managed
            itemObject.itemObject.Add(obj);
        }
        reader.ReadValueSafe(out itemObject.description);
        string iconName;
        reader.ReadValueSafe(out iconName);
        itemObject.icon = Resources.Load<Sprite>(iconName); // Assuming resources are properly managed
        reader.ReadValueSafe(out itemObject.isDefaultItem);
        reader.ReadValueSafe(out itemObject.stackSize);
        int itemType;
        reader.ReadValueSafe(out itemType);
        itemObject.itemType = (itemType)itemType;
        int itemRarity;
        reader.ReadValueSafe(out itemRarity);
        itemObject.itemRarity = (itemRarity)itemRarity;
        int itemSubType;
        reader.ReadValueSafe(out itemSubType);
        itemObject.itemSubType = (itemType)itemSubType;
    }

    public static void WriteValueSafe(this FastBufferWriter writer, in ItemObject[] itemObjects)
    {
        writer.WriteValueSafe(itemObjects.Length);
        foreach (var itemObject in itemObjects)
        {
            if (itemObject == null)
                return;
            writer.WriteValueSafe(itemObject);
        }
    }

    public static void ReadValueSafe(this FastBufferReader reader, out ItemObject[] itemObjects)
    {
        reader.ReadValueSafe(out int length);
        itemObjects = new ItemObject[length];
        for (int i = 0; i < length; i++)
        {
            if (itemObjects[i] != null)
                reader.ReadValueSafe(out itemObjects[i]);
        }
    }
}
