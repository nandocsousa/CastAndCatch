using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveLoad : MonoBehaviour
{
    private const string SaveFileName = "playerSaveData.json"; // Save file name

    // Class to hold player data
    [System.Serializable]
    public class SaveData
    {
        public int playerCoins;
    }

    public void SavePlayerData(int coins) // Save the player's data
    {
        SaveData data = new SaveData();
        data.playerCoins = coins;

        string json = JsonConvert.SerializeObject(data, Formatting.Indented); // Serialize the data to a JSON string
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName); // Save to a persistent path
        File.WriteAllText(filePath, json); // Write the JSON string to the file

        Debug.Log($"Player data saved to {filePath}");
    }

    public int LoadPlayerData() // Load the player's data
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName); // Path to the save file

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath); // Read the JSON string from the file
            SaveData data = JsonConvert.DeserializeObject<SaveData>(json); // Deserialize the JSON into a SaveData object

            Debug.Log($"Player data loaded from {filePath}");
            return data.playerCoins; // Return the player's coin count
        }
        else
        {
            Debug.Log("No save file found. Returning default value.");
            return 0; // Return 0 if no file exists
        }
    }
}