using UnityEngine;

public class PointInWaterTester : MonoBehaviour
{
	[SerializeField] private WaterbodyChecker waterbodyChecker;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) // Example trigger
		{
			double testLatitude = 41.140544;  // Replace with your test latitude
			double testLongitude = -8.608051; // Replace with your test longitude

			bool isInWater = waterbodyChecker.IsPointInWater(testLatitude, testLongitude);
			Debug.Log($"Point ({testLatitude}, {testLongitude}) is in water: {isInWater}");
		}
	}
}
