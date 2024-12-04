using UnityEngine;

public class PointInWaterTester : MonoBehaviour
{
	[SerializeField] private TagChecker tagChecker;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) // Example trigger
		{
			double testLatitude = 41.146285;  // Replace with your test latitude
			double testLongitude = -8.626496; // Replace with your test longitude

			tagChecker.CheckPointTags(testLatitude, testLongitude);
			//bool isInWater = tagChecker.IsPointInWater(testLatitude, testLongitude);
			//Debug.Log($"Point ({testLatitude}, {testLongitude}) is in water: {isInWater}");
		}
	}
}
