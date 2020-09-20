using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if (Globals.IsEditorScene)
            {
                var camera = GameplaySettings.MainCamera;
                camera.transform.SetParent(null);
                Destroy(other.gameObject);
            }
            else
            {
                SceneManager.LoadScene(Globals.LevelSelectionSceneName);
            }
        }
    }
}
