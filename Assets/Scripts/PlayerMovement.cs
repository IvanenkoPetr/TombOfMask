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
    private LevelInfo turningPoint;
    private Vector3 turningDirection;
    private Vector3 MovementDirectionUntilTurn;
    private Vector3 nextTileCoordinate;
    private Vector3 strartPosition;
    private Vector3 positionOnPreviousFrame;
    private float wallCollisionAnimationSpeed = 0.5f;
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
            transform.position = transform.position + MovementDirection * PlayerSpeed * Time.deltaTime;
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
