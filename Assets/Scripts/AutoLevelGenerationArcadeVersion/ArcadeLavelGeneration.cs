using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ArcadeLavelGeneration
{
    public static void GenerateRandomLevel(int numberOfLevelParts)
    {

        var fullLevelStructure = new List<List<LevelTileInfo>>();
        var currentLevelPart = string.Empty;
        var isFirstPart = true;
        while (numberOfLevelParts > 0)
        {
            var isFinalPart = numberOfLevelParts == 1;
            var levelPart = GetNextLevelPart(currentLevelPart, isFirstPart, isFinalPart);
            currentLevelPart = levelPart.name;
            SetNextLevelPart(fullLevelStructure, levelPart.structure);
            numberOfLevelParts--;
            isFirstPart = false;
        }

        ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(fullLevelStructure[0].Count, fullLevelStructure.Count);
        var finalLevelStructure = Globals.LevelStructure;
        for (var i = 0; i < fullLevelStructure[0].Count; i++)
        {
            for (var j = 0; j < fullLevelStructure.Count; j++)
            {
                finalLevelStructure[i, j].TileType = fullLevelStructure[j][i].TileType;
                finalLevelStructure[i, j].Options = fullLevelStructure[j][i].Options;
            }
        }

        Globals.SetLevelStructure(finalLevelStructure);

        ButtonClicker.DrawLevelInEditor();
    }

    private static (List<List<LevelInfoDto>> structure, string name) GetNextLevelPart(string currentLevelPart, bool isFirstPart, bool isFinalPart)
    {
        string newLevelPartName;
        if (isFirstPart)
        {
            newLevelPartName = @"RandomLevelParts\LevelStartStructure";
        }
        else if(isFinalPart)
    {
            newLevelPartName = @"RandomLevelParts\LevelFinishtStructure";
        }
        else
        {

            var levelGraphText = Resources.Load<TextAsset>("RandomLevelParts/Graph").text;
            var levelGraphObject = JsonConvert.DeserializeObject<LevelGraph>(levelGraphText);

            var destinations = levelGraphObject.Graph.
                First(a => string.Equals(a.src, currentLevelPart, System.StringComparison.InvariantCultureIgnoreCase)).dst.OrderBy(a=> Random.value);

            newLevelPartName = destinations.First();
        }

        var newLevelPart = ResourcesManagment.LoadLevelStructureInDto(newLevelPartName);

        return (newLevelPart, newLevelPartName);
    }

    private static (List<List<LevelInfoDto>> structure, string name) GetNextLevelPart(int levelPart)
    {


        var newLevelPartName = $@"RandomLevelParts\RandomLevelPart{levelPart}";
        var newLevelPart = ResourcesManagment.LoadLevelStructureInDto(newLevelPartName);

        return (newLevelPart, newLevelPartName);
    }

    private static void SetNextLevelPart(List<List<LevelTileInfo>> fullLevelStructure, List<List<LevelInfoDto>> levelPart)
    {
        for (var i = 0; i < levelPart[0].Count; i++)
        {
            var row = new List<LevelTileInfo>();
            for (var j = 0; j < levelPart.Count; j++)
            {
                var levelInfo = new LevelTileInfo();
                levelInfo.TileType = levelPart[j][i].TileType;
                if (levelInfo.TileType == TileType.Wall)
                {
                    var spikesOnWall = new Dictionary<SpikeType, bool>();
                    foreach (var spikeDto in levelPart[j][i].SpikesInfo)
                    {
                        spikesOnWall.Add(spikeDto.SpikeType, spikeDto.IsSetted);
                    }

                    levelInfo.Options = spikesOnWall;
                }
                row.Add(levelInfo);
            }

            fullLevelStructure.Add(row);
        }
    }

    public static void GenerateTwoPartLevel(int firstLevelPartName, int secondLevelPartName)
    {

        var fullLevelStructure = new List<List<LevelTileInfo>>();        
        var levelPart = GetNextLevelPart(firstLevelPartName);
        SetNextLevelPart(fullLevelStructure, levelPart.structure);

        levelPart = GetNextLevelPart(secondLevelPartName);
        SetNextLevelPart(fullLevelStructure, levelPart.structure);

        ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(fullLevelStructure[0].Count, fullLevelStructure.Count);
        var finalLevelStructure = Globals.LevelStructure;
        for (var i = 0; i < fullLevelStructure[0].Count; i++)
        {
            for (var j = 0; j < fullLevelStructure.Count; j++)
            {
                finalLevelStructure[i, j].TileType = fullLevelStructure[j][i].TileType;
                finalLevelStructure[i, j].Options = fullLevelStructure[j][i].Options;
            }
        }

        Globals.SetLevelStructure(finalLevelStructure);

        ButtonClicker.DrawLevelInEditor();
    }

    private class LevelGraph
    {
        public List<LevelGraphItem> Graph { get; set; }
    }

    private class LevelGraphItem
    {
        public string src { get; set; }
        public List<string> dst { get; set; }
    }

    private class LevelTileInfo
    {
        public TileType TileType { get; set; }
        public object Options { get; set; }
    }


}


