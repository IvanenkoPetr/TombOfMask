using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnToEditorClick()
    {
        ClearGameScene();

    }

    public static void ClearGameScene()
    {
        GameplaySettings.MainCamera.transform.SetParent(null);
        var mainGame = ConstractorUI.MainGame;
        foreach (Transform child in mainGame.transform)
        {
            if (child.GetComponent<Canvas>() == null)
            {
                Destroy(child.gameObject);
            }
        }

        var canvas = ConstractorUI.EditorCanvas; ;
        canvas.SetActive(true);

        mainGame.SetActive(false);

        var gameMenu = ConstractorUI.GameMenu;
        gameMenu.SetActive(false);
    }
}
