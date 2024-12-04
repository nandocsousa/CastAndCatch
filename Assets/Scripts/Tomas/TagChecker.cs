using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class TagChecker : MonoBehaviour
{
	private const string OverpassUrl = "https://overpass-api.de/api/interpreter";

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			double testLatitude = 41.146285;
			double testLongitude = -8.626496;

			CheckPointTags(testLatitude, testLongitude);
		}
	}

	public void CheckPointTags(double latitude, double longitude)
	{
		StartCoroutine(FetchTags(latitude, longitude));
	}

	private IEnumerator FetchTags(double latitude, double longitude)
	{
		// Corrected query with a larger radius
		string query = $"[out:json];node(around:50,{latitude},{longitude})[\"natural\"=\"water\"];out;";
		string encodedQuery = UnityWebRequest.EscapeURL(query);
		string url = $"{OverpassUrl}?data={encodedQuery}";

		using (UnityWebRequest request = UnityWebRequest.Get(url))
		{
			Debug.Log("Sending request to Overpass API...");
			yield return request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.Success)
			{
				Debug.Log("Response received!");
				ParseResponse(request.downloadHandler.text);
			}
			else
			{
				Debug.LogError($"Error fetching data: {request.error}");
			}
		}
	}

	private void ParseResponse(string jsonResponse)
	{
		JObject response = JObject.Parse(jsonResponse);
		bool isInWater = false;

		foreach (var element in response["elements"])
		{
			if (element["tags"]?["natural"]?.ToString() == "water")
			{
				isInWater = true;
				break;
			}
		}

		Debug.Log($"Point is in water: {isInWater}");
	}
}
