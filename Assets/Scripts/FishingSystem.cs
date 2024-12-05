using UnityEngine;

public class FishingSystem : MonoBehaviour
{
    public Item fishItem;

    private void Start()
    {
        fishItem = new Item("Joel", 10);
    }

    public void CatchFish()
    {
        InventoryManager.inventoryManager.AddFish(fishItem);
        Debug.Log("Caught a fish!");
    }
}