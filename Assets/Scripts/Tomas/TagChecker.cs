using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Globalization;
using TMPro; //for testing, remove after
using System;

public class TagChecker : MonoBehaviour
{
    public static event Action<bool> E_LureLandedOnWater;

    private const string OverpassUrl = "https://overpass-api.de/api/interpreter";
    //public double testLatitude = 41.146246;
    //public double testLongitude = -8.626450;
    //public TextMeshProUGUI isonwatertext; //for testing, remove after
    //public TextMeshProUGUI landedonwatertext;
    private bool iswater = false; //for testing, remove after
    private double latitude;
    private double longitude;
    private bool landedInWater = false;

	private void OnEnable()
	{
        GameManager.E_SendLureLandCoords += HandleLureLandCoords;

        FishingRodController.E_LureLanded += CheckPointTags;
	}

	private void OnDisable()
	{
		GameManager.E_SendLureLandCoords -= HandleLureLandCoords;

		FishingRodController.E_LureLanded -= CheckPointTags;
	}

	private void Update()
    {
        if (iswater)
        {
			//landedonwatertext.text = "LANDED WATER: TRUE";
        }
        else
        {
			//landedonwatertext.text = "LANDED WATER: FALSE";
        }
    }

    public void CheatTesting()
    {
        E_LureLandedOnWater?.Invoke(true);
    }

    public void TryCheckPointTags()
    {
        //CheckPointTags(GameManager.Instance.GetPlayerLatitude(), GameManager.Instance.GetPlayerLongitude());
		CheckPointTags(41.1430675063837, -8.65617706993687);
	}

    public void CheckPointTags(double latitude, double longitude)
    {
        StartCoroutine(FetchTags(latitude, longitude));
    }

    private IEnumerator FetchTags(double latitude, double longitude)
    {
        string query = $@"[out:json];(node(around:100,{latitude},{longitude})[""natural""=""water""];way(around:100,{latitude},{longitude})[""natural""=""water""];relation(around:100,{latitude},{longitude})[""natural""=""water""];);out body;>;out skel qt;";
		//string url = $"{OverpassUrl}?data={query}";
		string url = $"{OverpassUrl}?data={UnityWebRequest.EscapeURL(query)}";
		Debug.Log($"Request URL: {url}");

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
		Debug.Log($"Raw Response: {jsonResponse}"); // Log the full JSON response for debugging

		JObject response = JObject.Parse(jsonResponse);
		bool isInWater = false;

		foreach (var element in response["elements"])
		{
			Debug.Log($"Element: {element}"); // Log each element to check its contents

			if (element["tags"]?["natural"]?.ToString() == "water")
			{
				isInWater = true;
				iswater = true;
				break;
			}
		}

		E_LureLandedOnWater?.Invoke(isInWater);
		Debug.Log($"Point is in water: {isInWater}");
	}


	private void HandleLureLandCoords(double lureLat, double lureLon)
    {
        CheckPointTags(lureLat, lureLon);
    }
}
