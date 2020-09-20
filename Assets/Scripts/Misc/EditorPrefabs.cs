using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class EditorPrefabs : MonoBehaviour
{
    public GameObject TileButton;
}

public static class EditorPrefabsSettings
{
    private static EditorPrefabs prefabs;
    public static EditorPrefabs Prefabs => prefabs;

    static EditorPrefabsSettings()
    {
        var prefabObject  = Resources.Load<GameObject>("Prefabs/EditorsPrefabs");
        prefabs = prefabObject.GetComponent<EditorPrefabs>();
    }
}
