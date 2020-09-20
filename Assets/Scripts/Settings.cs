using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Settings : MonoBehaviour
{
    public float GameSpeed = 1f;
    public float PlayerSpeed = 1f;
    public float EnemySpeed = 1f;
    public float TimeToSwitchHatchState = 1f;
    public float MainCameraSize = 1f;
    public bool IsCanChangeDirectionInMovement = false; 

    // Start is called before the first frame update
    void Start()
    {
        RemoteSettings_Updated();
        //RemoteSettings.Updated += RemoteSettings_Updated;
        //Debug.Log("Settings");
    }

    private void RemoteSettings_Updated()
    {
        GameSpeed = RemoteSettings.GetFloat("GameSpeed", GameSpeed);
        PlayerSpeed = RemoteSettings.GetFloat("PlayerSpeed", PlayerSpeed);
        EnemySpeed = RemoteSettings.GetFloat("EnemySpeed", EnemySpeed);
        TimeToSwitchHatchState = RemoteSettings.GetFloat("TimeToSwitchHatchState", TimeToSwitchHatchState);
        MainCameraSize = RemoteSettings.GetFloat("MainCameraSize", MainCameraSize);

        IsCanChangeDirectionInMovement = RemoteSettings.GetBool("IsCanChangeDirectionInMovement", IsCanChangeDirectionInMovement);


        var gamesRemoteSettings = new GamesRemoteSettings()
        {
            EnemySpeed = EnemySpeed,
            GameSpeed = GameSpeed,
            IsCanChangeDirectionInMovement = IsCanChangeDirectionInMovement,
            MainCameraSize = MainCameraSize,
            PlayerSpeed = PlayerSpeed,
            TimeToSwitchHatchState = TimeToSwitchHatchState
        };

        SavingGlobalSettings.Settings.RemoteSettings = gamesRemoteSettings;
        SavingGlobalSettings.SaveGameData();
     }
}
