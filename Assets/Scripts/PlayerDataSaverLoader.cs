using System.IO;
using UnityEngine;

public class PlayerDataSaverLoader {

    public static PlayerData PlayerData { get; set; }
    static string dataPath;

    public static void Start()
    {
        dataPath = Path.Combine(Application.persistentDataPath, "CharacterData.txt");
    }

    public static void SavePlayerData(PlayerData data)
    {
        PlayerPrefs.SetString("characterName", "Default");
        PlayerPrefs.SetFloat("deposit", data.deposit);
        PlayerPrefs.SetInt("quantity", data.quantity);
        PlayerPrefs.Save();
    }

    public static PlayerData LoadPlayerData()
    {
        PlayerData loadedCharacter = new PlayerData();
        loadedCharacter.deposit = PlayerPrefs.GetFloat("deposit");
        loadedCharacter.quantity = PlayerPrefs.GetInt("quantity");

        return loadedCharacter;
    }
}

