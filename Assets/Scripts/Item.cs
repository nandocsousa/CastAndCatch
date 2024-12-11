[System.Serializable]
public class Item
{
    public string itemName;
    public int sellPrice;
    public string quality;

    public Item(string name, int price, string fishQuality)
    {
        itemName = name;
        sellPrice = price;
        quality = fishQuality;
    }
}