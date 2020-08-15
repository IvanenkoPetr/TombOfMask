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

    public static void SetPosition(Transform transform, ref Vector3 MovementDirection, float GameSpeed, Vector3 positionOnPreviousFrame, LevelInfo[,] LevelStructure)
    {
        var positionAfterMovement = transform.position + MovementDirection * GameSpeed * Time.deltaTime;
        if (MovementDirection == Vector3.right)
        {
            var strartPosition = new Vector3((float)Math.Truncate((double)positionOnPreviousFrame.x + 0.001), positionOnPreviousFrame.y, positionOnPreviousFrame.z);
            while (strartPosition.x < positionAfterMovement.x)
            {
                var tileTipe = LevelStructure[(int)strartPosition.x + 1, (int)strartPosition.y].TileType;
                if (tileTipe == TileType.Wall)
                {
                    // return strartPosition;
                    MovementDirection = Vector3.zero;
                    transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return;
                }
                strartPosition = new Vector3(strartPosition.x + 1f, strartPosition.y, strartPosition.z);
            }
        }
        else if (MovementDirection == Vector3.left)
        {

            var strartPosition = new Vector3((float)Math.Truncate((double)positionOnPreviousFrame.x - 0.001) + 1f, positionOnPreviousFrame.y, positionOnPreviousFrame.z);
            while (strartPosition.x > positionAfterMovement.x)
            {
                var tileTipe = LevelStructure[(int)strartPosition.x - 1, (int)strartPosition.y].TileType;
                if (tileTipe == TileType.Wall)
                {
                    //return strartPosition;
                    MovementDirection = Vector3.zero;
                    transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return;
                }
                strartPosition = new Vector3(strartPosition.x - 1f, strartPosition.y, strartPosition.z);
            }
        }
        else if (MovementDirection == Vector3.up)
        {
            var strartPosition = new Vector3(positionOnPreviousFrame.x, (float)Math.Truncate((double)positionOnPreviousFrame.y + 0.001f), positionOnPreviousFrame.z);
            while (strartPosition.y < positionAfterMovement.y)
            {
                var tileTipe = LevelStructure[(int)strartPosition.x, (int)strartPosition.y + 1].TileType;
                if (tileTipe == TileType.Wall)
                {
                    // return strartPosition;
                    MovementDirection = Vector3.zero;
                    transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return;
                }
                strartPosition = new Vector3(strartPosition.x, strartPosition.y + 1f, strartPosition.z);

            }
        }
        else if (MovementDirection == Vector3.down)
        {
            var strartPosition = new Vector3(positionOnPreviousFrame.x, (float)Math.Truncate((double)positionOnPreviousFrame.y - 0.001) + 1f, positionOnPreviousFrame.z);
            while (strartPosition.y > positionAfterMovement.y)
            {
                var tileTipe = LevelStructure[(int)strartPosition.x, (int)strartPosition.y - 1].TileType;
                if (tileTipe == TileType.Wall)
                {
                    //return strartPosition;
                    MovementDirection = Vector3.zero;
                    transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return;
                }
                strartPosition = new Vector3(strartPosition.x, strartPosition.y - 1f, strartPosition.z);
            }
        }

        transform.position = positionAfterMovement;
    }

}
