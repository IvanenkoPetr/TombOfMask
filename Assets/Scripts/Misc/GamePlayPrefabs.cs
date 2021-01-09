using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayPrefabs : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Player;
    public GameObject Enemy;
    public GameObject Collectible;
    public GameObject Hatch;
    public GameObject Star;
    public GameObject Exit;
    public GameObject Lava;

    public GameObject SoundController;
    public GameObject MainCamera;
    public GameObject Settings;
}


public static class GamePlayPrefabsSettings
{
    private static GamePlayPrefabs prefabs;
    public static GamePlayPrefabs Prefabs => prefabs;

    static GamePlayPrefabsSettings()
    {
        var prefabObject = Resources.Load<GameObject>("Prefabs/GameplayPrefabs");
        prefabs = prefabObject.GetComponent<GamePlayPrefabs>();
    }
}
