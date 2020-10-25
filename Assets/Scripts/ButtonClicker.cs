﻿using Edgar.GraphBasedGenerator.Grid2D;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour
{

    public void OnRandomLevelMenuButtonClick()
    {
        ConstractorUI.EditorCanvas.SetActive(false);
        ConstractorUI.MainGame.SetActive(false);
        ConstractorUI.LevelGenerationMenu.SetActive(true);
    }

    public void OnGenerateRandomLevelButtonClick()
    {
        var Xoffset = 50;
        var Yoffset = 50;
        DestroyCanvasObjects();

        ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(100, 100);
        var levelStructure = Globals.LevelStructure;
        var numberOfRooms = int.Parse(GameObject.Find("NumberOfRoomsInputField").GetComponent<InputField>().text);
        var roomsWight = int.Parse(GameObject.Find("RoomWidthInputField").GetComponent<InputField>().text);
        var roomsHeight = int.Parse(GameObject.Find("RoomHeightInputField").GetComponent<InputField>().text);

        var layout = GenerateLevel.Generate(numberOfRooms, roomsWight, roomsHeight);

        GenerateRooms(Xoffset, Yoffset, levelStructure, layout);
        GenerateDoors(Xoffset, Yoffset, levelStructure, layout);
        GenerateItems(Xoffset, Yoffset, levelStructure, layout);

        DrawLevel(Xoffset, Yoffset, levelStructure);

        ConstractorUI.EditorCanvas.SetActive(true);
        ConstractorUI.MainGame.SetActive(true);
        ConstractorUI.LevelGenerationMenu.SetActive(false);
    }

    private void GenerateItems(int xoffset, int yoffset, LevelInfo[,] levelStructure, LayoutGrid2D<int> layout)
    {
        var firstRoom = layout.Rooms[0];
        var lastRoom = layout.Rooms[layout.Rooms.Count - 1];

        var indent = firstRoom.Transformation == Edgar.Geometry.TransformationGrid2D.Identity ? 1 : -1;
        var startPoint = levelStructure[xoffset + firstRoom.Position.X + 1, yoffset + firstRoom.Position.Y + indent];
        startPoint.TileType = TileType.Player;

        indent = lastRoom.Transformation == Edgar.Geometry.TransformationGrid2D.Identity ? 1 : -1;
        var endPoint = levelStructure[xoffset + lastRoom.Position.X + 1, yoffset + lastRoom.Position.Y + indent];
        endPoint.TileType = TileType.Exit;

        GenerateEnemies(xoffset, yoffset, levelStructure, layout, indent);

        System.Random rnd = new System.Random();
        var isHorizontal = rnd.Next(0, 1);
       // var isHorizontal = 0;
        foreach (var room in layout.Rooms)
        {
            if (room.IsCorridor)
            {
                continue;
            }

            var points = room.Outline.GetPoints();
            var maxX = points.Max(a => a.X);
            var minX = points.Min(a => a.X);
            var maxY = points.Max(a => a.Y);
            var minY = points.Min(a => a.Y);

            if (isHorizontal == 1)
            {
                var rowY = rnd.Next(minY, maxY);

                if (rowY == 0)
                {
                    if (room.Transformation == Edgar.Geometry.TransformationGrid2D.Identity)
                    {
                        rowY++;
                    }
                    else
                    {
                        rowY--;
                    }
                }
                else if (Math.Abs(rowY) == Math.Abs(maxY) || Math.Abs(rowY) == Math.Abs(minY))
                {
                    if (room.Transformation == Edgar.Geometry.TransformationGrid2D.Identity)
                    {
                        rowY--;
                    }
                    else
                    {
                        rowY++;
                    }
                }

                var X = 1;
                while (X <= maxX - 1)
                {
                    var tile = levelStructure[xoffset + room.Position.X + X, yoffset + room.Position.Y + rowY];
                    if (tile.TileType == TileType.Empty)
                    {
                        tile.TileType = TileType.Collectible;

                    }
                    X++;
                }
            }
            else
            {
                var rowX = rnd.Next(minX, maxX);

                rowX = rowX == 0 ? 1 : rowX;
                rowX = rowX == maxX ? maxX - 1 : rowX;

                var YStarPoint = minY;
                var YEndPoint = maxY;
                if (room.Transformation == Edgar.Geometry.TransformationGrid2D.Identity)
                {
                    YStarPoint++;
                    YEndPoint--;
                }
                else
                {
                    YStarPoint++;
                    YEndPoint--;
                };

                while(YStarPoint <= YEndPoint)
                {
                    var tile = levelStructure[xoffset + room.Position.X + rowX, yoffset + room.Position.Y + YStarPoint];
                    if (tile.TileType == TileType.Empty)
                    {
                        tile.TileType = TileType.Collectible;

                    }

                    YStarPoint++;
                }

            }
        }


    }

    private static int GenerateEnemies(int xoffset, int yoffset, LevelInfo[,] levelStructure, LayoutGrid2D<int> layout, int indent)
    {
        System.Random rnd = new System.Random();
        var enemyType = new[] { TileType.Enemy, TileType.HorizontalEnemy, TileType.VerticalEnemy, TileType.RandomEnemy };

        foreach (var room in layout.Rooms)
        {
            if (room.IsCorridor)
            {
                continue;
            }

            var numberOfEnemy = 1;
            var i = 1;
            while (i <= numberOfEnemy)
            {
                indent = room.Transformation == Edgar.Geometry.TransformationGrid2D.Identity ? 1 : -1;

                var points = room.Outline.GetPoints();
                var maxX = points.Max(a => a.X);
                var minX = points.Min(a => a.X);
                var maxY = points.Max(a => a.Y);
                var minY = points.Min(a => a.Y);


                var enemyX = rnd.Next(minX, maxX);
                var enemyY = rnd.Next(minY, maxY);

                enemyX = enemyX == 0 ? 1 : enemyX;
                enemyX = enemyX == maxX ? maxX - 1 : enemyX;

                if (enemyY == 0)
                {
                    if (room.Transformation == Edgar.Geometry.TransformationGrid2D.Identity)
                    {
                        enemyY++;
                    }
                    else
                    {
                        enemyY--;
                    }
                }
                else if (Math.Abs(enemyY) == Math.Abs(maxY) || Math.Abs(enemyY) == Math.Abs(minY))
                {
                    if (room.Transformation == Edgar.Geometry.TransformationGrid2D.Identity)
                    {
                        enemyY--;
                    }
                    else
                    {
                        enemyY++;
                    }
                }

                var tile = levelStructure[xoffset + room.Position.X + enemyX, yoffset + room.Position.Y + enemyY];
                if (tile.TileType == TileType.Empty)
                {
                    tile.TileType = enemyType[rnd.Next(0, 3)];
                    i++;
                }

            }
        }

        return indent;
    }

    private static void DrawLevel(int Xoffset, int Yoffset, LevelInfo[,] levelStructure)
    {
        var wallBlockRectTransform = EditorPrefabsSettings.Prefabs.TileButton.GetComponent<RectTransform>();
        var blockWidth = wallBlockRectTransform.rect.width;
        var blockHight = wallBlockRectTransform.rect.height;

        var scrollRectTransform = ConstractorUI.Content.GetComponent<RectTransform>();
        scrollRectTransform.anchoredPosition = new Vector2(-blockWidth * Xoffset, -blockHight * Yoffset);

        var allTiles = ConstractorUI.MainLayerOnCanvas.transform;
        foreach (Transform tile in allTiles)
        {
            var tileLevelInfo = tile.gameObject.GetComponent<LevelInfo>();
            TileOnCanvas.SetTileColour(tile.gameObject, levelStructure[tileLevelInfo.x, tileLevelInfo.y].TileType);
        }
    }

    private static void GenerateDoors(int Xoffset, int Yoffset, LevelInfo[,] levelStructure, LayoutGrid2D<int> layout)
    {
        foreach (var room in layout.Rooms)
        {
            if (!room.IsCorridor)
            {
                continue;
            }

            var roomPosition = new Vector2Int(Xoffset + room.Position.X, Yoffset + room.Position.Y);

            var roomLines = room.Outline.GetLines();
            var maxLength = roomLines.Max(a => a.Length);

            var secondWall = false;
            foreach (var line in roomLines)
            {
                if (line.Length == maxLength)
                {
                    continue;
                }

                var MinX = Mathf.Min(line.From.X, line.To.X);
                var MaxX = Mathf.Max(line.From.X, line.To.X);
                var MinY = Mathf.Min(line.From.Y, line.To.Y);
                var MaxY = Mathf.Max(line.From.Y, line.To.Y);

                while (MinX < MaxX - 1)
                {
                    var tile = levelStructure[roomPosition.x + MinX + 1, roomPosition.y + (secondWall ? -maxLength : 0)];
                    tile.TileType = TileType.Empty;
                    tile.Options = null;
                    MinX++;
                }

                while (MinY < MaxY - 1)
                {
                    var tile = levelStructure[roomPosition.x + (secondWall ? maxLength : 0), roomPosition.y + MinY + 1];
                    tile.TileType = TileType.Empty;
                    tile.Options = null;
                    MinY++;
                }

                secondWall = true;
            }

        }
    }

    private static void GenerateRooms(int Xoffset, int Yoffset, LevelInfo[,] levelStructure, LayoutGrid2D<int> layout)
    {
        foreach (var room in layout.Rooms)
        {

            var points = room.Outline.GetPoints();
            for (var i = 0; i <= 3; i++)
            {
                var currentPoint = points[i];
                var nextPoint = points[i];
                if (i == 3)
                {
                    nextPoint = points[0];
                }
                else
                {
                    nextPoint = points[i + 1];
                }

                if (currentPoint.X == nextPoint.X)
                {
                    var minY = Mathf.Min(currentPoint.Y, nextPoint.Y);
                    var maxY = Mathf.Max(currentPoint.Y, nextPoint.Y);
                    while (minY <= maxY)
                    {
                        var tile = levelStructure[Xoffset + room.Position.X + currentPoint.X, Yoffset + room.Position.Y + minY];
                        tile.TileType = TileType.Wall;
                        var spikes = new Dictionary<SpikeType, bool>()
                        {
                            [SpikeType.Left] = false,
                            [SpikeType.Right] = false,
                            [SpikeType.Top] = false,
                            [SpikeType.Bottom] = false,
                        };

                        tile.Options = spikes;
                        minY++;
                    }
                }

                if (currentPoint.Y == nextPoint.Y)
                {
                    var minX = Mathf.Min(currentPoint.X, nextPoint.X);
                    var maxX = Mathf.Max(currentPoint.X, nextPoint.X);
                    while (minX <= maxX)
                    {
                        var tile = levelStructure[Xoffset + minX + room.Position.X, Yoffset + currentPoint.Y + room.Position.Y];
                        tile.TileType = TileType.Wall;
                        var spikes = new Dictionary<SpikeType, bool>()
                        {
                            [SpikeType.Left] = false,
                            [SpikeType.Right] = false,
                            [SpikeType.Top] = false,
                            [SpikeType.Bottom] = false,
                        };
                        tile.Options = spikes;
                        minX++;
                    }
                }
            }
        }
    }

    public void OnTileButtonClick(string tileType)
    {
        var UIConstractor = ConstractorUI.UIConstractor;
        var constractorUIComponent = UIConstractor.GetComponent<ConstractorUI>();
        constractorUIComponent.CurrentTileType = (TileType)Enum.Parse(typeof(TileType), tileType, true);
        constractorUIComponent.IsCurrentTypeTile = true;

        var image = ConstractorUI.OpenElementsCanvasButton.GetComponent<Image>();
        image.sprite = ConstractorUI.AccordanceTileTypeAndSprite[constractorUIComponent.CurrentTileType];

        ConstractorUI.EditorCanvas.SetActive(true);
        ConstractorUI.MainGame.SetActive(false);
        ConstractorUI.LevelElementsCanvas.SetActive(false);
    }

    public void OnSpikeButtonClick(string spikeType)
    {
        var UIConstractor = ConstractorUI.UIConstractor;
        var constractorUIComponent = UIConstractor.GetComponent<ConstractorUI>();
        constractorUIComponent.CurrentSpikeType = (SpikeType)Enum.Parse(typeof(SpikeType), spikeType, true);
        constractorUIComponent.IsCurrentTypeTile = false;

        var image = ConstractorUI.OpenElementsCanvasButton.GetComponent<Image>();
        image.sprite = ConstractorUI.AccordanceSpikeTypeAndSprite[constractorUIComponent.CurrentSpikeType];

        ConstractorUI.EditorCanvas.SetActive(true);
        ConstractorUI.MainGame.SetActive(false);
        ConstractorUI.LevelElementsCanvas.SetActive(false);
    }

    public void OnOpenElementsCanvasButtonClick()
    {
        ConstractorUI.EditorCanvas.SetActive(false);
        ConstractorUI.MainGame.SetActive(false);
        ConstractorUI.LevelElementsCanvas.SetActive(true);
    }

    public void OnSlideModeInEditorButtonClick()
    {
        ConstractorUI.IsEditorInSlideMode = !ConstractorUI.IsEditorInSlideMode;
        ConstractorUI.EditorScrollView.GetComponent<ScrollRect>().enabled = !ConstractorUI.IsEditorInSlideMode;

        var button = GameObject.Find("SlideModeInEditorButton");
        if (ConstractorUI.IsEditorInSlideMode)
        {
            button.GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            button.GetComponent<Image>().color = Color.white;
        }
    }

    public void OnToMainMenuButtonClick()
    {
        SceneManager.LoadScene(Globals.MainMenuSceneName);
    }

    public void OnSetLevelSizeButtonClick()
    {
        var newWidth = int.Parse(GameObject.Find("FieldWidthInputField").GetComponent<InputField>().text);
        var newHeight = int.Parse(GameObject.Find("FieldHeightInputField").GetComponent<InputField>().text);

        DestroyCanvasObjects();

        ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(newWidth, newHeight);

        ConstractorUI.EditorCanvas.SetActive(true);
        ConstractorUI.MainGame.SetActive(false);
        ConstractorUI.LevelElementsCanvas.SetActive(false);
    }

    public void OnGenerateLevelButtonClick()
    {
        var mainGameObject = ConstractorUI.MainGame.transform;

        Globals.GenerateLevel(mainGameObject, GameplaySettings.MainCamera);

        var canvas = ConstractorUI.EditorCanvas;
        canvas.SetActive(false);

        var mainGame = ConstractorUI.MainGame;
        mainGame.SetActive(true);

        var gameMenu = ConstractorUI.GameMenu;
        gameMenu.SetActive(true);

    }

    public void OnLoadLevelFromText()
    {
        var levelStructureInText = Resources.Load<TextAsset>("LevelStructure").text;
        DeserializeLevelStructure(levelStructureInText);

        var levelStructure = Globals.LevelStructure;
        var allTiles = ConstractorUI.MainLayerOnCanvas.transform;
        foreach (Transform tile in allTiles)
        {
            var tileLevelInfo = tile.gameObject.GetComponent<LevelInfo>();
            var infoInLevelStructure = levelStructure[tileLevelInfo.x, tileLevelInfo.y];
            TileOnCanvas.SetTileColour(tile.gameObject, infoInLevelStructure.TileType);

            if (infoInLevelStructure.Options != null)
            {
                var currentRectTransform = tile.gameObject.GetComponent<RectTransform>();
                var tileOnCanvas = tile.gameObject.GetComponent<TileOnCanvas>();
                var constractorObject = ConstractorUI.UIConstractor.GetComponent<ConstractorUI>();

                var tileGameObject = Instantiate(constractorObject.SpikesButton, ConstractorUI.SpikeLayerOnCanvas.transform);
                var RectTransform = tileGameObject.GetComponent<RectTransform>();
                RectTransform.anchoredPosition = currentRectTransform.anchoredPosition;
                tileOnCanvas.SpikesTile = tileGameObject;

                if (infoInLevelStructure.Options is IEnumerable<KeyValuePair<SpikeType, bool>>)
                {
                    foreach (var spikeInfo in infoInLevelStructure.Options as IEnumerable<KeyValuePair<SpikeType, bool>>)
                    {
                        tileGameObject.transform.Find(spikeInfo.Key.ToString()).gameObject.SetActive(spikeInfo.Value);
                    }
                }
            }

        }

    }

    public void OnSaveLevelIntoText()
    {
        var constractorObject = ConstractorUI.UIConstractor.GetComponent<ConstractorUI>();
        var levelStructure = Globals.LevelStructure;
        var levelInText = SerializeLevelStructure(levelStructure);
        var folder = Application.persistentDataPath;
        folder = @"C:\temp";
        File.WriteAllText(string.Concat(folder, "/", "LevelStructure.txt"), levelInText);

    }

    public string SerializeLevelStructure(LevelInfo[,] levelStructure)
    {
        var formatter = new XmlSerializer(typeof(List<List<LevelInfoDto>>));

        var levelStructureDto = new List<List<LevelInfoDto>>();
        for (var i = 0; i < levelStructure.GetLength(0); i++)
        {
            var row = new List<LevelInfoDto>();
            for (var j = 0; j < levelStructure.GetLength(1); j++)
            {
                row.Add(levelStructure[i, j].ToDTOObject());
            }

            levelStructureDto.Add(row);
        }

        var stringBuilder = new StringBuilder();
        using (var stream = XmlWriter.Create(stringBuilder))
        {
            formatter.Serialize(stream, levelStructureDto);
            return stringBuilder.ToString();

        }
    }

    public void DeserializeLevelStructure(string levelStractureInText)
    {
        DestroyCanvasObjects();

        var serializer = new XmlSerializer(typeof(List<List<LevelInfoDto>>));

        using (TextReader reader = new StringReader(levelStractureInText))
        {
            var levelStructureFromText = (List<List<LevelInfoDto>>)serializer.Deserialize(reader);

            ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(levelStructureFromText.Count, levelStructureFromText[0].Count);
            var levelStructure = Globals.LevelStructure;
            for (var i = 0; i < levelStructureFromText.Count; i++)
            {
                for (var j = 0; j < levelStructureFromText[0].Count; j++)
                {
                    var levelElement = levelStructure[i, j];

                    levelElement.x = levelStructureFromText[i][j].x;
                    levelElement.y = levelStructureFromText[i][j].y;
                    levelElement.TileType = levelStructureFromText[i][j].TileType;

                    if (levelElement.TileType == TileType.Wall)
                    {
                        var spikesOnWall = new Dictionary<SpikeType, bool>();
                        foreach (var spikeDto in levelStructureFromText[i][j].SpikesInfo)
                        {
                            spikesOnWall.Add(spikeDto.SpikeType, spikeDto.IsSetted);
                        }

                        levelElement.Options = spikesOnWall;
                    }
                }
            }
        }
    }

    private static void DestroyCanvasObjects()
    {
        var children = new List<GameObject>();
        foreach (Transform child in ConstractorUI.MainLayerOnCanvas.transform)
        {
            children.Add(child.gameObject);
        };
        children.ForEach(child => Destroy(child));

        children = new List<GameObject>();
        foreach (Transform child in ConstractorUI.SpikeLayerOnCanvas.transform)
        {
            children.Add(child.gameObject);
        };
        children.ForEach(child => Destroy(child));
    }


}
