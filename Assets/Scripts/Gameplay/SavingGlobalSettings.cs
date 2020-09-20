using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SavingGlobalSettings
{
    private static SavingGlobalSettings settings;
    public static SavingGlobalSettings Settings
    {
        get
        {
            if (settings == null)
            {
                var savedSettings = LoadGameData();
                if (savedSettings != null)
                {
                    settings = savedSettings;
                }
                else
                {
                    settings = new SavingGlobalSettings()
                    {
                        RemoteSettings = new GamesRemoteSettings(),
                        GameSettings = new GameSettings()
                    };
                }
            }

            return settings;
        }
    }

    public GamesRemoteSettings RemoteSettings;
    public GameSettings GameSettings; 
    public GameProgress GameProgress;

    public static void SaveGameData()
    {
        try
        {
            using (FileStream file = File.Create(SaveDataPath))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, SavingGlobalSettings.Settings);
            }
        }
        catch (Exception e)
        {

        }
    }

    public static SavingGlobalSettings LoadGameData()
    {
        SavingGlobalSettings gameData = null;
        try
        {
            using (FileStream file = File.Open(SaveDataPath, FileMode.Open))
            {
                var bf = new BinaryFormatter();
                gameData = (SavingGlobalSettings)bf.Deserialize(file);
                if(gameData.GameProgress == null)
                {
                    gameData.GameProgress = new GameProgress();
                }
                if (gameData.GameSettings == null)
                {
                    gameData.GameSettings = new GameSettings();
                }
                if (gameData.RemoteSettings == null)
                {
                    gameData.RemoteSettings = new GamesRemoteSettings();
                }
            }
        }
        catch (Exception e)
        {

        }
        return gameData;
    }

    private static string SaveDataPath
    {
        get
        {
            var saveDataPath = Path.Combine(Application.persistentDataPath, "atsd.dat");
            return saveDataPath;
        }
    }
}

[Serializable]
public class GamesRemoteSettings
{
    public float GameSpeed = 1f;
    public float PlayerSpeed = 1f;
    public float EnemySpeed = 1f;
    public float TimeToSwitchHatchState = 1f;
    public float MainCameraSize = 1f;
    public bool IsCanChangeDirectionInMovement = false;
}

[Serializable]
public class GameSettings
{
    public float SoundVolume = 0.5f;
}

[Serializable]
public class GameProgress
{
    public int Points;
    public int Stars;
}
