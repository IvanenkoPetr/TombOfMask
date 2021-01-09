using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Globals
{
    public const string LevelSelectionSceneName = "Scenes/LevelSelectionScene";
    public const string EditorSceneName = "Scenes/EditorScene";
    public const string SettingsSceneName = "Scenes/SettingsScene";
    public const string MainMenuSceneName = "Scenes/MainMenu";
    public const string GameplaySceneName = "Scenes/GameplayScene";

    private static LevelInfo[,] levelStructure;
    public static LevelInfo[,] LevelStructure => levelStructure;

    public static bool IsEditorScene { get; set; }

    public static bool IsArcadeMode { get; set; }

    public static void  SetLevelStructure(LevelInfo[,] levelInfos)
    {
        levelStructure = levelInfos;
    }

    private static Dictionary<string, List<List<LevelInfoDto>>> allLevelParts;
    public static Dictionary<string, List<List<LevelInfoDto>>> AllLevelParts => allLevelParts;

    public static void LoadAllLevelParts()
    {
        allLevelParts = new Dictionary<string, List<List<LevelInfoDto>>>();

        var partName = @"RandomLevelParts\LevelStartStructure";
        var levelPart = ResourcesManagment.LoadLevelStructureInDto(partName);
        allLevelParts.Add(partName, levelPart);

        for(var i = 1; i <= 5; i++)
        {
            partName = $@"RandomLevelParts\RandomLevelPart{i}";
            levelPart = ResourcesManagment.LoadLevelStructureInDto(partName);
            allLevelParts.Add(partName, levelPart);
        }
    }

    public static void GenerateLevel(Transform gameObjectParent, GameObject mainCamera)
    {
        var levelStructure = Globals.LevelStructure;
        var soundController = GameplaySettings.SoundController.GetComponent<SoundController>();

        for (var i = 0; i < levelStructure.GetLength(0); i++)
        {
            for (var j = 0; j < levelStructure.GetLength(1); j++)
            {
                var tileInfo = levelStructure[i, j];
                var tileType = tileInfo.TileType;
                switch (tileType)
                {
                    case TileType.Wall:

                        var gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Wall, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        if (tileInfo.Options != null)
                        {
                            var spikeLayer = gameObject.transform.Find("SpikesLayer").gameObject;
                            spikeLayer.SetActive(true);
                            var spikesInfo = (Dictionary<SpikeType, bool>)tileInfo.Options;
                            foreach (var spike in spikesInfo)
                            {
                                spikeLayer.transform.Find(spike.Key.ToString()).gameObject.SetActive(spike.Value);
                            }
                        }

                        var gameObjectTileInfo = gameObject.GetComponent<LevelInfo>();
                        gameObjectTileInfo.TileType = TileType.Wall;

                        break;
                    case TileType.Player:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Player, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        var movement = gameObject.GetComponent<PlayerMovement>();
                        movement.WallCollisionEvent += soundController.PlayerWallCollisionEvent;
                        movement.PlayerMovedUp += ArcadeLevelGeneration.PlayerMovedUp;

                        mainCamera.transform.SetParent(gameObject.transform);
                        mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.position.z);

                        var audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;

                        if (Globals.IsArcadeMode)
                        {
                            var lava = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Lava, gameObjectParent);
                            lava.transform.position = new Vector3(6, -3, 0);
                            movement.Lava = lava;
                        }

                        break;
                    case TileType.Collectible:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Collectible, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var collectible = gameObject.GetComponent<Collectible>();
                        collectible.CollectibleCollisionEvent += soundController.CollectibleCollisionEvent;
                        collectible.CollectibleCollisionEvent += ProgressController.CollectebleCollectedHandler;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        gameObjectTileInfo = gameObject.GetComponent<LevelInfo>();
                        gameObjectTileInfo.TileType = TileType.Collectible;

                        break;
                    case TileType.Enemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.None;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;

                        break;
                    case TileType.HorizontalEnemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.Horizontal;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;
                    case TileType.VerticalEnemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.Vertical;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;
                    case TileType.RandomEnemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.Random;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;

                        break;
                    case TileType.Hatch:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Hatch, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var hatchBehaviour = gameObject.GetComponent<Hatch>();
                        hatchBehaviour.ChangeStateInSeconds = SavingGlobalSettings.Settings.RemoteSettings.TimeToSwitchHatchState;
                        hatchBehaviour.HatchChangeStateEvent += soundController.HatchChangeStatusEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;

                    case TileType.Star:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Star, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var star = gameObject.GetComponent<Star>();
                        star.CollectibleCollisionEvent += soundController.StarCollisionEvent;
                        star.CollectibleCollisionEvent += ProgressController.StarsCollectedHandler;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;

                    case TileType.Exit:

                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Exit, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var exit = gameObject.GetComponent<Exit>();
                        exit.ExitCollisionEvent += ProgressController.ExitCollisionHandler;
                        break;
                }
            }
        }
        mainCamera.GetComponent<Camera>().orthographicSize = SavingGlobalSettings.Settings.RemoteSettings.MainCameraSize;


    }

    public static void GenerateLevelPartForArcade(Transform gameObjectParent, GameObject mainCamera, int generateAfterY)
    {
        var levelStructure = Globals.LevelStructure;
        var soundController = GameplaySettings.SoundController.GetComponent<SoundController>();

        var freeObjects = new List<GameObject>();
        foreach(Transform trans in gameObjectParent)
        {
            if(trans.position.y + 10 < mainCamera.transform.position.y)
            {
                freeObjects.Add(trans.gameObject);
            }
        }

        for (var i = 0; i < levelStructure.GetLength(0); i++)
        {
            for (var j = generateAfterY; j < levelStructure.GetLength(1); j++)
            {

                var tileInfo = levelStructure[i, j];     
                
                switch (tileInfo.TileType)
                {
                    case TileType.Wall:

                        GameObject gameObject;
                        gameObject = freeObjects.FirstOrDefault(a => a.GetComponent<LevelInfo>()?.TileType == TileType.Wall);
                        if(gameObject is null)
                        {
                            gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Wall, gameObjectParent);
                        }
                        else
                        {
                            freeObjects.Remove(gameObject);
                        }
                        
                        gameObject.transform.position = new Vector3(i, j, 0);
                        if (tileInfo.Options != null)
                        {
                            var spikeLayer = gameObject.transform.Find("SpikesLayer").gameObject;
                            spikeLayer.SetActive(true);
                            var spikesInfo = (Dictionary<SpikeType, bool>)tileInfo.Options;
                            foreach (var spike in spikesInfo)
                            {
                                spikeLayer.transform.Find(spike.Key.ToString()).gameObject.SetActive(spike.Value);
                            }
                        }

                        var gameObjectTileInfo = gameObject.GetComponent<LevelInfo>();
                        gameObjectTileInfo.TileType = TileType.Wall;

                        break;
                    case TileType.Player:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Player, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        var movement = gameObject.GetComponent<PlayerMovement>();
                        movement.WallCollisionEvent += soundController.PlayerWallCollisionEvent;
                        movement.PlayerMovedUp += ArcadeLevelGeneration.PlayerMovedUp;

                        mainCamera.transform.SetParent(gameObject.transform);
                        mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.position.z);

                        var audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;
                    case TileType.Collectible:

                        gameObject = freeObjects.FirstOrDefault(a => a.GetComponent<LevelInfo>()?.TileType == TileType.Collectible);
                        if (gameObject is null)
                        {
                            gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Collectible, gameObjectParent);
                        }
                        else
                        {
                            freeObjects.Remove(gameObject);
                        }

                        gameObject.transform.position = new Vector3(i, j, 0);

                        var collectible = gameObject.GetComponent<Collectible>();
                        collectible.CollectibleCollisionEvent += soundController.CollectibleCollisionEvent;
                        collectible.CollectibleCollisionEvent += ProgressController.CollectebleCollectedHandler;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;

                        gameObjectTileInfo = gameObject.GetComponent<LevelInfo>();
                        gameObjectTileInfo.TileType = TileType.Collectible;
                        break;
                    case TileType.Enemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.None;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;

                        break;
                    case TileType.HorizontalEnemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.Horizontal;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;
                    case TileType.VerticalEnemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.Vertical;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;
                    case TileType.RandomEnemy:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Enemy, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        swipeMovement = gameObject.GetComponent<SwipeMovement>();
                        swipeMovement.MovementAxis = MovementAxis.Random;
                        swipeMovement.WallCollisionEvent += soundController.EnemyWallCollisionEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;

                        break;
                    case TileType.Hatch:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Hatch, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var hatchBehaviour = gameObject.GetComponent<Hatch>();
                        hatchBehaviour.ChangeStateInSeconds = SavingGlobalSettings.Settings.RemoteSettings.TimeToSwitchHatchState;
                        hatchBehaviour.HatchChangeStateEvent += soundController.HatchChangeStatusEvent;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;

                    case TileType.Star:
                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Star, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var star = gameObject.GetComponent<Star>();
                        star.CollectibleCollisionEvent += soundController.StarCollisionEvent;
                        star.CollectibleCollisionEvent += ProgressController.StarsCollectedHandler;

                        audio = gameObject.GetComponent<AudioSource>();
                        audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                        break;

                    case TileType.Exit:

                        gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Exit, gameObjectParent);
                        gameObject.transform.position = new Vector3(i, j, 0);

                        var exit = gameObject.GetComponent<Exit>();
                        exit.ExitCollisionEvent += ProgressController.ExitCollisionHandler;
                        break;
                }
            }
        }
        mainCamera.GetComponent<Camera>().orthographicSize = SavingGlobalSettings.Settings.RemoteSettings.MainCameraSize;
    }
}
