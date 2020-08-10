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
        var constructorUI = ConstractorUI.UIConstractor.GetComponent<ConstractorUI>();

        switch (currentTileType)
        {
            case TileType.Empty:
                //image.color = new Color(0, 0, 0);
                image.sprite = constructorUI.EmptyTileSprite;
                break;
            case TileType.Wall:
                image.sprite = constructorUI.WallTileSprite;
                break;
            case TileType.Player:
                image.sprite = constructorUI.PlayerTileSprite;
                break;
            case TileType.Enemy:
                image.sprite = constructorUI.EnemyTileSprite;
                break;
            case TileType.Collectible:
                image.sprite = constructorUI.CollectibleTileSprite;
                break;
            case TileType.Hatch:
                image.sprite = constructorUI.HatchTileSprite;
                break;
        }
    }
}
