using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelInfo:MonoBehaviour
{
    public int x { get; set; }
    public int y { get; set; }
    public TileType TileType { get; set; }


    public LevelInfoDto ToDTOObject()
    {
        var result = new LevelInfoDto()
        {
            x = x,
            y = y,
            TileType = TileType
        };

        return result;
    }

}

[Serializable]
public class LevelInfoDto
{
    public int x { get; set; }
    public int y { get; set; }
    public TileType TileType { get; set; }
}

[Serializable]
public enum TileType
{
    Empty,
    Wall,
    Player,
    Enemy,
    Collectible,
    Hatch
}
 

