[System.Serializable]

public class Item
{
    public string itemName;
    public int sellPrice;

    public Item(string name, int price)
    {
        itemName = name;
        sellPrice = price;
    }
}