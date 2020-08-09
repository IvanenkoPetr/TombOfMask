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


    private void Update()
    {

    }

    public void OnTileButtonClick(string tileType)
    {
        var UIConstractor = ConstractorUI.UIConstractor;
        var constractorUIComponent = UIConstractor.GetComponent<ConstractorUI>();
        constractorUIComponent.CurrentTileType = (TileType)Enum.Parse(typeof(TileType), tileType, true);
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
                var tileType = levelStructure[i, j].TileType;
                switch (tileType)
                {
                    case TileType.Wall:
                        var gameObject = Instantiate(Wall, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
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
                        break;
                }
            }
        }

        var canvas = ConstractorUI.Canvas;
        canvas.SetActive(false);

        var mainGame = ConstractorUI.MainGame;
        mainGame.SetActive(true);
    
    }

    public void OnLoadLevelFromText()
    {
        var constractorObject = GameObject.Find("ConstractorUI").GetComponent<ConstractorUI>();
        var levelStructure = constractorObject.LevelStructure;

        var levelStructureInText = Resources.Load<TextAsset>("LevelStructure").text;
        DeserializeLevelStructure(levelStructureInText, levelStructure);

        var allTiles = ConstractorUI.CanvasContent.transform;
        foreach(Transform tile in allTiles)
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
        File.WriteAllText(string.Concat(folder, "/", "LevelStructureFile.txt"), levelInText);

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

        var q = new StringBuilder();
        using (var stream = XmlWriter.Create(q))
        {
            // сериализуем весь массив people
            formatter.Serialize(stream, levelStructureDto);
            return q.ToString();

        }
    }

    public void DeserializeLevelStructure(string levelStractureInText, LevelInfo[,] levelStructure)
    {

        var serializer = new XmlSerializer(typeof(List<List<LevelInfoDto>>));

        using (TextReader reader = new StringReader(levelStractureInText))
        {
            var levelStructureFromText = (List<List<LevelInfoDto>>)serializer.Deserialize(reader);
            for (var i = 0; i < levelStructureFromText.Count; i++)
            {
                for (var j = 0; j < levelStructureFromText[0].Count; j++)
                {
                    var levelElement = levelStructure[i, j];

                    levelElement.x = levelStructureFromText[i][j].x;
                    levelElement.y = levelStructureFromText[i][j].y;
                    levelElement.TileType = levelStructureFromText[i][j].TileType;     
                }
            }


        }

    }

}
