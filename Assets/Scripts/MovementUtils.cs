using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementUtils
{
    public static LevelInfo GetNextTile(Vector3 currentPosition, Vector3 direction, ref Vector3 nextTileCoordinate, LevelInfo[,] LevelStructure)
    {
        var currentX = (int)Math.Round(currentPosition.x);
        var currentY = (int)Math.Round(currentPosition.y);
        var precision = 0.01f;

        LevelInfo nextTile = null;
        if (direction == Vector3.right)
        {
            if (nextTileCoordinate == Vector3.zero)
            {
                nextTileCoordinate = new Vector3((float)Math.Round(currentPosition.x) + 1, currentPosition.y);
            }

            if (nextTileCoordinate.x - currentPosition.x <= precision + 1)
            {
                nextTile = LevelStructure[(int)nextTileCoordinate.x, currentY];
                nextTileCoordinate = Vector3.zero;
            }
        }
        else if (direction == Vector3.left)
        {

            if (nextTileCoordinate == Vector3.zero)
            {
                nextTileCoordinate = new Vector3((float)Math.Round(currentPosition.x) - 1, currentPosition.y);
            }

            if (currentPosition.x - nextTileCoordinate.x <= precision + 1)
            {
                nextTile = LevelStructure[(int)nextTileCoordinate.x, currentY];
                nextTileCoordinate = Vector3.zero;
            }
        }
        else if (direction == Vector3.up)
        {

            if (nextTileCoordinate == Vector3.zero)
            {
                nextTileCoordinate = new Vector3(currentPosition.x, (float)Math.Round(currentPosition.y) + 1);
            }

            if (nextTileCoordinate.y - currentPosition.y <= precision + 1)
            {
                nextTile = LevelStructure[currentX, (int)nextTileCoordinate.y];
                nextTileCoordinate = Vector3.zero;
            }
        }
        else if (direction == Vector3.down)
        {
            if (nextTileCoordinate == Vector3.zero)
            {
                nextTileCoordinate = new Vector3((float)(int)currentPosition.x, (float)Math.Round(currentPosition.y) - 1);
            }

            if (currentPosition.y - nextTileCoordinate.y <= precision + 1)
            {
                nextTile = LevelStructure[currentX, (int)nextTileCoordinate.y];
                nextTileCoordinate = Vector3.zero;
            }
        }

        return nextTile;
    }

}
