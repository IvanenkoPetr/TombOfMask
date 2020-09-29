using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public event Action<GameObject> CollectibleCollisionEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            CollectibleCollisionEvent(gameObject);
            Destroy(gameObject);
        }
    }
}
