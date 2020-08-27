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
    [SerializeField]
    public GameObject SpikesButton;
    public LevelInfo[,] LevelStructure;    
    public TileType CurrentTileType;
    public SpikeType CurrentSpikeType;
    public bool IsCurrentTypeTile;

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

    [SerializeField]
    public Sprite LeftSpikeSprite;
    [SerializeField]
    public Sprite RightSpikeSprite;
    [SerializeField]
    public Sprite TopSpikeSprite;
    [SerializeField]
    public Sprite BottomSpikeSprite;

    public static GameObject EditorCanvas { get; set; }
    public static GameObject MainGame { get; set; }
    public static GameObject GameMenu{ get; set; }
    public static GameObject MainCamera { get; set; }
    public static GameObject UIConstractor { get; set; }
    public static GameObject MainLayerOnCanvas { get; set; }
    public static GameObject SpikeLayerOnCanvas { get; set; }
    public static GameObject LevelElementsCanvas { get; set; }
    public static GameObject OpenElementsCanvasButton { get; set; }

    public static Dictionary<TileType, Sprite> AccordanceTileTypeAndSprite { get; set; }
    public static Dictionary<SpikeType, Sprite> AccordanceSpikeTypeAndSprite { get; set; }

    void Start()
    {
        EditorCanvas = GameObject.Find("MainMenu");
        MainGame = GameObject.Find("MainGame");
        GameMenu = GameObject.Find("GameMenu");
        MainCamera = GameObject.Find("MainCamera");
        UIConstractor = GameObject.Find("ConstractorUI");
        MainLayerOnCanvas = GameObject.Find("MainLayer");
        SpikeLayerOnCanvas = GameObject.Find("SpikesLayer");
        LevelElementsCanvas = GameObject.Find("LevelElementsMenu");
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

        AccordanceSpikeTypeAndSprite = new Dictionary<SpikeType, Sprite>
        {
            [SpikeType.Bottom] = BottomSpikeSprite,
            [SpikeType.Top] = TopSpikeSprite,
            [SpikeType.Right] = RightSpikeSprite,
            [SpikeType.Left] = LeftSpikeSprite
        };

        EditorCanvas.SetActive(true);
        MainGame.SetActive(false);
        LevelElementsCanvas.SetActive(false);
        GameMenu.SetActive(false);

        InstantiateLevelField();
    }

    public void InstantiateLevelField(int widthInBlocks, int heightInBlocks)
    {
        var wallBlockRectTransform = TileButton.GetComponent<RectTransform>();
        var blockWidth = wallBlockRectTransform.rect.width;
        var blockHight = wallBlockRectTransform.rect.height;

        var ContentTransform = MainLayerOnCanvas.transform;
        var canvasRectTransform = ContentTransform.GetComponent<RectTransform>();
        canvasRectTransform.sizeDelta = new Vector2(widthInBlocks * blockWidth + 65, heightInBlocks * blockHight + 65);

        var SpikeTransform = SpikeLayerOnCanvas.transform;
        var canvasSpikeRectTransform = SpikeTransform.GetComponent<RectTransform>();
        canvasSpikeRectTransform.sizeDelta = new Vector2(widthInBlocks * blockWidth + 65, heightInBlocks * blockHight + 65);

        InstantiateLevelField();
    }

    private void InstantiateLevelField()
    {
        var ContentTransform = MainLayerOnCanvas.transform;
        var canvasRectTransform = ContentTransform.GetComponent<RectTransform>();
        var canvasWidth = canvasRectTransform.rect.width;
        var canvasHight = canvasRectTransform.rect.height;

        var parentOfContent = ContentTransform.parent.gameObject.GetComponent<RectTransform>();
        parentOfContent.sizeDelta = new Vector2(canvasRectTransform.rect.width, canvasRectTransform.rect.height);

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
