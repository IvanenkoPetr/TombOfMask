using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileOnCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnButtonClick()
    {
        var tileObject = EventSystem.current.currentSelectedGameObject;

        SetTileInfo(tileObject);
    }

    public static void SetTileInfo(GameObject tileObject)
    {
        var tileInfo = tileObject.GetComponent<LevelInfo>();

        var constractorObject = GameObject.Find("ConstractorUI").GetComponent<ConstractorUI>();
        var tile = constractorObject.LevelStructure[tileInfo.x, tileInfo.y];
        tile.TileType = constractorObject.CurrentTileType;
        SetTileColour(tileObject, constractorObject.CurrentTileType);
    }

    public static void SetTileColour(GameObject tileObject, TileType currentTileType)
    {
        var image = tileObject.GetComponent<Image>();

        switch (currentTileType)
        {
            case TileType.Empty:
                image.color = new Color(0, 0, 0);
                break;
            case TileType.Wall:
                image.color = new Color(255, 255, 255);
                break;
            case TileType.Player:
                image.color = new Color(0, 255, 0);
                break;
            case TileType.Enemy:
                image.color = new Color(255, 0, 0);
                break;
            case TileType.Collectible:
                image.color = new Color(255, 255, 0);
                break;
        }
    }
}
