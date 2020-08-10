using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        ChooseNextDirection();
    }

    // Update is called once per frame
    void Update()
    {
        if(MovementAxis == MovementAxis.None)
        {
            return;
        }

        var currentPosition = transform.position;

        LevelInfo nextTile = MovementUtils.GetNextTile(currentPosition, MovementDirection, ref nextTileCoordinate, LevelStructure);

        if ((nextTile == null || nextTile.TileType != TileType.Wall))
        {
            transform.position = transform.position + MovementDirection * GameSpeed * Time.deltaTime;
        }
        else
        {
            ChooseNextDirection();

        }

    }

    private void ChooseNextDirection()
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
