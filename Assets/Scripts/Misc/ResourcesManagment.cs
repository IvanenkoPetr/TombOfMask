using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class ResourcesManagment
{
    public static LevelInfo[,] LoadLevelStructure(string levelName)
    {
        var levelStructureInText = Resources.Load<TextAsset>(levelName).text;
        var result = DeserializeLevelStructure(levelStructureInText);
        return result;
    }

    public static List<List<LevelInfoDto>> LoadLevelStructureInDto(string levelName)
    {
        var levelStructureInText = Resources.Load<TextAsset>(levelName).text;
        var result = DeserializeLevelStructureInDto(levelStructureInText);
        return result;
    }

    public static List<List<List<LevelInfoDto>>> LoadLevelStructureFromFolderInDto(string folderNamme)
    {
        var result = new List<List<List<LevelInfoDto>>>();
        var levelStructuresInText = Resources.LoadAll<TextAsset>(folderNamme);
        foreach(var levelPart in levelStructuresInText)
        {
            var levelPartDto = DeserializeLevelStructureInDto(levelPart.text);
            result.Add(levelPartDto);
        }

        return result;
    }

    private static LevelInfo[,] DeserializeLevelStructure(string levelStractureInText)
    {
        var serializer = new XmlSerializer(typeof(List<List<LevelInfoDto>>));

        using (TextReader reader = new StringReader(levelStractureInText))
        {
            var levelStructureFromText = (List<List<LevelInfoDto>>)serializer.Deserialize(reader);

            var levelStructure = new LevelInfo[levelStructureFromText.Count, levelStructureFromText[0].Count];
            for (var i = 0; i < levelStructureFromText.Count; i++)
            {
                for (var j = 0; j < levelStructureFromText[0].Count; j++)
                {
                    var levelElement = GameObject.Instantiate(EditorPrefabsSettings.Prefabs.TileButton).GetComponent<LevelInfo>();

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

                    levelStructure[i, j] = levelElement;
                    GameObject.Destroy(levelElement);
                }
            }

            return levelStructure;
        }
    }

    private static List<List<LevelInfoDto>> DeserializeLevelStructureInDto(string levelStractureInText)
    {
        var serializer = new XmlSerializer(typeof(List<List<LevelInfoDto>>));

        using (TextReader reader = new StringReader(levelStractureInText))
        {
            var levelStructureFromText = (List<List<LevelInfoDto>>)serializer.Deserialize(reader);
            return levelStructureFromText;

        }

    }
}

