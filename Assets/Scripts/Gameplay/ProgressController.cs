using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ProgressController
{
    public static void CollectebleCollectedHandler(GameObject collectable)
    {
        SavingGlobalSettings.Settings.GameProgress.Points++;
        if (!Globals.IsEditorScene)
        {
            PaintHUD();
        }

    }

    public static void StarsCollectedHandler(GameObject star)
    {
        SavingGlobalSettings.Settings.GameProgress.Stars++;
        if (!Globals.IsEditorScene)
        {
            PaintHUD();
        }

    }

    public static void ExitCollisionHandler(GameObject collectable)
    {
        SavingGlobalSettings.SaveGameData();
    }

    public static void PaintHUD()
    {
        var textComponent = GameplaySettings.StarsText.GetComponent<Text>();
        textComponent.text = $"Stars {SavingGlobalSettings.Settings.GameProgress.Stars}";

        textComponent = GameplaySettings.PointsText.GetComponent<Text>();
        textComponent.text = $"Points {SavingGlobalSettings.Settings.GameProgress.Points}";
    }
}
