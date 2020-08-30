using DG.Tweening;
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
                var tile = LevelStructure[(int)strartPosition.x + 1, (int)strartPosition.y];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var isPlayer = transform.gameObject.GetComponent<PlayerMovement>() != null;
                    if (isPlayer && CheckSpikesCollision(tile, MovementDirection))
                    {
                        var camera = ConstractorUI.MainCamera;
                        camera.transform.SetParent(null);
                        UnityEngine.Object.Destroy(transform.gameObject);
                    }
                    else
                    {
                        MovementDirection = Vector3.zero;
                        transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                        if (isPlayer)
                        {
                            DOTween.Sequence().Append(transform.DOScaleX(0.75f, 2f))
                                .Append(transform.DOScaleX(1f, 2f));

                            DOTween.Sequence().Append(transform.DOMoveX(transform.position.x + 0.125f, 2f))
                                .Append(transform.DOMoveX(transform.position.x, 2f));
                        }

                    }

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
                var tile = LevelStructure[(int)strartPosition.x - 1, (int)strartPosition.y];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var isPlayer = transform.gameObject.GetComponent<PlayerMovement>() != null;
                    if (isPlayer && CheckSpikesCollision(tile, MovementDirection))
                    {
                        var camera = ConstractorUI.MainCamera;
                        camera.transform.SetParent(null);
                        UnityEngine.Object.Destroy(transform.gameObject);
                    }
                    else
                    {
                        MovementDirection = Vector3.zero;
                        transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);

                        if (isPlayer)
                        {
                            DOTween.Sequence().Append(transform.DOScaleX(0.75f, 2f))
                                .Append(transform.DOScaleX(1f, 2f));

                            DOTween.Sequence().Append(transform.DOMoveX(transform.position.x - 0.125f, 2f))
                                .Append(transform.DOMoveX(transform.position.x, 2f));
                        }
                    }

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
                var tile = LevelStructure[(int)strartPosition.x, (int)strartPosition.y + 1];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var isPlayer = transform.gameObject.GetComponent<PlayerMovement>() != null;
                    if (isPlayer && CheckSpikesCollision(tile, MovementDirection))
                    {
                        var camera = ConstractorUI.MainCamera;
                        camera.transform.SetParent(null);
                        UnityEngine.Object.Destroy(transform.gameObject);
                    }
                    else
                    {
                        MovementDirection = Vector3.zero;
                        transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);

                        if (isPlayer)
                        {
                            DOTween.Sequence().Append(transform.DOScaleY(0.75f, 2f))
                                .Append(transform.DOScaleY(1f, 2f));

                            DOTween.Sequence().Append(transform.DOMoveY(transform.position.y + 0.125f, 2f))
                                .Append(transform.DOMoveY(transform.position.y, 2f));
                        }
                    }

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
                var tile = LevelStructure[(int)strartPosition.x, (int)strartPosition.y - 1];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var isPlayer = transform.gameObject.GetComponent<PlayerMovement>() != null;
                    if (isPlayer && CheckSpikesCollision(tile, MovementDirection))
                    {
                        var camera = ConstractorUI.MainCamera;
                        camera.transform.SetParent(null);
                        UnityEngine.Object.Destroy(transform.gameObject);
                    }
                    else
                    {
                        MovementDirection = Vector3.zero;
                        transform.position = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);

                        if (isPlayer)
                        {
                            DOTween.Sequence().Append(transform.DOScaleY(0.75f, 2f))
                                .Append(transform.DOScaleY(1f, 2f));

                            DOTween.Sequence().Append(transform.DOMoveY(transform.position.y - 0.125f, 2f))
                                .Append(transform.DOMoveY(transform.position.y, 2f));
                        }
                    }
                    return;
                }
                strartPosition = new Vector3(strartPosition.x, strartPosition.y - 1f, strartPosition.z);
            }
        }

        transform.position = positionAfterMovement;
    }

    public static (Vector3 newPosition, LevelInfo nextTile) GetPosition(Transform transform, Vector3 MovementDirection, float GameSpeed, Vector3 positionOnPreviousFrame, LevelInfo[,] LevelStructure)
    {
        var positionAfterMovement = transform.position + MovementDirection * GameSpeed * Time.deltaTime;
        if (MovementDirection == Vector3.right)
        {
            var strartPosition = new Vector3((float)Math.Truncate((double)positionOnPreviousFrame.x + 0.001), positionOnPreviousFrame.y, positionOnPreviousFrame.z);
            while (strartPosition.x < positionAfterMovement.x)
            {
                var tile = LevelStructure[(int)strartPosition.x + 1, (int)strartPosition.y];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var stopPosition = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return (stopPosition, tile);
                }
                strartPosition = new Vector3(strartPosition.x + 1f, strartPosition.y, strartPosition.z);
            }
        }
        else if (MovementDirection == Vector3.left)
        {

            var strartPosition = new Vector3((float)Math.Truncate((double)positionOnPreviousFrame.x - 0.001) + 1f, positionOnPreviousFrame.y, positionOnPreviousFrame.z);
            while (strartPosition.x > positionAfterMovement.x)
            {
                var tile = LevelStructure[(int)strartPosition.x - 1, (int)strartPosition.y];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var stopPosition = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return (stopPosition, tile);
                }
                strartPosition = new Vector3(strartPosition.x - 1f, strartPosition.y, strartPosition.z);
            }
        }
        else if (MovementDirection == Vector3.up)
        {
            var strartPosition = new Vector3(positionOnPreviousFrame.x, (float)Math.Truncate((double)positionOnPreviousFrame.y + 0.001f), positionOnPreviousFrame.z);
            while (strartPosition.y < positionAfterMovement.y)
            {
                var tile = LevelStructure[(int)strartPosition.x, (int)strartPosition.y + 1];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var stopPosition = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return (stopPosition, tile);
                }
                strartPosition = new Vector3(strartPosition.x, strartPosition.y + 1f, strartPosition.z);
            }
        }

        else if (MovementDirection == Vector3.down)
        {
            var strartPosition = new Vector3(positionOnPreviousFrame.x, (float)Math.Truncate((double)positionOnPreviousFrame.y - 0.001) + 1f, positionOnPreviousFrame.z);
            while (strartPosition.y > positionAfterMovement.y)
            {
                var tile = LevelStructure[(int)strartPosition.x, (int)strartPosition.y - 1];
                var tileTipe = tile.TileType;
                if (tileTipe == TileType.Wall)
                {
                    var stopPosition = new Vector3(strartPosition.x, strartPosition.y, strartPosition.z);
                    return (stopPosition, tile);
                }

                strartPosition = new Vector3(strartPosition.x, strartPosition.y - 1f, strartPosition.z);
            }
        }
        return (positionAfterMovement, null);
    }

    public static bool CheckSpikesCollision(LevelInfo tile, Vector3 movementDirection)
    {
        var result = false;
        var spikesOnWall = (Dictionary<SpikeType, bool>)tile.Options;

        if (movementDirection == Vector3.left)
        {
            result = spikesOnWall[SpikeType.Right];
        }
        else if (movementDirection == Vector3.right)
        {
            result = spikesOnWall[SpikeType.Left];
        }
        else if (movementDirection == Vector3.up)
        {
            result = spikesOnWall[SpikeType.Bottom];
        }
        else if (movementDirection == Vector3.down)
        {
            result = spikesOnWall[SpikeType.Top];
        };

        return result;
    }

    public static List<Sequence> DoWallCollisionAnimation(Vector3 movementDirection, Transform gameObjectTransform, float wallCollisionAnimationSpeed)
    {
        var result = new List<Sequence>();

        if (movementDirection == Vector3.right)
        {
            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOScaleX(0.75f, wallCollisionAnimationSpeed))
                    .Append(gameObjectTransform.DOScaleX(1f, wallCollisionAnimationSpeed))
                );

            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x + 0.125f, wallCollisionAnimationSpeed))
                    .Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x, wallCollisionAnimationSpeed))
                );

        }
        else if (movementDirection == Vector3.left)
        {
            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOScaleX(0.75f, wallCollisionAnimationSpeed))
                    .Append(gameObjectTransform.DOScaleX(1f, wallCollisionAnimationSpeed))
                );

            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x - 0.125f, wallCollisionAnimationSpeed))
                    .Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x, wallCollisionAnimationSpeed))
                );

        }
        else if (movementDirection == Vector3.up)
        {
            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOScaleY(0.75f, wallCollisionAnimationSpeed))
                    .Append(gameObjectTransform.DOScaleY(1f, wallCollisionAnimationSpeed))
                );

            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y + 0.125f, wallCollisionAnimationSpeed))
                    .Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y, wallCollisionAnimationSpeed))
                );

        }
        else if (movementDirection == Vector3.down)
        {
            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOScaleY(0.75f, wallCollisionAnimationSpeed))
                    .Append(gameObjectTransform.DOScaleY(1f, wallCollisionAnimationSpeed))
                );

            result.Add(
                DOTween.Sequence().Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y - 0.125f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y, wallCollisionAnimationSpeed))
                );
        }

        return result;
    }
}
