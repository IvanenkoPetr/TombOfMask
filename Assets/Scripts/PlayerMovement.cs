using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LevelInfo[,] LevelStructure;
    Vector3 MovementDirection = Vector3.zero;
    private float GameSpeed;
    private bool IsCanChangeDirectionInMovement;
    private LevelInfo turningPoint;
    private Vector3 turningDirection;
    private Vector3 MovementDirectionUntilTurn;
    private Vector3 nextTileCoordinate;
    private Vector3 strartPosition;
    private Vector3 positionOnPreviousFrame;

    // Start is called before the first frame update
    void Start()
    {
        positionOnPreviousFrame = transform.position;
        GameSpeed = ConstractorUI.MainGame.GetComponent<Settings>().GameSpeed;
        IsCanChangeDirectionInMovement = ConstractorUI.MainGame.GetComponent<Settings>().IsCanChangeDirectionInMovement;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsCanChangeDirectionInMovement)
        {
            SimpleMovement();
        }
        else
        {
            MovementWithChangeDirection();
        }
    }

    private void MovementWithChangeDirection()
    {
        LevelInfo nextTile = null;
        var currentPosition = transform.position;

        var movementDirectionFromInput = SwipeDetector.GetDirectionFromSwipes();
        if (movementDirectionFromInput != Vector3.zero)
        {
            if (MovementDirection == Vector3.zero)
            {
                MovementDirection = movementDirectionFromInput;
            }
            else
            {
                nextTile = MovementUtils.GetNextTile(currentPosition, MovementDirection, ref nextTileCoordinate, LevelStructure); 
                MovementDirectionUntilTurn = MovementDirection;
                turningPoint = nextTile;
                turningDirection = movementDirectionFromInput;
            }
        }

        if (turningPoint != null)
        {
            var distance = Math.Sqrt(Math.Pow((double)(currentPosition.x - turningPoint.x), 2.0) + Math.Pow((double)(currentPosition.y - turningPoint.y), 2.0));
            if (distance <= 0.2)
            {
                MovementDirection = turningDirection;
                turningPoint = null;
                turningDirection = Vector3.zero;
            }
        }
        
        nextTile = MovementUtils.GetNextTile(currentPosition, MovementDirection, ref nextTileCoordinate, LevelStructure);

        if ((nextTile == null || nextTile.TileType != TileType.Wall) && MovementDirection != Vector3.zero)
        {
            transform.position = transform.position + MovementDirection * GameSpeed * Time.deltaTime;
        }
        else
        {
            MovementDirection = Vector3.zero;
            transform.position = new Vector3((float)Math.Round(transform.position.x),
                (float)Math.Round(transform.position.y), (float)Math.Round(transform.position.z));
        }
    }

    private void SimpleMovement()
    {

        var movementDirectionFromInput = SwipeDetector.GetDirectionFromSwipes();
        if (MovementDirection == Vector3.zero)
        {
            MovementDirection = movementDirectionFromInput;
        }

        if (MovementDirection == Vector3.zero)
        {
            return;
        }

        MovementUtils.SetPosition(transform, ref MovementDirection, GameSpeed, positionOnPreviousFrame, LevelStructure);
        positionOnPreviousFrame = transform.position;
    }

    private void SetPosition()
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
            var strartPosition = new Vector3(positionOnPreviousFrame.x,(float)Math.Truncate((double)positionOnPreviousFrame.y - 0.001) + 1f,  positionOnPreviousFrame.z);
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
