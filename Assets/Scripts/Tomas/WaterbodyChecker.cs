using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json.Linq;

public class WaterbodyChecker : MonoBehaviour
{
	private const string OverpassUrl = "https://overpass-api.de/api/interpreter";
	//portugal
	//private const string QueryPortoWaterBodies = "[out:json];(way[\"natural\"=\"water\"](38.0,-9.5,41.9,-6.2););out body;";
	//private const string QueryPortoWaterBodies = "[out:json];(way[\"natural\"=\"water\"](38.0,-9.5,41.9,-6.2);relation[\"natural\"=\"water\"](41.013,-8.881,41.367,-8.193););out body;";
	private const string QueryPortoWaterBodies = "[out:json];(way[\"natural\"=\"water\"](41.013,-8.881,41.367,-8.193););out body;";

	public List<Geometry> waterPolygons = new List<Geometry>();

	void Start()
	{
		StartCoroutine(FetchWaterBodies());
	}

	public bool IsPointInWater(double latitude, double longitude)
	{

		Point point = new Point(latitude, longitude); // Longitude first
		foreach (var polygon in waterPolygons)
		{
			Debug.Log($"Checking polygon with bounds: {polygon.Envelope}");
			if (polygon.Contains(point))
			{
				Debug.Log($"Point {point} is in polygon: {polygon}");
				return true;
			}
		}
		//foreach (var polygon in waterPolygons)
		//{

		//	if (polygon.Contains(point))
		//	{
		//		return true;
		//	}
		//}
		return false;
	}

	private IEnumerator FetchWaterBodies()
	{
		string url = OverpassUrl + "?data=" + UnityWebRequest.EscapeURL(QueryPortoWaterBodies);

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
		var geoJsonReader = new GeoJsonReader();
		var response = JObject.Parse(jsonResponse);

		Dictionary<long, Coordinate> nodeMap = new Dictionary<long, Coordinate>();

		// Extract nodes
		foreach (var element in response["elements"])
		{
			if (element["type"]?.ToString() == "node")
			{
				long nodeId = (long)element["id"];
				double lat = (double)element["lat"];
				double lon = (double)element["lon"];
				nodeMap[nodeId] = new Coordinate(lon, lat);
			}
		}

		// Extract polygons (ways)
		foreach (var element in response["elements"])
		{
			if (element["type"]?.ToString() == "way")
			{
				var coordinates = new List<Coordinate>();
				foreach (var nodeId in element["nodes"])
				{
					if (nodeMap.ContainsKey((long)nodeId))
					{
						coordinates.Add(nodeMap[(long)nodeId]);
					}
				}

				// Ensure polygon is closed
				if (coordinates.Count > 0 && coordinates[0] != coordinates[coordinates.Count - 1])
				{
					coordinates.Add(coordinates[0]);
				}

				// Add to water polygons
				var polygon = new Polygon(new LinearRing(coordinates.ToArray()));
				waterPolygons.Add(polygon);
			}
		}

		Debug.Log($"Parsed {waterPolygons.Count} water polygons.");
	}
}
