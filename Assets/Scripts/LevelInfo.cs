using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo:MonoBehaviour
{
    public int x { get; set; }
    public int y { get; set; }
    public TileType TileType { get; set; }

}

public enum TileType
{
    Empty,
    Wall,
    Player,
    Enemy,
    Collectible
}
 

