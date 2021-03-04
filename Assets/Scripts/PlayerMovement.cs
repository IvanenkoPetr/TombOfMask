using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 MovementDirection = Vector3.zero;
    private float PlayerSpeed;
    private bool IsCanChangeDirectionInMovement;
    private Vector3 turningPoint = Vector3.zero;
    private Vector3 turningDirection;
    private Vector3 positionOnPreviousFrame;
    private float wallCollisionAnimationSpeed = 0.2f;
    private List<Sequence> animationSequence = new List<Sequence>();
    private Vector3 pointWhereLevelWasUpdated;

    public GameObject Lava;

    public event Action<GameObject> WallCollisionEvent;
    public event Action<GameObject> PlayerMovedUp;

    // Start is called before the first frame update
    void Start()
    {
        positionOnPreviousFrame = transform.position;
        pointWhereLevelWasUpdated = transform.position;
        PlayerSpeed = SavingGlobalSettings.Settings.RemoteSettings.PlayerSpeed;
        IsCanChangeDirectionInMovement = SavingGlobalSettings.Settings.RemoteSettings.IsCanChangeDirectionInMovement;

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

        //var levelStructure 
        if(transform.position.x > Globals.ArcadeCurrentLevelPartPosition[Vector2.left]
            && transform.position.x < Globals.ArcadeCurrentLevelPartPosition[Vector2.right] 
            && transform.position.y < Globals.ArcadeCurrentLevelPartPosition[Vector2.up]
            && transform.position.y > Globals.ArcadeCurrentLevelPartPosition[Vector2.down])
        {            
            PlayerMovedUp(gameObject);
            pointWhereLevelWasUpdated = transform.position;
        }

        if(Lava != null)
        {
            var height = Lava.GetComponent<SpriteRenderer>().bounds.size.y/2;
            if(Lava.transform.position.y + height > transform.position.y)
            {
                //var camera = GamePlayPrefabsSettings.Prefabs.MainCamera;
                //camera.transform.SetParent(null);
                GameplaySettings.MainCamera.transform.SetParent(null);
                Destroy(gameObject);
            }

        }
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

        var newPositionInfo = MovementUtils.GetPosition(transform, MovementDirection, PlayerSpeed, positionOnPreviousFrame);

        transform.position = newPositionInfo.newPosition;
        if (newPositionInfo.nextTile?.TileType == TileType.Wall)
        {
            if (MovementUtils.CheckSpikesCollision(newPositionInfo.nextTile, MovementDirection))
            {
                //var camera = GamePlayPrefabsSettings.Prefabs.MainCamera;
                //camera.transform.SetParent(null);
                GameplaySettings.MainCamera.transform.SetParent(null);
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

        var newPositionInfo = MovementUtils.GetPosition(transform, MovementDirection, PlayerSpeed, positionOnPreviousFrame);

        transform.position = newPositionInfo.newPosition;
        if (newPositionInfo.nextTile?.TileType == TileType.Wall)
        {
            if (MovementUtils.CheckSpikesCollision(newPositionInfo.nextTile, MovementDirection))
            {
                var camera = GamePlayPrefabsSettings.Prefabs.MainCamera;
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
