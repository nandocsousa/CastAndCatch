using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int playerCoins = 0;
    public TextMeshProUGUI coinsText;

    public FishingSystem fishingSystem;

    private void Start()
    {
        UpdateCoinsUI();
    }

    public void SellFish(string fishQuality)
    {
        // Find a fish of the specified quality in the inventory
        Item fishToSell = InventoryManager.inventoryManager.inventory.Find(f => f.quality == fishQuality);

        if (fishToSell != null)
        {
            InventoryManager.inventoryManager.RemoveFish(fishToSell);
            fishingSystem.UpdateFishQualityCount(fishQuality, -1); // Updates counts and UI
            playerCoins += fishToSell.sellPrice;
            UpdateCoinsUI();

            Debug.Log($"Sold a {fishQuality} fish for {fishToSell.sellPrice} coins. Current coins: {playerCoins}");
        }
        else
        {
            Debug.Log($"No {fishQuality} fish available to sell.");
        }
    }

    public void SellBronzeFish()
    {
        SellFish("Bronze");
    }

    public void SellSilverFish()
    {
        SellFish("Silver");
    }

    public void SellGoldFish()
    {
        SellFish("Gold");
    }

    private void UpdateCoinsUI()
    {
        coinsText.text = $"{playerCoins} coins";
    }
}