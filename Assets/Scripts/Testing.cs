using UnityEngine;

public class Testing : MonoBehaviour
{
    private FishingSystem fishingSystem;
    private ShopManager shopManager;

    private void Start()
    {
        fishingSystem = GameObject.FindWithTag("GameManager").GetComponent<FishingSystem>();
        shopManager = GameObject.FindWithTag("GameManager").GetComponent<ShopManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Sell key pressed");
            shopManager.SellFish(fishingSystem.fishItem);
        }
    }
}