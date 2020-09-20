using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public event Action<GameObject> ExitCollisionEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if (Globals.IsEditorScene)
            {
                GameButtons.ClearGameScene();
            }
            else
            {
                ExitCollisionEvent(gameObject);
                SceneManager.LoadScene(Globals.LevelSelectionSceneName);
                
            }

        }
    }
}
