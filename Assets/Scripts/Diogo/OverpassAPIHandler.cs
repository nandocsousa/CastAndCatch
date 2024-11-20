using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

public class OverpassAPIHandler : MonoBehaviour
{
    private const string OverpassUrl = "https://overpass-api.de/api/interpreter";

    // Retrieves water bodies("natural"="water") within Portugal's bounding box
    private const string QueryPortugalWaterBodies = "[out:json];(way[\"natural\"=\"water\"](38.0,-9.5,41.9,-6.2););out body;";

    void Start()
    {
        StartCoroutine(FetchWaterBodies());
    }

    private IEnumerator FetchWaterBodies()
    {
        // Encode the query for the URL
        string url = OverpassUrl + "?data=" + UnityWebRequest.EscapeURL(QueryPortugalWaterBodies);

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

        // Extract coordinates of water bodies
        foreach (var element in response["elements"])
        {
            if (element["type"]?.ToString() == "way")
            {
                Debug.Log("Water Body Found:");
                foreach (var node in element["nodes"])
                {
                    Debug.Log($"Node ID: {node}");
                }
            }
        }
    }
}
