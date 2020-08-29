using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwipeMovement : MonoBehaviour
{
    public LevelInfo[,] LevelStructure;
    Vector3 MovementDirection;
    private float EnemySpeed;
    public MovementAxis MovementAxis;
    private Vector3 nextTileCoordinate;
    private Vector3 positionOnPreviousFrame;
    public bool isStoped = false;
    private float wallCollisionAnimationSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        positionOnPreviousFrame = transform.position;
        EnemySpeed = ConstractorUI.MainGame.GetComponent<Settings>().EnemySpeed;
        ChooseNextDirection(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        if(MovementAxis == MovementAxis.None || isStoped)
        {
            return;
        }

        var movementBeforeStop = MovementDirection;
        var newPositionInfo = MovementUtils.GetPosition(transform, MovementDirection, EnemySpeed, positionOnPreviousFrame, LevelStructure);

        if (newPositionInfo.nextTile?.TileType == TileType.Wall)
        {
            MovementUtils.DoWallCollisionAnimation(MovementDirection, transform, wallCollisionAnimationSpeed);
            ChooseNextDirection(movementBeforeStop);

        }
        else
        {
            transform.position = newPositionInfo.newPosition;
        }

        positionOnPreviousFrame = transform.position;

    }

    private void ChooseNextDirection(Vector3 movementBeforeStop)
    {
        if (MovementAxis == MovementAxis.Vertical)
        {
            if (movementBeforeStop == Vector3.up)
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
            if (movementBeforeStop == Vector3.left)
            {
                MovementDirection = Vector3.right;
            }
            else
            {
                MovementDirection = Vector3.left;
            }
        }else if (MovementAxis == MovementAxis.Random)
        {
            var currentPosition = transform.position;
            var x = (int)currentPosition.x;
            var y = (int)currentPosition.y;

            var allDirection = new[] {
                new { x = x - 1, y, direction = Vector3.left },
                new { x = x + 1, y, direction = Vector3.right },
                new { x = x, y = y - 1 , direction = Vector3.down },
                new { x = x, y = y + 1 , direction = Vector3.up }};

            var possibleDirection = new List<int>();
            foreach(var elem in allDirection.Select((a,b) => new { index = b, direction = a }))
            {
                try
                {
                    var tileInfo = LevelStructure[elem.direction.x, elem.direction.y];
                    if (tileInfo.TileType != TileType.Wall)
                    {
                        possibleDirection.Add(elem.index);
                    }
                }catch(Exception e)
                {

                }
            }

            System.Random rnd = new System.Random();
            var randomValue = rnd.Next(possibleDirection.Count);
            var newIndexDirection = possibleDirection[randomValue];
            MovementDirection = allDirection[newIndexDirection].direction;
        }
    }
}

public enum MovementAxis
{
    Vertical,
    Horizontal,
    Random,
    None
}
