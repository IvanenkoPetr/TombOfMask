using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Player;
    public GameObject Enemy;
    public GameObject Collectible;
    public GameObject Hatch;

    private void Update()
    {

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

    public void OnSetLevelSizeButtonClick()
    {
        var newWidth = int.Parse(GameObject.Find("FieldWidthInputField").GetComponent<InputField>().text);
        var newHeight = int.Parse(GameObject.Find("FieldHeightInputField").GetComponent<InputField>().text);

        var children = new List<GameObject>();
        foreach (Transform child in ConstractorUI.MainLayerOnCanvas.transform)
        {
            children.Add(child.gameObject);
        };
        children.ForEach(child => Destroy(child));

        ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(newWidth, newHeight);

        ConstractorUI.EditorCanvas.SetActive(true);
        ConstractorUI.MainGame.SetActive(false);
        ConstractorUI.LevelElementsCanvas.SetActive(false);
    }

    public void OnGenerateLevelButtonClick()
    {
        var mainGameObject = ConstractorUI.MainGame.transform;
        var constractorObject = GameObject.Find("ConstractorUI").GetComponent<ConstractorUI>();
        var levelStructure = constractorObject.LevelStructure;

        for (var i = 0; i < levelStructure.GetLength(0); i++)
        {
            for (var j = 0; j < levelStructure.GetLength(1); j++)
            {
                var tileInfo = levelStructure[i, j];
                var tileType = tileInfo.TileType;
                switch (tileType)
                {
                    case TileType.Wall:

                        var gameObject = Instantiate(Wall, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        if (tileInfo.Options != null)
                        {
                            var spikeLayer = gameObject.transform.Find("SpikesLayer").gameObject;
                            spikeLayer.SetActive(true);
                            var spikesInfo = (Dictionary<SpikeType,bool>)tileInfo.Options;
                            foreach(var spike in spikesInfo)
                            {
                                spikeLayer.transform.Find(spike.Key.ToString()).gameObject.SetActive(spike.Value);
                            }
                        }

                        break;
                    case TileType.Player:
                        gameObject = Instantiate(Player, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        var movement = gameObject.GetComponent<PlayerMovement>();
                        movement.LevelStructure = levelStructure;

                        var mainCamera = ConstractorUI.MainCamera;
                        mainCamera.transform.SetParent(gameObject.transform);
                        mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.position.z);
                        break;
                    case TileType.Collectible:
                        gameObject = Instantiate(Collectible, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        break;
                    case TileType.Enemy:
                        gameObject = Instantiate(Enemy, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        var swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.LevelStructure = levelStructure;
                        swipeMovement.MovementAxis = MovementAxis.None;
                        break;
                    case TileType.HorizontalEnemy:
                        gameObject = Instantiate(Enemy, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.LevelStructure = levelStructure;
                        swipeMovement.MovementAxis = MovementAxis.Horizontal;
                        break;
                    case TileType.VerticalEnemy:
                        gameObject = Instantiate(Enemy, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.LevelStructure = levelStructure;
                        swipeMovement.MovementAxis = MovementAxis.Vertical;
                        break;
                    case TileType.RandomEnemy:
                        gameObject = Instantiate(Enemy, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.LevelStructure = levelStructure;
                        swipeMovement.MovementAxis = MovementAxis.Random;
                        break;
                    case TileType.Hatch:
                        gameObject = Instantiate(Hatch, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var hatchBehaviour = gameObject.GetComponent<Hatch>();
                        hatchBehaviour.ChangeStateInSeconds = 1.0f;
                        break;
                }
            }
        }

        var canvas = ConstractorUI.EditorCanvas;
        canvas.SetActive(false);

        var mainGame = ConstractorUI.MainGame;
        mainGame.SetActive(true);

        var gameMenu = ConstractorUI.GameMenu;
        gameMenu.SetActive(true);

    }

    public void OnLoadLevelFromText()
    {
        var constractorObject = GameObject.Find("ConstractorUI").GetComponent<ConstractorUI>();

        var levelStructureInText = Resources.Load<TextAsset>("LevelStructure").text;
        DeserializeLevelStructure(levelStructureInText);

        var levelStructure = constractorObject.LevelStructure;
        var allTiles = ConstractorUI.MainLayerOnCanvas.transform;
        foreach (Transform tile in allTiles)
        {
            var tileLevelInfo = tile.gameObject.GetComponent<LevelInfo>();
            TileOnCanvas.SetTileColour(tile.gameObject, levelStructure[tileLevelInfo.x, tileLevelInfo.y].TileType);
        }

    }

    public void OnSaveLevelIntoText()
    {
        var constractorObject = GameObject.Find("ConstractorUI").GetComponent<ConstractorUI>();
        var levelStructure = constractorObject.LevelStructure;
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
            var levelStructure = ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().LevelStructure;
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
