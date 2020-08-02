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

    // Start is called before the first frame update
    void Start()
    {
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
                nextTile = GetNextTile(currentPosition, MovementDirection);
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

        nextTile = GetNextTile(currentPosition, MovementDirection);
        if ((nextTile == null || nextTile.TileType != TileType.Wall) && MovementDirection != Vector3.zero)
        {
            transform.position = transform.position + MovementDirection * GameSpeed * Time.deltaTime;
        }
        else
        {
            MovementDirection = Vector3.zero;
        }
    }

    private void SimpleMovement()
    {

        var movementDirectionFromInput = SwipeDetector.GetDirectionFromSwipes();
        if (MovementDirection == Vector3.zero)
        {
            MovementDirection = movementDirectionFromInput;
        }

        var currentPosition = transform.position;
        LevelInfo nextTile = GetNextTile(currentPosition, MovementDirection);

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

    private LevelInfo GetNextTile(Vector3 currentPosition, Vector3 direction)
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

            if (nextTileCoordinate.x - currentPosition.x <= precision)
            {
                nextTile = LevelStructure[(int)nextTileCoordinate.x + 1, currentY];
                nextTileCoordinate = Vector3.zero;
            }
        }
        else if (direction == Vector3.left)
        {

            if (nextTileCoordinate == Vector3.zero)
            {
                nextTileCoordinate = new Vector3((float)Math.Round(currentPosition.x) - 1, currentPosition.y);
            }

            if (currentPosition.x - nextTileCoordinate.x <= precision)
            {
                nextTile = LevelStructure[(int)nextTileCoordinate.x - 1, currentY];
                nextTileCoordinate = Vector3.zero;
            }
        }
        else if (direction == Vector3.up)
        {

            //var nextCoordinate = (float)(int)currentPosition.y + 1;
            if (nextTileCoordinate == Vector3.zero)
            {
                nextTileCoordinate = new Vector3(currentPosition.x, (float)Math.Round(currentPosition.y) + 1);
            }

            if (nextTileCoordinate.y - currentPosition.y <= precision)
            {
                nextTile = LevelStructure[currentX, (int)nextTileCoordinate.y + 1];
                nextTileCoordinate = Vector3.zero;
            }
        }
        else if (direction == Vector3.down)
        {
            if (nextTileCoordinate == Vector3.zero)
            {
                nextTileCoordinate = new Vector3((float)(int)currentPosition.x, (float)Math.Round(currentPosition.y) - 1);
            }

            if (currentPosition.y - nextTileCoordinate.y <= precision)
            {
                nextTile = LevelStructure[currentX, (int)nextTileCoordinate.y - 1];
                nextTileCoordinate = Vector3.zero;
            }
        }

        return nextTile;
    }

}
