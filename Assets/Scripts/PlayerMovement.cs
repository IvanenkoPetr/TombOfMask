using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LevelInfo[,] LevelStructure;
    Vector3 MovementDirection = Vector3.zero;
    private float PlayerSpeed;
    private bool IsCanChangeDirectionInMovement;
    private Vector3 turningPoint = Vector3.zero;
    private Vector3 turningDirection;
    private Vector3 MovementDirectionUntilTurn;
    private Vector3 nextTileCoordinate;
    private Vector3 strartPosition;
    private Vector3 positionOnPreviousFrame;
    private float wallCollisionAnimationSpeed = 0.2f;
    private List<Sequence> animationSequence = new List<Sequence>();

    public event Action<GameObject> WallCollisionEvent;

    // Start is called before the first frame update
    void Start()
    {
        positionOnPreviousFrame = transform.position;
        PlayerSpeed = ConstractorUI.MainGame.GetComponent<Settings>().PlayerSpeed;
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

    private void MovementWithChangeDirection1()
    {
        var movementDirectionFromInput = SwipeDetector.GetDirectionFromSwipes();
        if (movementDirectionFromInput != Vector3.zero)
        {
            if (animationSequence.Any())
            {
                animationSequence.ForEach(a => a.Kill());
                animationSequence.Clear();

                transform.localScale = Vector3.one;
                transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            }

            if (MovementDirection == Vector3.zero)
            {
                MovementDirection = movementDirectionFromInput;
            }
            else
            {
                turningDirection = movementDirectionFromInput;
                if (MovementDirection == Vector3.left)
                {
                    turningPoint = new Vector3(Mathf.Floor(transform.position.x), transform.position.y, transform.position.z);
                }
                else if (MovementDirection == Vector3.right)
                {
                    turningPoint = new Vector3(Mathf.Ceil(transform.position.x), transform.position.y, transform.position.z);
                }
                else if (MovementDirection == Vector3.down)
                {
                    turningPoint = new Vector3(transform.position.x, Mathf.Floor(transform.position.y), transform.position.z);
                }
                else if (MovementDirection == Vector3.up)
                {
                    turningPoint = new Vector3(transform.position.x, Mathf.Ceil(transform.position.y), transform.position.z);
                }
            }
        }

        var newPositionInfo = MovementUtils.GetPosition(transform, MovementDirection, PlayerSpeed, positionOnPreviousFrame, LevelStructure);
        transform.position = newPositionInfo.newPosition;
        if (newPositionInfo.nextTile?.TileType == TileType.Wall)
        {
            if(turningDirection != Vector3.zero && MovementDirection != turningDirection)
            {
                MovementDirection = turningDirection;
            }
            else
            {
                
                if (MovementUtils.CheckSpikesCollision(newPositionInfo.nextTile, MovementDirection))
                {
                    var camera = ConstractorUI.MainCamera;
                    camera.transform.SetParent(null);
                    Destroy(transform.gameObject);
                }
                else
                {
                    animationSequence = MovementUtils.DoWallCollisionAnimation(MovementDirection, transform, wallCollisionAnimationSpeed);
                    animationSequence[0].AppendCallback(() => animationSequence.Clear());

                    MovementDirection = Vector3.zero;
                    WallCollisionEvent(gameObject);
                    MovementDirection = Vector3.zero;
                }
                
            }
            
        }
        else
        {
            if (turningPoint != Vector3.zero)
            {
                var makeTurn = false;
                if (MovementDirection == Vector3.left)
                {
                    if(transform.position.x <= turningPoint.x)
                    {
                        makeTurn = true;
                    }
                }
                else if (MovementDirection == Vector3.right)
                {
                    if (transform.position.x >= turningPoint.x )
                    {
                        makeTurn = true;
                    }
                }
                else if (MovementDirection == Vector3.down)
                {
                    if (transform.position.y <= turningPoint.y)
                    {
                        makeTurn = true;
                    }
                }
                else if (MovementDirection == Vector3.up)
                {
                    if (transform.position.y >= turningPoint.y)
                    {
                        makeTurn = true;
                    }
                }
                if (makeTurn)
                {
                    transform.position = turningPoint;
                    MovementDirection = turningDirection;
                    turningPoint = Vector3.zero;
                    turningDirection = Vector3.zero;
                }
            }
        }
        positionOnPreviousFrame = transform.position;
    }
    
    private void MovementWithChangeDirection()
    {
        var movementDirectionFromInput = SwipeDetector.GetDirectionFromSwipes();
        if(movementDirectionFromInput != Vector3.zero)
        {
            if (MovementDirection != Vector3.zero)
            {
                transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
            }
            MovementDirection = movementDirectionFromInput;
        }


        if (MovementDirection == Vector3.zero)
        {
            return;
        }

        if (animationSequence.Any())
        {
            animationSequence.ForEach(a => a.Kill());
            animationSequence.Clear();

            transform.localScale = Vector3.one;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        }

        var newPositionInfo = MovementUtils.GetPosition(transform, MovementDirection, PlayerSpeed, positionOnPreviousFrame, LevelStructure);

        transform.position = newPositionInfo.newPosition;
        if (newPositionInfo.nextTile?.TileType == TileType.Wall)
        {
            if (MovementUtils.CheckSpikesCollision(newPositionInfo.nextTile, MovementDirection))
            {
                var camera = ConstractorUI.MainCamera;
                camera.transform.SetParent(null);
                Destroy(transform.gameObject);
            }
            else
            {

                animationSequence = MovementUtils.DoWallCollisionAnimation(MovementDirection, transform, wallCollisionAnimationSpeed);
                animationSequence[0].AppendCallback(() => animationSequence.Clear());

                MovementDirection = Vector3.zero;
                WallCollisionEvent(gameObject);
            }
        }

        positionOnPreviousFrame = transform.position;
    }

    private void SimpleMovement()
    {

        if (MovementDirection == Vector3.zero)
        {
            var movementDirectionFromInput = SwipeDetector.GetDirectionFromSwipes();
            MovementDirection = movementDirectionFromInput;
        }

        if (MovementDirection == Vector3.zero)
        {
            return;
        }

        if (animationSequence.Any())
        {
            animationSequence.ForEach(a => a.Kill());
            animationSequence.Clear();

            transform.localScale = Vector3.one;
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        }

        var newPositionInfo = MovementUtils.GetPosition(transform, MovementDirection, PlayerSpeed, positionOnPreviousFrame, LevelStructure);

        transform.position = newPositionInfo.newPosition;
        if (newPositionInfo.nextTile?.TileType == TileType.Wall)
        {
            if (MovementUtils.CheckSpikesCollision(newPositionInfo.nextTile, MovementDirection))
            {
                var camera = ConstractorUI.MainCamera;
                camera.transform.SetParent(null);
                Destroy(transform.gameObject);
            }
            else
            {

                animationSequence = MovementUtils.DoWallCollisionAnimation(MovementDirection, transform, wallCollisionAnimationSpeed);
                animationSequence[0].AppendCallback(() => animationSequence.Clear());

                MovementDirection = Vector3.zero;
                WallCollisionEvent(gameObject);
            }
        }

        positionOnPreviousFrame = transform.position;
    }
}
