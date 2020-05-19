using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetector : MonoBehaviour
{
    private static Touch fingerPositionDown;
    private static Touch fingerPositionUp;

    public static double MinSwipeDistance = 60.0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public static Vector3 GetDirectionFromSwipes()
    {
        var direction = Vector3.zero;

        IEnumerable<Touch> touches;
        if (Application.isEditor)
        {
            touches = InputHelper.GetTouches();
        }
        else
        {
            touches = Input.touches;
        }

        foreach (var touch in touches)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    fingerPositionDown = touch;
                    break;
                case TouchPhase.Ended:
                    fingerPositionUp = touch;
                    direction = ProcessSwipe(fingerPositionDown, fingerPositionUp);
                    break;
            }
        }

        return direction;

    }

    private static Vector3 ProcessSwipe(Touch fingerTouchDown, Touch fingerTouchUp)
    {
        var direction = Vector3.zero;

        if (fingerTouchDown.fingerId != fingerTouchUp.fingerId)
        {
            return direction;
        }

        var swipeDistance = Math.Sqrt(Math.Pow(fingerTouchDown.position.x - fingerTouchUp.position.x, 2)
            + Math.Pow(fingerTouchDown.position.y - fingerTouchUp.position.y, 2));

        if (swipeDistance < MinSwipeDistance)
        {
            return direction;
        }

        var beginX = fingerTouchDown.position.x;
        var endX = fingerTouchUp.position.x;

        var beginY = fingerTouchDown.position.y;
        var endY = fingerTouchUp.position.y;

        var distanceX = endX - beginX;
        var distanceY = endY - beginY;


        if (Math.Abs(distanceX) > Math.Abs(distanceY))
        {
            if (distanceX > 0)
            {
                direction = Vector3.right;
            }
            else
            {
                direction = Vector3.left;
            }
        }
        else
        {
            if (distanceY > 0)
            {
                direction = Vector3.up;
            }
            else
            {
                direction = Vector3.down;
            }
        }

        return direction;
    }
}
