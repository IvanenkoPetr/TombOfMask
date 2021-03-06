﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hatch : MonoBehaviour
{
    public HatchState CurrentState;
    public float ChangeStateInSeconds;
    private float seconds;
    private float timeToSwitchState = 2f;
    public event Action<GameObject> HatchChangeStateEvent;
    // Start is called before the first frame update
    void Start()
    {
        timeToSwitchState = SavingGlobalSettings.Settings.RemoteSettings.TimeToSwitchHatchState;
        ChangeState();
    }


    void FixedUpdate()
    {
        seconds += 0.02f;
        if (seconds >= timeToSwitchState)
        {
            ChangeState();
            HatchChangeStateEvent(gameObject);
            seconds = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if (Globals.IsEditorScene)
            {
                if (CurrentState == HatchState.Opened)
                {
                    var camera = GameplaySettings.MainCamera;
                    camera.transform.SetParent(null);
                    Destroy(other.gameObject);
                }
            }
            else
            {
                if (CurrentState == HatchState.Opened)
                {
                    SceneManager.LoadScene(Globals.LevelSelectionSceneName);
                }                
            }
        }
    }

    private void ChangeState()
    {
        var image = gameObject.GetComponent<SpriteRenderer>();
        if (CurrentState == HatchState.Closed)
        {
            CurrentState = HatchState.Opened;
            image.color = new Color(255, 0, 0);
        }else if (CurrentState == HatchState.Opened)
        {
            CurrentState = HatchState.Closed;
            image.color = new Color(0, 255, 0);
        }
    }
}

public enum HatchState
{
    Opened,
    Closed
}
