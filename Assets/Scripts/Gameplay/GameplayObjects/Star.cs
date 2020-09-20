using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public event Action<GameObject> CollectibleCollisionEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Destroy(gameObject);
            CollectibleCollisionEvent(gameObject);
        }
    }
}
