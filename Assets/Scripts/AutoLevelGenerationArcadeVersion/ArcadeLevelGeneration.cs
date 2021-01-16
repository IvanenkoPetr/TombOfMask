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
            SetNextLevelPart(fullLevelStructure, levelPart);
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

    private static (List<List<LevelInfoDto>> CurrentLevelPart, Vector2 direction, string name) GetNextLevelPart(string currentLevelPart, bool isFirstPart)
    {
        (List<List<LevelInfoDto>> CurrentLevelPart, Vector2 direction, string name) result;
        if (isFirstPart)
        {
            var name = @"RandomLevelParts\LevelStartStructure";
            result = ( Globals.AllLevelParts[name], Vector2.up, name);
        }
        else
        {

            var destinations = new List<(string levelPartName, Vector2 direction)>();

            var currentLevelStructure = Globals.AllLevelParts[currentLevelPart];
            foreach (var levelPart in Globals.AllLevelParts)
            {
                if (levelPart.Key.Contains("Start"))
                {
                    continue;
                }

                var conection = IsLevelPartCanBeConnected(currentLevelStructure, levelPart.Value);
                if (conection.IsConnected)
                {
                    destinations.Add((levelPart.Key, conection.direction));
                }
            }

            var newLevelPart = destinations.OrderBy(a => Random.value).First();
            result = (Globals.AllLevelParts[newLevelPart.levelPartName], newLevelPart.direction, newLevelPart.levelPartName);
        }

        return result; 
    }

    private static (List<List<LevelInfoDto>> structure, string name) GetNextLevelPart(int levelPart)
    {


        var newLevelPartName = $@"RandomLevelParts\RandomLevelPart{levelPart}";
        var newLevelPart = ResourcesManagment.LoadLevelStructureInDto(newLevelPartName);

        return (newLevelPart, newLevelPartName);
    }

    private static void SetNextLevelPart(List<List<LevelTileInfo>> fullLevelStructure, (List<List<LevelInfoDto>> CurrentLevelPart, Vector2 direction, string name) levelPart)
    {
        if(levelPart.direction == Vector2.up)
        {
            var currentLevelPartHeight = levelPart.CurrentLevelPart[0].Count;
            var currentLevelWidth = levelPart.CurrentLevelPart.Count;

            var fullLevelStructureWidth = fullLevelStructure.Count;
            var fullLevelStructureHeight = fullLevelStructure.FirstOrDefault()?.Count ?? 0;

            var newFullLevelStructureWidth = Globals.ArcadeCurrentLevelPartPosition[Vector2.left] + currentLevelWidth;
            var newFullLevelStructureHeight = Globals.ArcadeCurrentLevelPartPosition[Vector2.down] + currentLevelPartHeight;

            for (var i = 0; i < newFullLevelStructureHeight; i++)//высота fullLevelStructureHeight
            {
                var row = new List<LevelTileInfo>();
                for (var j = fullLevelStructureWidth; j < newFullLevelStructureWidth; j++)// ширина
                {
                    var levelInfo = new LevelTileInfo();
                    levelInfo.TileType = TileType.Empty;

                    row.Add(levelInfo);
                }

                fullLevelStructure.Add(row);
            }


            for (var i = 0; i < levelPart.CurrentLevelPart[0].Count; i++)
            {
                for (var j = 0; j < levelPart.CurrentLevelPart.Count; j++)
                {
                    var levelInfo = fullLevelStructure[i][j];
                    levelInfo.TileType = levelPart.CurrentLevelPart[j][i].TileType;
                    if (levelInfo.TileType == TileType.Wall)
                    {
                        var spikesOnWall = new Dictionary<SpikeType, bool>();
                        foreach (var spikeDto in levelPart.CurrentLevelPart[j][i].SpikesInfo)
                        {
                            spikesOnWall.Add(spikeDto.SpikeType, spikeDto.IsSetted);
                        }

                        levelInfo.Options = spikesOnWall;
                    }
          
                }          
            }
        }
      
    }

    private static (bool IsConnected, Vector2 direction) IsLevelPartCanBeConnected(List<List<LevelInfoDto>> current, List<List<LevelInfoDto>> next)
    {
        var result = (false, Vector2.zero);
        
        for(var i = 0; i< current.Count; i++)
        {
            if((current[i].Last().TileType == TileType.Empty) && (next[i].First().TileType == TileType.Empty))
            {
                result = (true, Vector2.up);
                return result;
            }
        }

        for(var i = 0; i < current[0].Count; i++) 
        {
            if (current.Last()[i].TileType == TileType.Empty && next.First()[i].TileType == TileType.Empty)
            {
                result = (true, Vector2.right);
                return result;
            }
        }

        return result;
    }
    public static void GenerateTwoPartLevel(int firstLevelPartName, int secondLevelPartName)
    {

        //var fullLevelStructure = new List<List<LevelTileInfo>>();
        //var levelPart = GetNextLevelPart(firstLevelPartName);
        //SetNextLevelPart(fullLevelStructure, levelPart.structure);

        //levelPart = GetNextLevelPart(secondLevelPartName);
        //SetNextLevelPart(fullLevelStructure, levelPart.structure);

        //ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(fullLevelStructure[0].Count, fullLevelStructure.Count);
        //var finalLevelStructure = Globals.LevelStructure;
        //for (var i = 0; i < fullLevelStructure[0].Count; i++)
        //{
        //    for (var j = 0; j < fullLevelStructure.Count; j++)
        //    {
        //        finalLevelStructure[i, j].TileType = fullLevelStructure[j][i].TileType;
        //        finalLevelStructure[i, j].Options = fullLevelStructure[j][i].Options;
        //    }
        //}

        //Globals.SetLevelStructure(finalLevelStructure);

        //ButtonClicker.DrawLevelInEditor();
    }

    public static void PlayerMovedUp(GameObject player)
    {
        if (!Globals.IsArcadeMode)
        {
            return;
        }

        var addedLevelPart = GetNextLevelPart(CurrentLevelPart, false).CurrentLevelPart;

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


