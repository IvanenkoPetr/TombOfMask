using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//public class LevelInfo:MonoBehaviour
public class LevelInfo : MonoBehaviour
{
    public int x { get; set; }
    public int y { get; set; }
    public TileType TileType { get; set; }
    public object Options { get; set; }

    public bool IsActive { get; set; }

    public bool IsPassable
    {
        get
        {
            var passable = new[]
            {
                TileType.Empty,
                TileType.Player,
                TileType.Collectible,
                TileType.Hatch,
                TileType.Star,
                TileType.Exit
            };

            return passable.Any(a => a == TileType);
        }
    }

    public bool IsPassableForEnemy
    {
        get
        {
            var passable = new[]
            {
                TileType.Empty,
                TileType.Player,
                TileType.Collectible,
                TileType.Hatch,
                TileType.Star,
                TileType.Exit,
                TileType.Enemy,
                TileType.HorizontalEnemy,
                TileType.VerticalEnemy,
                TileType.RandomEnemy
            };

            return passable.Any(a => a == TileType);
        }
    }

    public LevelInfoDto ToDTOObject()
    {
        var result = new LevelInfoDto()
        {
            x = x,
            y = y,
            TileType = TileType
        };

        if (TileType == TileType.Wall)
        {
            var wallOptions = (Dictionary<SpikeType, bool>)Options;
            var dtoOptions = new List<SpikesOnWallDto>();
            foreach (var spike in wallOptions)
            {
                var spikeDto = new SpikesOnWallDto()
                {
                    SpikeType = spike.Key,
                    IsSetted = spike.Value
                };
                dtoOptions.Add(spikeDto);
            }
            result.SpikesInfo = dtoOptions.ToArray();
        }

        return result;
    }

}

[Serializable]
public class SpikesOnWallDto
{
    public SpikeType SpikeType { get; set; }
    public bool IsSetted { get; set; }
}

[Serializable]
public class LevelInfoDto
{
    public int x { get; set; }
    public int y { get; set; }
    public TileType TileType { get; set; }

    //public object Options { get; set; }
    public SpikesOnWallDto[] SpikesInfo { get; set; }
}

[Serializable]
public enum TileType
{
    Empty,
    Wall,
    Player,
    Enemy,
    HorizontalEnemy,
    VerticalEnemy,
    RandomEnemy,
    Collectible,
    Hatch,
    Star,
    Exit
}

[Serializable]
public enum SpikeType
{
    Left,
    Right,
    Top,
    Bottom
}




