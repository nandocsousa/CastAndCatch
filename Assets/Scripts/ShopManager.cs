using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int playerCoins = 0;

    public void SellFish(Item item)
    {
        if (InventoryManager.inventoryManager.inventory.Contains(item))
        {
            InventoryManager.inventoryManager.RemoveFish(item);
            playerCoins += item.sellPrice;

            Debug.Log($"Sold {item.itemName} for {item.sellPrice} coins. Current coins: {playerCoins}");
        }
        else Debug.Log("Fish not found in inventory.");
    }
}