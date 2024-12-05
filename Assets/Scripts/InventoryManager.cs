using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager inventoryManager;
    public List<Item> inventory = new List<Item>();

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = this;
        else Destroy(gameObject);
    }

    public void AddFish(Item item)
    {
        inventory.Add(item);
        Debug.Log($"Added {item.itemName} to inventory.");
    }

    public void RemoveFish(Item item)
    {
        if (inventory.Contains(item))
        {
            inventory.Remove(item);
            Debug.Log($"Removed {item.itemName} from inventory.");
        }
    }
}