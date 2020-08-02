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

    public static GameObject Canvas { get; set; }
    public static GameObject MainGame { get; set; }
    public static GameObject MainCamera { get; set; }
    public static GameObject UIConstractor { get; set; }
    public static GameObject LevelInfoInText { get; set; }
    public static GameObject CanvasContent { get; set; }

    void Start()
    {
        Canvas = GameObject.Find("EditorCanvas");
        MainGame = GameObject.Find("MainGame");
        MainCamera = GameObject.Find("Main Camera");
        UIConstractor = GameObject.Find("ConstractorUI");
        LevelInfoInText = GameObject.Find("LevelInfoInText");
        CanvasContent = GameObject.Find("Content");

        Canvas.SetActive(true);
        MainGame.SetActive(false);

        InstantiateLevelField();
    }

    private void InstantiateLevelField()
    {
        var ContentTransform = CanvasContent.transform;
        var canvasRectTransform = ContentTransform.GetComponent<RectTransform>();
        var canvasWidth = canvasRectTransform.rect.width;
        var canvasHight = canvasRectTransform.rect.height;
        
        var wallBlockRectTransform = TileButton.GetComponent<RectTransform>();
        var blockWidth = wallBlockRectTransform.rect.width;
        var blockHight = wallBlockRectTransform.rect.height;

        var i = 0;
        var j = 0;

        var LevelStructureList = new List<List<LevelInfo>>();
        for (float x = 50; x <= canvasWidth-50; x = x + blockWidth)
        {
            j = 0;
            var verticalList = new List<LevelInfo>();
            for (float y = 50; y <= canvasHight-50; y = y + blockHight)
            {
                var gameObject = Instantiate(TileButton, ContentTransform);
                var RectTransform = gameObject.GetComponent<RectTransform>();
                RectTransform.anchoredPosition = new Vector2(x, y);

                var levelInfo = gameObject.GetComponent<LevelInfo>();
                levelInfo.x = i;
                levelInfo.y = j;
                levelInfo.TileType = TileType.Empty;
                verticalList.Add(levelInfo);

                var image = gameObject.GetComponent<Image>();
                image.color = new Color(0, 0, 0);

                j++;
            }
            i++;
            LevelStructureList.Add(verticalList);
        }

        LevelStructure = new LevelInfo[LevelStructureList.Count, LevelStructureList[0].Count];
        i = 0;
        foreach (var row in LevelStructureList)
        {
            j = 0;
            foreach (var column in row)
            {
                LevelStructure[i, j] = column;
                j++;
            }
            i++;
        }
    }
}
