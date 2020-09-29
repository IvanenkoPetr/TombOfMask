using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionPrefabs : MonoBehaviour
{
    public GameObject LevelButton;
}


public static class LevelSelectionPrefabsSettings
{
    private static LevelSelectionPrefabs prefabs;
    public static LevelSelectionPrefabs Prefabs => prefabs;

    static LevelSelectionPrefabsSettings()
    {
        var prefabObject = Resources.Load<GameObject>("Prefabs/LevelSelectionPrefabs");
        prefabs = prefabObject.GetComponent<LevelSelectionPrefabs>();
    }
}