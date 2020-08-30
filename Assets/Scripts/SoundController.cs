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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
            return Camera.main.orthographicSize * Camera.main.aspect + 1;
        }
    }

    private float DynamicVerticalScreenBorderDistancePlusOneCell
    {
        get
        {
            return Camera.main.orthographicSize + 1;
        }
    }

    private bool IsOnScreenPlusOneCell(Vector3 position)
    {
        return (Mathf.Abs(Camera.main.transform.position.x - position.x) <= DynamicHorizontalScreenBorderPlusOneCell &&
                Mathf.Abs(Camera.main.transform.position.y - position.y) <= DynamicVerticalScreenBorderDistancePlusOneCell);
    }
}
