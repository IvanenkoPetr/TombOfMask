﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionButtons : MonoBehaviour
{
    public void OnLevelButtonClick(string level)
    {
        var newLevelStructure = ResourcesManagment.LoadLevelStructure($"Levels/Level{level}Structure");
        Globals.SetLevelStructure(newLevelStructure);
        SceneManager.LoadScene(Globals.GameplaySceneName);
    }
}
