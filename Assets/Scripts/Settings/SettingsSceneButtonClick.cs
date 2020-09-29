using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsSceneButtonClick : MonoBehaviour
{

    private GameObject soundVolumeSlider;
    void Start()
    {
        soundVolumeSlider = GameObject.Find("VolumeSlider");
        var sliderComponent = soundVolumeSlider.GetComponent<Slider>();
        sliderComponent.value = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
    }

    public void OnCancelButtonClick()
    {
        SceneManager.LoadScene(Globals.MainMenuSceneName);

    }

    public void OnSaveButtonClick()
    {
        var slider = soundVolumeSlider.GetComponent<Slider>();
        SavingGlobalSettings.Settings.GameSettings.SoundVolume = slider.value;
        SceneManager.LoadScene(Globals.MainMenuSceneName);

    }
}
