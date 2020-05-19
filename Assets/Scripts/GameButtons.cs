using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameButtons : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnToEditorClick()
    {
        ConstractorUI.MainCamera.transform.SetParent(null);
        var mainGame = ConstractorUI.MainGame;
        foreach (Transform child in mainGame.transform)
        {
            if(child.GetComponent<Canvas>() == null)
            {
                Destroy(child.gameObject);
            }           
        }

        var canvas = ConstractorUI.Canvas; ;
        canvas.SetActive(true);

        mainGame.SetActive(false);

    }
}
