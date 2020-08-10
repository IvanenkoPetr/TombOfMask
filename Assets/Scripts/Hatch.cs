using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Hatch : MonoBehaviour
{
    public HatchState CurrentState;
    public float ChangeStateInSeconds;
    private float miliseconds;
    // Start is called before the first frame update
    void Start()
    {
        ChangeState();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        miliseconds += 0.02f;
        if (miliseconds >= 2f)
        {
            ChangeState();
            miliseconds = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerMovement>();
        if (player != null)
        {
            if(CurrentState == HatchState.Opened)
            { 
            var camera = ConstractorUI.MainCamera;
            camera.transform.SetParent(null);
            Destroy(other.gameObject);
            }
        }
    }

    private void ChangeState()
    {
        var image = gameObject.GetComponent<SpriteRenderer>();
        if (CurrentState == HatchState.Closed)
        {
            CurrentState = HatchState.Opened;
            image.color = new Color(0, 255, 0);
        }else if (CurrentState == HatchState.Opened)
        {
            CurrentState = HatchState.Closed;
            image.color = new Color(255, 0, 0);
        }
    }
}

public enum HatchState
{
    Opened,
    Closed
}
