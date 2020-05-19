using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 0.1f;
    public LevelInfo[,] LevelStructure;

    Vector3 MovementDirection = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (MovementDirection == Vector3.zero)
        {


            //float h = Input.GetAxisRaw("Horizontal");
            //float v = Input.GetAxisRaw("Vertical");

            //if (h == 1f)
            //{
            //    MovementDirection = Vector3.right;
            //    //nextTile = LevelStructure[currentX + 1, currentY];
            //}
            //else if (h == -1f)
            //{
            //    MovementDirection = Vector3.left;
            //    //nextTile = LevelStructure[currentX - 1, currentY];
            //}
            //else if (v == 1f)
            //{
            //    MovementDirection = Vector3.up;
            //    //nextTile = LevelStructure[currentX, currentY+1];
            //}
            //else if (v == -1f)
            //{
            //    MovementDirection = Vector3.down;
            //    //nextTile = LevelStructure[currentX, currentY - 1];
            //}

            MovementDirection = SwipeDetector.GetDirectionFromSwipes();
        }

        var currentPosition = transform.position;
        var currentX = (int)Math.Round(currentPosition.x);
        var currentY = (int)Math.Round(currentPosition.y);

        LevelInfo nextTile = null;
        if (MovementDirection == Vector3.right)
        {
            if(Math.Abs((float)Math.Round(currentPosition.x)-currentPosition.x) <= 0.01)
            {
                nextTile = LevelStructure[currentX + 1, currentY];
            }
        }
        else if(MovementDirection == Vector3.left)
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

        if ((nextTile == null || nextTile.TileType != TileType.Wall) && MovementDirection != Vector3.zero)
        {
            transform.position = transform.position + MovementDirection * Speed;
        }
        else
        {
            MovementDirection = Vector3.zero;
        }
        
    }
}
