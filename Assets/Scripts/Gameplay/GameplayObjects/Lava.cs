using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private float seconds;

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
        seconds = 0f;
    }
}
