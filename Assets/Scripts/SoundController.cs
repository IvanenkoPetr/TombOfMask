using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField]
    public AudioClip PlayerWallCollisionSound;
  
    [SerializeField]
    public AudioClip EnemyWallCollisionSound;

    [SerializeField]
    public AudioClip CollectibleCollisionSound;

    [SerializeField]
    public AudioClip HatchChangeStatusSound;

    [SerializeField]
    public AudioClip StarCollisionSound;

    public void PlayerWallCollisionEvent(GameObject player)
    {
        var audioSource = player.GetComponent<AudioSource>();
        audioSource.PlayOneShot(PlayerWallCollisionSound);
    }

    public void EnemyWallCollisionEvent(GameObject enemy)
    {
        if (IsOnScreenPlusOneCell(enemy.transform.position))
        {
            var audioSource = enemy.GetComponent<AudioSource>();
            audioSource.PlayOneShot(EnemyWallCollisionSound);
        }
    }

    public void CollectibleCollisionEvent(GameObject collectible)
    {
        var audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(CollectibleCollisionSound);
 
    }

    public void StarCollisionEvent(GameObject collectible)
    {
        var audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(StarCollisionSound);

    }

    public void HatchChangeStatusEvent(GameObject hatch)
    {
        if (IsOnScreenPlusOneCell(hatch.transform.position))
        {
            var audioSource = hatch.GetComponent<AudioSource>();
            audioSource.PlayOneShot(HatchChangeStatusSound);
        }
    }


    private float DynamicHorizontalScreenBorderPlusOneCell
    {
        get
        {
            return GameplaySettings.MainCamera.GetComponent<Camera>().orthographicSize 
                * GameplaySettings.MainCamera.GetComponent<Camera>().aspect + 1;
        }
    }

    private float DynamicVerticalScreenBorderDistancePlusOneCell
    {
        get
        {
            return GameplaySettings.MainCamera.GetComponent<Camera>().orthographicSize + 1;
        }
    }

    private bool IsOnScreenPlusOneCell(Vector3 position)
    {
        return (Mathf.Abs(GameplaySettings.MainCamera.transform.position.x - position.x) <= DynamicHorizontalScreenBorderPlusOneCell &&
                Mathf.Abs(GameplaySettings.MainCamera.transform.position.y - position.y) <= DynamicVerticalScreenBorderDistancePlusOneCell);
    }
}
