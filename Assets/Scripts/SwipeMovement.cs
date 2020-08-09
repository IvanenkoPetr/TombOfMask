using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeMovement : MonoBehaviour
{
    public LevelInfo[,] LevelStructure;
    Vector3 MovementDirection;
    private float GameSpeed;
    public MovementAxis MovementAxis;
    private Vector3 nextTileCoordinate;

    // Start is called before the first frame update
    void Start()
    {
        GameSpeed = ConstractorUI.MainGame.GetComponent<Settings>().GameSpeed;
        if(MovementAxis == MovementAxis.Vertical)
        {
            MovementDirection = Vector3.down;
        }
        else if (MovementAxis == MovementAxis.Horizontal)
        {
            MovementDirection = Vector3.left;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(MovementAxis == MovementAxis.None)
        {
            return;
        }

        var currentPosition = transform.position;

        LevelInfo nextTile = GetNextTile(currentPosition);
        nextTile = MovementUtils.GetNextTile(currentPosition, MovementDirection, ref nextTileCoordinate, LevelStructure);

        if ((nextTile == null || nextTile.TileType != TileType.Wall))
        {
            transform.position = transform.position + MovementDirection * GameSpeed * Time.deltaTime;
        }
        else
        {
            if (MovementAxis == MovementAxis.Vertical)
            {
                if (MovementDirection == Vector3.up)
                {
                    MovementDirection = Vector3.down;
                }
                else
                {
                    MovementDirection = Vector3.up;
                }
            }
            else if (MovementAxis == MovementAxis.Horizontal)
            {
                if (MovementDirection == Vector3.left)
                {
                    MovementDirection = Vector3.right;
                }
                else
                {
                    MovementDirection = Vector3.left;
                }
            }

        }

    }

    private LevelInfo GetNextTile(Vector3 currentPosition)
    {
        var currentX = (int)Math.Round(currentPosition.x);
        var currentY = (int)Math.Round(currentPosition.y);

        LevelInfo nextTile = null;
        if (MovementDirection == Vector3.right)
        {
            if (Math.Abs((float)Math.Round(currentPosition.x) - currentPosition.x) <= 0.01)
            {
                nextTile = LevelStructure[currentX + 1, currentY];
            }
        }
        else if (MovementDirection == Vector3.left)
        {
            if (Math.Abs((float)Math.Round(currentPosition.x) - currentPosition.x) <= 0.01)
            {
                nextTile = LevelStructure[currentX - 1, currentY];
            }
        }
        else if (MovementDirection == Vector3.up)
        {
            if (Math.Abs((float)Math.Round(currentPosition.y) - currentPosition.y) <= 0.01)
            {
                nextTile = LevelStructure[currentX, currentY + 1];
            }
        }
        else if (MovementDirection == Vector3.down)
        {
            if (Math.Abs((float)Math.Round(currentPosition.y) - currentPosition.y) <= 0.01)
            {
                nextTile = LevelStructure[currentX, currentY - 1];
            }
        }

        return nextTile;
    }
}

public enum MovementAxis
{
    Vertical,
    Horizontal,
    Random,
    None
}
