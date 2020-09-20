using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplaySettings
{
    private static GameObject soundController;
    public static GameObject settings;
    public static GameObject PointsText { get; set; }
    public static GameObject StarsText { get; set; }

    public static GameObject MainCamera;
    
    public static GameObject SoundController { 
        get 
        { 
            if(soundController == null)
            {
                soundController = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.SoundController);                
            };
            return soundController;
        } 
    }

    public static GameObject Settings
    {
        get
        {
            if (settings == null)
            {
                settings = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Settings);
            };

            return settings;
        }
    }
}
