using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectionButtonInfo : MonoBehaviour
{
    public int Level { get; set; }

    public void OnLevelButtonClick()
    {
        var newLevelStructure = ResourcesManagment.LoadLevelStructure($"Levels/Level{Level}Structure");
        Globals.SetLevelStructure(newLevelStructure);
        SceneManager.LoadScene(Globals.GameplaySceneName);
    }


}
