using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Globalization;

public class TagChecker : MonoBehaviour
{
    private const string OverpassUrl = "https://overpass-api.de/api/interpreter";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            double testLatitude = 41.144523;
            double testLongitude = -8.626097;

            CheckPointTags(testLatitude, testLongitude);
        }
    }

    public void CheckPointTags(double latitude, double longitude)
    {
        StartCoroutine(FetchTags(latitude, longitude));
    }

    private IEnumerator FetchTags(double latitude, double longitude)
    {
        string query = $@"[out:json];(node(around:50,{latitude},{longitude})[""natural""=""water""];way(around:50,{latitude},{longitude})[""natural""=""water""];relation(around:50,{latitude},{longitude})[""natural""=""water""];);out body;>;out skel qt;";
        string url = $"{OverpassUrl}?data={query}";
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
