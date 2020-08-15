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

    [SerializeField]
    public Sprite EmptyTileSprite;
    [SerializeField]
    public Sprite WallTileSprite;
    [SerializeField]
    public Sprite PlayerTileSprite;
    [SerializeField]
    public Sprite EnemyTileSprite;
    [SerializeField]
    public Sprite HorizontalEnemyTileSprite;
    [SerializeField]
    public Sprite VerticalEnemyTileSprite;
    [SerializeField]
    public Sprite RandomEnemyTileSprite;
    [SerializeField]
    public Sprite CollectibleTileSprite;
    [SerializeField]
    public Sprite HatchTileSprite;



    public static GameObject EditorCanvas { get; set; }
    public static GameObject MainGame { get; set; }
    public static GameObject MainCamera { get; set; }
    public static GameObject UIConstractor { get; set; }
    public static GameObject CanvasContent { get; set; }
    public static GameObject LevelElementsCanvas { get; set; }
    public static GameObject OpenElementsCanvasButton { get; set; }

    public static Dictionary<TileType, Sprite> AccordanceTileTypeAndSprite { get; set; }

    void Start()
    {
        EditorCanvas = GameObject.Find("EditorCanvas");
        MainGame = GameObject.Find("MainGame");
        MainCamera = GameObject.Find("Main Camera");
        UIConstractor = GameObject.Find("ConstractorUI");
        CanvasContent = GameObject.Find("Content");
        LevelElementsCanvas = GameObject.Find("LevelElementsCanvas");
        OpenElementsCanvasButton = GameObject.Find("OpenElementsCanvasButton");

        AccordanceTileTypeAndSprite = new Dictionary<TileType, Sprite>
        {
            [TileType.Collectible] = CollectibleTileSprite,
            [TileType.Empty] = EmptyTileSprite,
            [TileType.Enemy] = EnemyTileSprite,
            [TileType.HorizontalEnemy] = HorizontalEnemyTileSprite,
            [TileType.VerticalEnemy] = VerticalEnemyTileSprite,
            [TileType.RandomEnemy] = RandomEnemyTileSprite,
            [TileType.Hatch] = HatchTileSprite,
            [TileType.Player] = PlayerTileSprite,
            [TileType.Wall] = WallTileSprite,
        };

        EditorCanvas.SetActive(true);
        MainGame.SetActive(false);
        LevelElementsCanvas.SetActive(false);

        InstantiateLevelField();
    }

    public void InstantiateLevelField(int widthInBlocks, int heightInBlocks)
    {
        var wallBlockRectTransform = TileButton.GetComponent<RectTransform>();
        var blockWidth = wallBlockRectTransform.rect.width;
        var blockHight = wallBlockRectTransform.rect.height;

        var ContentTransform = CanvasContent.transform;
        var canvasRectTransform = ContentTransform.GetComponent<RectTransform>();
        canvasRectTransform.sizeDelta = new Vector2(widthInBlocks * blockWidth + 65, heightInBlocks * blockHight + 65);
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
                image.sprite = EmptyTileSprite;
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
