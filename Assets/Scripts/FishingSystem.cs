using UnityEngine;
using TMPro;

public class FishingSystem : MonoBehaviour
{
    public TextMeshProUGUI bronzeFishText;
    public TextMeshProUGUI silverFishText;
    public TextMeshProUGUI goldFishText;

    private int bronzeCount = 0;
    private int silverCount = 0;
    private int goldCount = 0;

    private void OnEnable()
    {
        FishManager.E_FishGranted += OnFishGranted;
    }

    private void OnDisable()
    {
        FishManager.E_FishGranted -= OnFishGranted;
    }

    private void Start()
    {
        UpdateFishCountUI();
    }

    private void OnFishGranted(string fishName, string fishQuality)
    {
        int sellPrice = GenerateSellPrice(fishQuality);
        Item fishItem = new Item(fishName, sellPrice, fishQuality);
        InventoryManager.inventoryManager.AddFish(fishItem);

        UpdateFishQualityCount(fishQuality, 1); // Update count when a fish is granted
        UpdateFishCountUI(); // Update UI

        Debug.Log($"Caught a {fishQuality} {fishName}! Added to inventory.");
    }

    private int GenerateSellPrice(string fishQuality)
    {
        return fishQuality switch
        {
            "Bronze" => 10,
            "Silver" => 20,
            "Gold" => 50,
            _ => 10
        };
    }

    public void UpdateFishQualityCount(string fishQuality, int amount)
    {
        switch (fishQuality)
        {
            case "Bronze":
                bronzeCount += amount;
                break;
            case "Silver":
                silverCount += amount;
                break;
            case "Gold":
                goldCount += amount;
                break;
        }
    }

    private void UpdateFishCountUI()
    {
        //Debug.Log($"Updating UI: Bronze: {bronzeCount}, Silver: {silverCount}, Gold: {goldCount}");
        bronzeFishText.text = bronzeCount.ToString();
        silverFishText.text = silverCount.ToString();
        goldFishText.text = goldCount.ToString();
    }

    // Temporary method to add a fish
    public void AddRandomFishForTesting()
    {
        string[] fishQualities = { "Bronze", "Silver", "Gold" };
        string randomQuality = fishQualities[Random.Range(0, fishQualities.Length)];
        string randomName = $"{randomQuality} Fish";

        int sellPrice = GenerateSellPrice(randomQuality);
        Item fishItem = new Item(randomName, sellPrice, randomQuality);
        InventoryManager.inventoryManager.AddFish(fishItem);

        UpdateFishQualityCount(randomQuality, 1);
        UpdateFishCountUI();

        //Debug.Log($"Test: Added a {randomQuality} fish!");
    }
}