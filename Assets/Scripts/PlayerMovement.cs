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
    public bool isStoped = false;

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

        if (MovementDirection == Vector3.zero || isStoped)
        {
            return;
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
                MovementUtils.DoWallCollisionAnimation(MovementDirection, transform, wallCollisionAnimationSpeed);
                MovementDirection = Vector3.zero;
            }
        }

        positionOnPreviousFrame = transform.position;
    }

    private void DoWallCollisionAnimation(Vector3 movementDirection, Transform gameObjectTransform)
    {
        var gameObject = gameObjectTransform.gameObject;
        var player = gameObject.GetComponent<PlayerMovement>();
        TweenCallback callback = null;
        if (player != null)
        {
            player.isStoped = true;
            callback = () => player.isStoped = false;
        }
        else
        {
            var enemy = gameObject.GetComponent<SwipeMovement>();
            enemy.isStoped = true;
            callback = () => enemy.isStoped = false;
        }

        isStoped = true;

        if (movementDirection == Vector3.right)
        {

            DOTween.Sequence().Append(gameObjectTransform.DOScaleX(0.75f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOScaleX(1f, wallCollisionAnimationSpeed)).AppendCallback(callback);

            DOTween.Sequence().Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x + 0.125f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x, wallCollisionAnimationSpeed));

        }
        else if (movementDirection == Vector3.left)
        {
            DOTween.Sequence().Append(gameObjectTransform.DOScaleX(0.75f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOScaleX(1f, wallCollisionAnimationSpeed)).AppendCallback(callback);

            DOTween.Sequence().Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x - 0.125f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOMoveX(gameObjectTransform.position.x, wallCollisionAnimationSpeed));

        }
        else if (movementDirection == Vector3.up)
        {

            DOTween.Sequence().Append(gameObjectTransform.DOScaleY(0.75f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOScaleY(1f, wallCollisionAnimationSpeed)).AppendCallback(callback);

            DOTween.Sequence().Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y + 0.125f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y, wallCollisionAnimationSpeed));

        }
        else if (movementDirection == Vector3.down)
        {
            DOTween.Sequence().Append(gameObjectTransform.DOScaleY(0.75f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOScaleY(1f, wallCollisionAnimationSpeed)).AppendCallback(callback);

            DOTween.Sequence().Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y - 0.125f, wallCollisionAnimationSpeed))
                .Append(gameObjectTransform.DOMoveY(gameObjectTransform.position.y, wallCollisionAnimationSpeed));
        }
    }
}
