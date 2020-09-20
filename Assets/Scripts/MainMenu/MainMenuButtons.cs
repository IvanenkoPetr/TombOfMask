using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void OnEditorButtonClick()
    {
        SceneManager.LoadScene(Globals.EditorSceneName);
    }

    public void OnStartGameButtonClick()
    {
        SceneManager.LoadScene(Globals.LevelSelectionSceneName);
    }

    public void OnSettingsButtonClick()
    {
        SceneManager.LoadScene(Globals.SettingsSceneName);
    }
}
