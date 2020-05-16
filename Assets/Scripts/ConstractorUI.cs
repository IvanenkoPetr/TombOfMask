using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConstractorUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject TileButton;
    public LevelInfo[,] LevelStructure;    
    public TileType CurrentTileType;

    void Start()
    {
        InstantiateLevelField();
    }

    public void OnButtonClick()
    {
        var tileObject = EventSystem.current.currentSelectedGameObject;

        var tileInfo = tileObject.GetComponent<LevelInfo>();
        
        var constractorObject = GameObject.Find("ConstractorUI").GetComponent<ConstractorUI>();
        var tile = constractorObject.LevelStructure[tileInfo.x, tileInfo.y];
        tile.TileType = constractorObject.CurrentTileType;

        var image = tileObject.GetComponent<Image>();
        
        switch (constractorObject.CurrentTileType)
        {
            case TileType.Empty:
                image.color = new Color(0, 0, 0);
                break;
            case TileType.Wall:
                image.color = new Color(255, 255, 255);
                break;
            case TileType.Player:
                image.color = new Color(255, 255, 0);
                break;

        }
    }

    private void InstantiateLevelField()
    {
        var Canvas = GameObject.Find("Canvas");
        var ContentTransform = Canvas.transform.Find("Scroll View/Content");
        var canvasRectTransform = ContentTransform.GetComponent<RectTransform>();
        var canvasWidth = canvasRectTransform.rect.width;
        var canvasHight = canvasRectTransform.rect.height;
        
        var wallBlockRectTransform = TileButton.GetComponent<RectTransform>();
        var blockWidth = wallBlockRectTransform.rect.width;
        var blockHight = wallBlockRectTransform.rect.height;

        var i = 0;
        var j = 0;

        LevelStructure = new LevelInfo[(int)((canvasWidth / blockWidth)-1), (int)((canvasHight / blockHight)-1)];
        //for (var x = -canvasWidth / 2 + blockWidth; x < canvasWidth / 2 - blockWidth; x = x + blockWidth)
        for (float x = 50; x <= canvasWidth-50; x = x + blockWidth)
        {
            j = 0;
            //for (var y = -canvasHight / 2 + blockHight; y < canvasHight / 2 - blockHight; y = y + blockHight)
            for (float y = 50; y <= canvasHight-50; y = y + blockHight)
            {
                var gameObject = Instantiate(TileButton, ContentTransform);
                var RectTransform = gameObject.GetComponent<RectTransform>();
                RectTransform.anchoredPosition = new Vector2(x, y);

                var levelInfo = gameObject.GetComponent<LevelInfo>();
                levelInfo.x = i;
                levelInfo.y = j;
                levelInfo.TileType = TileType.Empty;
                LevelStructure[i, j] = levelInfo;

                var image = gameObject.GetComponent<Image>();
                image.color = new Color(0, 0, 0);

                j++;
            }
            i++;
        }
    }
}
