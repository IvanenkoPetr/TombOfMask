using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ArcadeLevelGeneration
{
    private static int levelPart = 0;
    private static string CurrentLevelPart;
    private static Vector2 PreviousLevelDirection;
    private static List<List<LevelTileInfo>> FullLevelStructureInLists = new List<List<LevelTileInfo>>();
    private static List<Vector2> previousLevelGrowth = new List<Vector2>();

    public static void GenerateRandomLevel(int numberOfLevelParts)
    {
        FullLevelStructureInLists.Clear();
        previousLevelGrowth.Clear();

        //Globals.ArcadeCurrentLevelPartPosition = new Dictionary<Vector2, int>
        //{
        //    [Vector2.up] = 25,
        //    [Vector2.down] = 25,
        //    [Vector2.left] = 25,
        //    [Vector2.right] = 25,

        //};

        levelPart = 0;

        Globals.LoadAllLevelParts();
        CurrentLevelPart = string.Empty;
        var isFirstPart = true;
        while (numberOfLevelParts > 0)
        {

            var levelPart = GetNextLevelPart(CurrentLevelPart, isFirstPart);
            if (levelPart.stopGeneration)
            {
                break;
            }

            CurrentLevelPart = levelPart.name;
            SetNextLevelPart(FullLevelStructureInLists, levelPart);
            numberOfLevelParts--;
            isFirstPart = false;
        }

        ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(FullLevelStructureInLists[0].Count, FullLevelStructureInLists.Count);
        var finalLevelStructure = Globals.LevelStructureNew;
        for (var i = 0; i < FullLevelStructureInLists[0].Count; i++)
        {
            for (var j = 0; j < FullLevelStructureInLists.Count; j++)
            {       
                    finalLevelStructure[(i, j)].TileType = FullLevelStructureInLists[j][i].TileType;
                    finalLevelStructure[(i, j)].Options = FullLevelStructureInLists[j][i].Options;
                    finalLevelStructure[(i, j)].x = i;
                    finalLevelStructure[(i, j)].y = j;        
            }
        }
        Globals.SetLevelStructureNew(finalLevelStructure);

        ButtonClicker.DrawLevelInEditor();
    }

    private static (List<List<LevelInfoDto>> CurrentLevelPart, Vector2 direction, string name, bool stopGeneration) GetNextLevelPart(string currentLevelPart, bool isFirstPart = false)
    {
        (List<List<LevelInfoDto>> CurrentLevelPart, Vector2 direction, string name, bool stopGeneration) result;

        result = (Globals.AllLevelParts[@"RandomLevelParts\LevelStartStructure"], Vector2.up, @"RandomLevelParts\LevelStartStructure", true);

        //if (isFirstPart)
        //{
        //    var name = @"RandomLevelParts\LevelStartStructure";
        //    //var name = @"RandomLevelParts\RandomLevelPart3";
        //    result = (Globals.AllLevelParts[name], Vector2.up, name, false);
        //    PreviousLevelDirection = Vector2.up;
        //}
        //else
        //{

        //    var destinations = new List<(string levelPartName, Vector2 direction)>();

        //    var currentLevelStructure = Globals.AllLevelParts[currentLevelPart];
        //    foreach (var levelPart in Globals.AllLevelParts)
        //    {
        //        if (levelPart.Key.Contains("Start"))
        //        {
        //            continue;
        //        }

        //        var conection = IsLevelPartCanBeConnected(currentLevelStructure, levelPart.Value);
        //        if (conection.IsConnected)
        //        {
        //            destinations.Add((levelPart.Key, conection.direction));
        //        }
        //    }

        //    var newLevelPart = destinations.OrderBy(a => UnityEngine.Random.value).FirstOrDefault();
        //    if (String.IsNullOrEmpty(newLevelPart.levelPartName))
        //    {
        //        Debug.LogError(newLevelPart.levelPartName);
        //        return result;


        //    }
        //    PreviousLevelDirection = newLevelPart.direction;
        //    result = (Globals.AllLevelParts[newLevelPart.levelPartName], newLevelPart.direction, newLevelPart.levelPartName, false);
        //    previousLevelGrowth.Add(newLevelPart.direction);
        //    Debug.Log(result.name);

        //}

        switch (levelPart)
        {
            case 0:
                var name = @"RandomLevelParts\LevelStartStructure";
                result = (Globals.AllLevelParts[name], Vector2.up, name, false);
                break;
            case 1:
                name = @"RandomLevelParts\RandomLevelPart9";
                result = (Globals.AllLevelParts[name], Vector2.up, name, false);
                break;
            case 2:
                name = @"RandomLevelParts\RandomLevelPart8";
                result = (Globals.AllLevelParts[name], Vector2.left, name, false);
                break;
            case 3:
                name = @"RandomLevelParts\RandomLevelPart1";
                result = (Globals.AllLevelParts[name], Vector2.up, name, false);
                break;
            case 4:
                name = @"RandomLevelParts\RandomLevelPart6";
                result = (Globals.AllLevelParts[name], Vector2.up, name, false);
                break;
            case 5:
                name = @"RandomLevelParts\RandomLevelPart9";
                result = (Globals.AllLevelParts[name], Vector2.right, name, false);
                break;
            case 6:
                name = @"RandomLevelParts\RandomLevelPart11";
                result = (Globals.AllLevelParts[name], Vector2.down, name, false);
                break;
            case 7:
                name = @"RandomLevelParts\RandomLevelPart11";
                result = (Globals.AllLevelParts[name], Vector2.down, name, false);
                break;
            case 8:
                name = @"RandomLevelParts\RandomLevelPart11";
                result = (Globals.AllLevelParts[name], Vector2.down, name, false);
                break;
        }
        levelPart++;

        return result; 
    }

    private static (List<List<LevelInfoDto>> structure, string name) GetNextLevelPart(int levelPart)
    {


        var newLevelPartName = $@"RandomLevelParts\RandomLevelPart{levelPart}";
        var newLevelPart = ResourcesManagment.LoadLevelStructureInDto(newLevelPartName);

        return (newLevelPart, newLevelPartName);
    }

    private static void SetNextLevelPart(List<List<LevelTileInfo>> fullLevelStructure, (List<List<LevelInfoDto>> CurrentLevelPart, Vector2 direction, string name, bool stopGeneration) levelPart)
    {
        var currentLevelPartHeight = levelPart.CurrentLevelPart[0].Count;
        var currentLevelPartWidth = levelPart.CurrentLevelPart.Count;

        var fullLevelStructureWidth = fullLevelStructure.FirstOrDefault()?.Count ?? 0;
        var fullLevelStructureHeight = fullLevelStructure.Count;

        if (levelPart.direction == Vector2.up)
        {
            var newFullLevelStructureWidth = Math.Max(Globals.ArcadeCurrentLevelPartPosition[Vector2.left] + currentLevelPartWidth, fullLevelStructureWidth);
            var newFullLevelStructureHeight = Math.Max(Globals.ArcadeCurrentLevelPartPosition[Vector2.up] + currentLevelPartHeight, fullLevelStructureHeight);

            for (var i = 0; i < newFullLevelStructureHeight; i++) //высота fullLevelStructureHeight
            {
                List<LevelTileInfo> row;
                if(i < fullLevelStructureHeight)
                {
                    row = fullLevelStructure[i];
                }
                else
                {
                    row = new List<LevelTileInfo>();
                    fullLevelStructure.Add(row);
                }

                for (var j = 0; j < newFullLevelStructureWidth; j++)// ширина
                {
                    LevelTileInfo levelInfo;
                    if(j < row.Count)
                    {
                        levelInfo = row[j];
                    }
                    else
                    {
                        levelInfo = new LevelTileInfo();
                        levelInfo.TileType = TileType.Empty;
                        row.Add(levelInfo);
                    }                   
                    
                }                
            }

            for (var i = 0; i < levelPart.CurrentLevelPart[0].Count; i++)
            {
                for (var j = 0; j < levelPart.CurrentLevelPart.Count; j++)
                {
                    var levelInfo = fullLevelStructure[i + Globals.ArcadeCurrentLevelPartPosition[Vector2.up]][j + Globals.ArcadeCurrentLevelPartPosition[Vector2.left]];
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

            Globals.ArcadeCurrentLevelPartPosition[Vector2.down] = Globals.ArcadeCurrentLevelPartPosition[Vector2.up];
            Globals.ArcadeCurrentLevelPartPosition[Vector2.up] += currentLevelPartHeight;
            Globals.ArcadeCurrentLevelPartPosition[Vector2.right] = Globals.ArcadeCurrentLevelPartPosition[Vector2.left] + currentLevelPartWidth;
                            
        }
        else if (levelPart.direction == Vector2.right)
        {
            var newFullLevelStructureWidth = Math.Max(Globals.ArcadeCurrentLevelPartPosition[Vector2.right] + currentLevelPartWidth, fullLevelStructureWidth);
            var newFullLevelStructureHeight = Math.Max(Globals.ArcadeCurrentLevelPartPosition[Vector2.down] + currentLevelPartHeight, fullLevelStructureHeight);

            for (var i = 0; i < newFullLevelStructureHeight; i++)//высота fullLevelStructureHeight
            {
                List<LevelTileInfo> row;
                if (i < fullLevelStructureHeight)
                {
                    row = fullLevelStructure[i];
                }
                else
                {
                    row = new List<LevelTileInfo>();
                    fullLevelStructure.Add(row);
                }

                for (var j = 0; j < newFullLevelStructureWidth; j++)// ширина
                {
                    LevelTileInfo levelInfo;
                    if (j < row.Count)
                    {
                        levelInfo = row[j];
                    }
                    else
                    {
                        levelInfo = new LevelTileInfo();
                        levelInfo.TileType = TileType.Empty;
                        row.Add(levelInfo);
                    }

                }
            }

            for (var i = 0; i < levelPart.CurrentLevelPart[0].Count; i++)
            {
                for (var j = 0; j < levelPart.CurrentLevelPart.Count; j++)
                {
                    var levelInfo = fullLevelStructure[i + Globals.ArcadeCurrentLevelPartPosition[Vector2.down]][j + Globals.ArcadeCurrentLevelPartPosition[Vector2.right]];
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

            Globals.ArcadeCurrentLevelPartPosition[Vector2.left] = Globals.ArcadeCurrentLevelPartPosition[Vector2.right];
            Globals.ArcadeCurrentLevelPartPosition[Vector2.right] = Globals.ArcadeCurrentLevelPartPosition[Vector2.left] + currentLevelPartWidth;
            Globals.ArcadeCurrentLevelPartPosition[Vector2.up] = Globals.ArcadeCurrentLevelPartPosition[Vector2.down] + currentLevelPartHeight;
        
        }else if (levelPart.direction == Vector2.left)
        {
            int  newFullLevelStructureWidth;
            if ((Globals.ArcadeCurrentLevelPartPosition[Vector2.left] - currentLevelPartWidth)< 0)
            {
                newFullLevelStructureWidth = fullLevelStructureWidth + Math.Abs(Globals.ArcadeCurrentLevelPartPosition[Vector2.left] - currentLevelPartWidth);
                Globals.ArcadeCurrentLevelPartPosition[Vector2.left]+= Math.Abs(Globals.ArcadeCurrentLevelPartPosition[Vector2.left] - currentLevelPartWidth);
                Globals.ArcadeCurrentLevelPartPosition[Vector2.right]+= Math.Abs(Globals.ArcadeCurrentLevelPartPosition[Vector2.left] - currentLevelPartWidth);
            }
            else
            {
                newFullLevelStructureWidth = currentLevelPartWidth;
            } 

            //var newFullLevelStructureWidth = Math.Max(Globals.ArcadeCurrentLevelPartPosition[Vector2.left] - currentLevelPartWidth, currentLevelPartWidth);
            var newFullLevelStructureHeight = Math.Max(Globals.ArcadeCurrentLevelPartPosition[Vector2.down] + currentLevelPartHeight, fullLevelStructureHeight);

            for (var i = 0; i < newFullLevelStructureHeight; i++)//высота fullLevelStructureHeight
            {
                List<LevelTileInfo> row;
                if (i < fullLevelStructureHeight)
                {
                    row = fullLevelStructure[i];
                }
                else
                {
                    row = new List<LevelTileInfo>();
                    fullLevelStructure.Add(row);
                    for(var k = 0; k< fullLevelStructureWidth; k++)
                    {
                        var levelInfo = new LevelTileInfo();
                        levelInfo.TileType = TileType.Empty;
                        row.Add(levelInfo);
                    }
                }

                for (var j = 0; j < newFullLevelStructureWidth; j++)// ширина
                {
                    LevelTileInfo levelInfo;
                    if (j < fullLevelStructureWidth)
                    {
                        levelInfo = row[j];
                    }
                    else
                    {
                        levelInfo = new LevelTileInfo();
                        levelInfo.TileType = TileType.Empty;
                        row.Insert(0, levelInfo);
                    }
                }            
            }

            for (var i = 0; i < levelPart.CurrentLevelPart[0].Count; i++)
            {
                for (var j = 0; j < levelPart.CurrentLevelPart.Count; j++)
                {
                    var levelInfo = fullLevelStructure[i + Globals.ArcadeCurrentLevelPartPosition[Vector2.down]][Globals.ArcadeCurrentLevelPartPosition[Vector2.left] - currentLevelPartWidth + j];
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
            Globals.ArcadeCurrentLevelPartPosition[Vector2.left] = Globals.ArcadeCurrentLevelPartPosition[Vector2.left] - currentLevelPartWidth;
            Globals.ArcadeCurrentLevelPartPosition[Vector2.right] = Globals.ArcadeCurrentLevelPartPosition[Vector2.left] + currentLevelPartWidth;
            Globals.ArcadeCurrentLevelPartPosition[Vector2.up] = Globals.ArcadeCurrentLevelPartPosition[Vector2.down] + currentLevelPartHeight;
        }
        else if (levelPart.direction == Vector2.down)
        {
            var newFullLevelStructureWidth = Math.Max(Globals.ArcadeCurrentLevelPartPosition[Vector2.left] + currentLevelPartWidth, fullLevelStructureWidth);
            int newFullLevelStructureHeight;
            if ((Globals.ArcadeCurrentLevelPartPosition[Vector2.down] - currentLevelPartHeight) < 0)
            {
                newFullLevelStructureHeight = fullLevelStructureHeight + Math.Abs(Globals.ArcadeCurrentLevelPartPosition[Vector2.down] - currentLevelPartHeight);
                var increaded = Math.Abs(Globals.ArcadeCurrentLevelPartPosition[Vector2.down] - currentLevelPartHeight);
                Globals.ArcadeCurrentLevelPartPosition[Vector2.down] += increaded;
                Globals.ArcadeCurrentLevelPartPosition[Vector2.up] += increaded;
            }
            else
            {
                newFullLevelStructureHeight = currentLevelPartHeight;
            }

            for (var i = 0; i < newFullLevelStructureHeight; i++)//высота fullLevelStructureHeight
            {
                List<LevelTileInfo> row;
                if (i < fullLevelStructureHeight)
                {
                    row = fullLevelStructure[i];
                }
                else
                {
                    row = new List<LevelTileInfo>();
                    fullLevelStructure.Insert(0, row);
                }

                for (var j = 0; j < newFullLevelStructureWidth; j++)// ширина
                {
                    LevelTileInfo levelInfo;
                    if (j < row.Count)
                    {
                        levelInfo = row[j];
                    }
                    else
                    {
                        levelInfo = new LevelTileInfo();
                        levelInfo.TileType = TileType.Empty;
                        row.Add(levelInfo);
                    }

                }
            }

            for (var i = 0; i < levelPart.CurrentLevelPart[0].Count; i++)
            {
                for (var j = 0; j < levelPart.CurrentLevelPart.Count; j++)
                {
                    var levelInfo = fullLevelStructure[i + Globals.ArcadeCurrentLevelPartPosition[Vector2.down] - currentLevelPartHeight][j + Globals.ArcadeCurrentLevelPartPosition[Vector2.left]];
                    //var levelInfo = fullLevelStructure[i + Globals.ArcadeCurrentLevelPartPosition[Vector2.up]][j + Globals.ArcadeCurrentLevelPartPosition[Vector2.left]];
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
            Globals.ArcadeCurrentLevelPartPosition[Vector2.up] = Globals.ArcadeCurrentLevelPartPosition[Vector2.down];
            Globals.ArcadeCurrentLevelPartPosition[Vector2.down] -= currentLevelPartHeight;
            Globals.ArcadeCurrentLevelPartPosition[Vector2.right] = Globals.ArcadeCurrentLevelPartPosition[Vector2.left] + currentLevelPartWidth;
        }


    }

    private static (bool IsConnected, Vector2 direction) IsLevelPartCanBeConnected(List<List<LevelInfoDto>> current, List<List<LevelInfoDto>> next)
    {
        var result = (false, Vector2.zero);
        var posibleDirections = new List<(bool IsConnected, Vector2 direction)>();
        var mainGameObject = ConstractorUI.MainGame.transform;
        var threeLastElement = previousLevelGrowth.Skip(previousLevelGrowth.Count - 1);
        if (!threeLastElement.Any(a => a == Vector2.down))
        {
            //if (mainGameObject.position.y < 35)
            // {
            for (var i = 0; i < Math.Min(current.Count, next.Count); i++)
                {
                    if ((current[i].Last().TileType == TileType.Empty) && (next[i].First().TileType == TileType.Empty))
                    {
                        posibleDirections.Add((true, Vector2.up));
                    }
                }
            //}
        }

        if (!threeLastElement.Any(a => a == Vector2.left))
        {
            // if (mainGameObject.position.x < 35)
            // {
            for (var i = 0; i < Math.Min(current.Last().Count, next.First().Count); i++)
                {
                    if (current.Last()[i].TileType == TileType.Empty && next.First()[i].TileType == TileType.Empty)
                    {
                        posibleDirections.Add((true, Vector2.right));
                    }
                }
            // }
        }

        if (!threeLastElement.Any(a => a == Vector2.right))
        {
            // if (mainGameObject.position.x > 15)
            // {
            for (var i = 0; i < Math.Min(current.First().Count, next.Last().Count); i++)
                {
                    if (current.First()[i].TileType == TileType.Empty && next.Last()[i].TileType == TileType.Empty)
                    {
                        posibleDirections.Add((true, Vector2.left));
                    }
                }
            // }
        }
        if (!threeLastElement.Any(a => a == Vector2.up))
        {
            // if (mainGameObject.position.y > 15)
            // {
            for (var i = 0; i < Math.Min(current.Count, next.Count); i++)
                {
                    if ((current[i].First().TileType == TileType.Empty) && (next[i].Last().TileType == TileType.Empty))
                    {
                        posibleDirections.Add((true, Vector2.down));
                    }
                }
            // }
        }

        if (posibleDirections.Count == 0)
        {
            return  (false, Vector2.zero);
        }
        else
        {
            return posibleDirections.OrderBy(a => UnityEngine.Random.value).First();
        }

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
        //if (!Globals.IsArcadeMode)
        //{
        //    return;
        //}

        var nextLevelPart = GetNextLevelPart(CurrentLevelPart, false);
        var addedLevelPart = nextLevelPart.CurrentLevelPart;
        SetNextLevelPart(FullLevelStructureInLists, nextLevelPart);    

        //ConstractorUI.UIConstractor.GetComponent<ConstractorUI>().InstantiateLevelField(FullLevelStructureInLists[0].Count, FullLevelStructureInLists.Count);
        var finalLevelStructure = Globals.LevelStructureNew;
        var cameraPosition = new Vector3(GameplaySettings.MainCamera.transform.position.x, GameplaySettings.MainCamera.transform.position.y, 0);

        for (var i = 0; i < FullLevelStructureInLists[0].Count; i++)
        {
            for (var j = 0; j < FullLevelStructureInLists.Count; j++)
            {
                var pointPosition = new Vector3(i, j, 0);
                if(Vector3.Distance(cameraPosition, pointPosition) > 30)
                {
                    finalLevelStructure[(i, j)].TileType = TileType.Empty;
                    finalLevelStructure[(i, j)].Options = null;
                    FullLevelStructureInLists[j][i].TileType = TileType.Empty;
                    FullLevelStructureInLists[j][i].Options = null;
                }

                finalLevelStructure[(i, j)].TileType = FullLevelStructureInLists[j][i].TileType;
                finalLevelStructure[(i, j)].Options = FullLevelStructureInLists[j][i].Options;
                finalLevelStructure[(i, j)].x = i;
                finalLevelStructure[(i, j)].y = j;
            }
        }

        Globals.SetLevelStructureNew(finalLevelStructure);

        var mainGameObject = ConstractorUI.MainGame.transform;

        Globals.GenerateLevelPartForArcade(mainGameObject, GameplaySettings.MainCamera);

        CurrentLevelPart = nextLevelPart.name;
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


