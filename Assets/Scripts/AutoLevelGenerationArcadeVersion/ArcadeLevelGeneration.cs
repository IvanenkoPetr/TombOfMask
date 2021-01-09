using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ArcadeLevelGeneration
{
    private static string CurrentLevelPart;
    public static void GenerateRandomLevel(int numberOfLevelParts)
    {

        var fullLevelStructure = new List<List<LevelTileInfo>>();
        var currentLevelPart = string.Empty;
        var isFirstPart = true;
        while (numberOfLevelParts > 0)
        {

            var levelPart = GetNextLevelPart(currentLevelPart, isFirstPart);
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
                finalLevelStructure[i, j].x = i;
                finalLevelStructure[i, j].y = j;
            }
        }

        Globals.SetLevelStructure(finalLevelStructure);

        ButtonClicker.DrawLevelInEditor();
    }

    private static (List<List<LevelInfoDto>> structure, string name) GetNextLevelPart(string currentLevelPart, bool isFirstPart)
    {
        string newLevelPartName;
        if (isFirstPart)
        {
            newLevelPartName = @"RandomLevelParts\LevelStartStructure";
        }
        else
        {

            var levelGraphText = Resources.Load<TextAsset>("RandomLevelParts/Graph").text;
            var levelGraphObject = JsonConvert.DeserializeObject<LevelGraph>(levelGraphText);

            var destinations = new List<KeyValuePair<string, List<List<LevelInfoDto>>>>();

            var currentLevelStructure = Globals.AllLevelParts[currentLevelPart];
            foreach (var levelPart in Globals.AllLevelParts)
            {
                if (levelPart.Key.Contains("Start"))
                {
                    continue;
                }

                if (IsLevelPartCanBeConnected(currentLevelStructure, levelPart.Value))
                {
                    destinations.Add(levelPart);
                }
            }

            newLevelPartName = destinations.OrderBy(a => Random.value).First().Key;
        }

        var newLevelPart = Globals.AllLevelParts[newLevelPartName];
        CurrentLevelPart = newLevelPartName;

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

    private static bool IsLevelPartCanBeConnected(List<List<LevelInfoDto>> current, List<List<LevelInfoDto>> next)
    {
        var result = false;
        for(var i = 0; i< current.Count; i++)
        {
            if((current[i].Last().TileType == TileType.Empty) && (next[i].First().TileType == TileType.Empty))
            {
                result = true;
                break;
            }
        }
        return result;
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

    public static void PlayerMovedUp(GameObject player)
    {
        if (!Globals.IsArcadeMode)
        {
            return;
        }

        var addedLevelPart = GetNextLevelPart(CurrentLevelPart, false).structure;

        var levelStructure = Globals.LevelStructure;
        var currentHeith = levelStructure.GetLength(1);
        var newLevelStructure = new LevelInfo[levelStructure.GetLength(0),
            levelStructure.GetLength(1) + addedLevelPart[0].Count];

        for (var i = 0; i < levelStructure.GetLength(1); i++)
        {
            for (var j = 0; j < levelStructure.GetLength(0); j++)
            {
                newLevelStructure[j, i] = levelStructure[j, i];         
            }
        }

        for (var i = 0; i < addedLevelPart[0].Count; i++)
        {
            for (var j = 0; j < addedLevelPart.Count; j++)
            {
                var levelInfo = new LevelInfo();
                levelInfo.x = j;
                levelInfo.y = i + levelStructure.GetLength(1);
                levelInfo.TileType = addedLevelPart[j][i].TileType;
                if (levelInfo.TileType == TileType.Wall)
                {
                    var spikesOnWall = new Dictionary<SpikeType, bool>();
                    foreach (var spikeDto in addedLevelPart[j][i].SpikesInfo)
                    {
                        spikesOnWall.Add(spikeDto.SpikeType, spikeDto.IsSetted);
                    }

                    levelInfo.Options = spikesOnWall;                    
                }
                newLevelStructure[levelInfo.x, levelInfo.y] = levelInfo;
            }            
        }

        Globals.SetLevelStructure(newLevelStructure);

        var mainGameObject = ConstractorUI.MainGame.transform;

        Globals.GenerateLevelPartForArcade(mainGameObject, GameplaySettings.MainCamera, currentHeith);
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


