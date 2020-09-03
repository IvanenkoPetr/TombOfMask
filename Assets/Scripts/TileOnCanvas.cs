using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileOnCanvas : MonoBehaviour
{
    public GameObject SpikesTile { get; set; }

    // Start is called before the first frame update
    public void OnButtonClick()
    {
        var tileObject = EventSystem.current.currentSelectedGameObject;

        SetTileInfo(tileObject);
    }

    public static void SetTileInfo(GameObject tileObject)
    {
        if (ConstractorUI.TilesPressedInTouch.Any(a => a==tileObject))
        {
            return;
        }

        var tileInfo = tileObject.GetComponent<LevelInfo>();
        var currentRectTransform = tileObject.GetComponent<RectTransform>();

        var constractorObject =ConstractorUI.UIConstractor.GetComponent<ConstractorUI>();
        var tile = constractorObject.LevelStructure[tileInfo.x, tileInfo.y];

        if (constractorObject.IsCurrentTypeTile)
        {
            tile.TileType = constractorObject.CurrentTileType;
            SetTileColour(tileObject, constractorObject.CurrentTileType);
            if(constractorObject.CurrentTileType == TileType.Wall)
            {
                var spikes = new Dictionary<SpikeType, bool>()
                {
                    [SpikeType.Left] = false,
                    [SpikeType.Right] = false,
                    [SpikeType.Top] = false,
                    [SpikeType.Bottom] = false,
                };

                tile.Options = spikes;

                var tileOnCanvas = tileObject.GetComponent<TileOnCanvas>();
                if(tileOnCanvas.SpikesTile != null)
                {
                    Destroy(tileOnCanvas.SpikesTile);
                }
            }
        }
        else
        {
            if(tile.TileType == TileType.Wall)
            {
                var spikes = (Dictionary<SpikeType, bool>)tile.Options;
                //spikes[constractorObject.CurrentSpikeType] = true;
                var newValue = !spikes[constractorObject.CurrentSpikeType];
                spikes[constractorObject.CurrentSpikeType] = newValue;

                var tileOnCanvas = tileObject.GetComponent<TileOnCanvas>();
                GameObject gameObject = null;

                if(tileOnCanvas.SpikesTile == null)
                {

                    gameObject = Instantiate(constractorObject.SpikesButton, ConstractorUI.SpikeLayerOnCanvas.transform);
                    var RectTransform = gameObject.GetComponent<RectTransform>();
                    RectTransform.anchoredPosition = currentRectTransform.anchoredPosition;
                    tileOnCanvas.SpikesTile = gameObject;
                }
                else
                {
                    gameObject = tileOnCanvas.SpikesTile;
                }

                gameObject.transform.Find(constractorObject.CurrentSpikeType.ToString()).gameObject.SetActive(newValue);
            }
            
        }
    }

    public static void SetTileColour(GameObject tileObject, TileType currentTileType)
    {
        var image = tileObject.GetComponent<Image>();
        image.sprite = ConstractorUI.AccordanceTileTypeAndSprite[currentTileType];
        
    }
}
