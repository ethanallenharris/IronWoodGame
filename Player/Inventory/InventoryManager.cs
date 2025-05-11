using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public PlayerStateMachine player;
    public List<GameObject> slots;
    [HideInInspector]
    public List<InventorySlot> inventorySlots;

    // Start is called before the first frame update
    void Start()
    {
        inventorySlots = slots.ConvertAll(s => s.GetComponent<InventorySlot>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Item Get(InventorySlot pItem)
    {
        //if(itemDictionary.TryGetValue(referenceData, out Item value))
        {
            //return value;
        }
        return null;
    }

    public bool ItemInInventory(ItemObject pItem)
    {
        // DO THIS WITH LINQ
        foreach (GameObject slot in slots)
        {
            if (slot.GetComponent<InventorySlot>().item == pItem)
            {
                return true;
            }
        }
        return false;
    }

    public int GetFreeSlot()
    {
        int count = 0;
        foreach (GameObject slot in slots)
        {
            if (slot.GetComponent<InventorySlot>().item == null)
            {
                return count;
            }
            count++;
        }
        return 999;
    }



    public void Add(ItemObject pItem)
    {
        int slotNumber = GetFreeSlot();
        if (slotNumber < 999)
        {
            slots[slotNumber].GetComponent<InventorySlot>().newItem(pItem);
        }
        else
        {
            
        }
    }

    public void Remove(InventorySlot pSlot)
    {

    }

}
