using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwipeMovement : MonoBehaviour
{
    Vector3 MovementDirection;
    private float EnemySpeed;
    public MovementAxis MovementAxis;
    private Vector3 positionOnPreviousFrame;
    public bool isStoped = false;
    private float wallCollisionAnimationSpeed = 0.2f;

    public event Action<GameObject> WallCollisionEvent;

    // Start is called before the first frame update
    void Start()
    {
        positionOnPreviousFrame = transform.position;
        EnemySpeed = SavingGlobalSettings.Settings.RemoteSettings.EnemySpeed;
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
        var newPositionInfo = MovementUtils.GetPosition(transform, MovementDirection, EnemySpeed, positionOnPreviousFrame);

        if (newPositionInfo.nextTile?.TileType == TileType.Wall)
        {
            isStoped = true;
            var animationSequence = MovementUtils.DoWallCollisionAnimation(MovementDirection, transform, wallCollisionAnimationSpeed);
            animationSequence[0].AppendCallback(() => isStoped = false);
            WallCollisionEvent(gameObject);
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
            var x = Mathf.RoundToInt(currentPosition.x);
            var y = Mathf.RoundToInt(currentPosition.y); ;

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
                    var tileInfo = Globals.LevelStructure[elem.direction.x, elem.direction.y];
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
