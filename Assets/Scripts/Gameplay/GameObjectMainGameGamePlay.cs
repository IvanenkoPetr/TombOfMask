using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameObjectMainGameGamePlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var settings = GameplaySettings.Settings;
        var mainGame = GameObject.Find("MainGame");
        GameplaySettings.MainCamera = GameObject.Find("MainCamera");
        GameplaySettings.PointsText = GameObject.Find("PointsText");
        GameplaySettings.StarsText = GameObject.Find("StarsText"); 
        Globals.GenerateLevel(mainGame.transform, GameplaySettings.MainCamera);

        ProgressController.PaintHUD();
    }

}
