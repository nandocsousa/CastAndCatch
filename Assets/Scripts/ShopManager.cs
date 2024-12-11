using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public int playerCoins = 0;
    public TextMeshProUGUI coinsText;

    public FishingSystem fishingSystem;

    private SaveLoad saveLoad;

    private void Start()
    {
        saveLoad = FindObjectOfType<SaveLoad>(); // Find the SaveLoad script
        playerCoins = saveLoad.LoadPlayerData(); // Load the player's saved coins
        UpdateCoinsUI();
    }

    public void SellFish(string fishQuality) // Sell a specific type of fish
    {
        // Find a fish of the specified quality in the inventory
        Item fishToSell = InventoryManager.inventoryManager.inventory.Find(f => f.quality == fishQuality);

        if (fishToSell != null)
        {
            InventoryManager.inventoryManager.RemoveFish(fishToSell); // Remove the fish from the inventory
            fishingSystem.UpdateFishQualityCount(fishQuality, -1); // Updates counts and UI
            playerCoins += fishToSell.sellPrice; // Add the fish's price to the player's coins
            UpdateCoinsUI(); // Update the UI to the new coin count

            saveLoad.SavePlayerData(playerCoins); // Save the updated coins

            Debug.Log($"Sold a {fishQuality} fish for {fishToSell.sellPrice} coins. Current coins: {playerCoins}");
        }
        else Debug.Log($"No {fishQuality} fish available to sell.");
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