using UnityEngine;

[CreateAssetMenu(fileName = "New Fishing Rod", menuName = "Fishing Rod")]
public class FishingRodItem : ScriptableObject
{
	public GameObject FishingRodPrefab;
	public string FishingRodName;
	public int GoldCost;
	public Sprite ItemSprite;
}
