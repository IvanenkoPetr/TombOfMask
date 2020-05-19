using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        var camera = ConstractorUI.MainCamera; 
        camera.transform.SetParent(null);
        Destroy(other.gameObject);

    }
}
