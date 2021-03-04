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

    public static Dictionary<(int x, int y), LevelInfo> LevelStructureNew = new Dictionary<(int x, int y), LevelInfo>();

    public static bool IsEditorScene { get; set; }

    public static bool IsArcadeMode { get; set; }

    public static void SetLevelStructure(LevelInfo[,] levelInfos)
    {
        levelStructure = levelInfos;
    }

    public static void SetLevelStructureNew(Dictionary<(int x, int y), LevelInfo> levelStructure)
    {
        LevelStructureNew = levelStructure;
    }

    public static List<Dictionary<Vector2, int>> RoomsList = new List<Dictionary<Vector2, int>>();

    public static Dictionary<Vector2, int> ArcadeCurrentLevelPartPosition { get; set; } = new Dictionary<Vector2, int>
    {
        [Vector2.up] = 0,
        [Vector2.down] = 0,
        [Vector2.left] = 0,
        [Vector2.right] = 0,

    };
    private static Dictionary<string, List<List<LevelInfoDto>>> allLevelParts;
    public static Dictionary<string, List<List<LevelInfoDto>>> AllLevelParts => allLevelParts;

    public static void LoadAllLevelParts()
    {
        allLevelParts = new Dictionary<string, List<List<LevelInfoDto>>>();

        var partName = @"RandomLevelParts\LevelStartStructure";
        var levelPart = ResourcesManagment.LoadLevelStructureInDto(partName);
        allLevelParts.Add(partName, levelPart);

        for (var i = 1; i <= 11; i++)
        //for (var i = 1; i <= 3; i++)
        {
            partName = $@"RandomLevelParts\RandomLevelPart{i}";
            levelPart = ResourcesManagment.LoadLevelStructureInDto(partName);
            allLevelParts.Add(partName, levelPart);
        }
    }

    public static void GenerateLevel(Transform gameObjectParent, GameObject mainCamera)
    {
        var levelStructure = Globals.LevelStructureNew;
        var soundController = GameplaySettings.SoundController.GetComponent<SoundController>();

        foreach (var tileFromLevelStructure in levelStructure)
        {
            var tileInfo = tileFromLevelStructure.Value;
            var i = tileFromLevelStructure.Key.x;
            var j = tileFromLevelStructure.Key.y;

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

                    //if (Globals.IsArcadeMode)
                    //{
                    //    var lava = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Lava, gameObjectParent);
                    //    lava.transform.position = new Vector3(6, -3, 0);
                    //    movement.Lava = lava;
                    //}

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
        mainCamera.GetComponent<Camera>().orthographicSize = SavingGlobalSettings.Settings.RemoteSettings.MainCameraSize;


    }

    public static void GenerateLevelPartForArcade(Transform gameObjectParent, GameObject mainCamera)
    {
        var levelStructure = Globals.LevelStructure;
        var soundController = GameplaySettings.SoundController.GetComponent<SoundController>();

        var freeObjects = new List<GameObject>();
        var cameraPosition = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
        foreach (Transform trans in gameObjectParent)
        {

            if (Vector3.Distance(cameraPosition, trans.position) > 30)
            {
                freeObjects.Add(trans.gameObject);
            }
        }


        for (var i = Globals.ArcadeCurrentLevelPartPosition[Vector2.left]; i < Globals.ArcadeCurrentLevelPartPosition[Vector2.right]; i++)
        {
            for (var j = Globals.ArcadeCurrentLevelPartPosition[Vector2.down]; j < Globals.ArcadeCurrentLevelPartPosition[Vector2.up]; j++)
            {

                //if (!(i >= Globals.ArcadeCurrentLevelPartPosition[Vector2.left]
                //    && i <= Globals.ArcadeCurrentLevelPartPosition[Vector2.right]
                //    && j <= Globals.ArcadeCurrentLevelPartPosition[Vector2.up]
                //    && j >= Globals.ArcadeCurrentLevelPartPosition[Vector2.down]))
                //{
                //    continue;
                //}

                var tileInfo = levelStructure[i, j];

                switch (tileInfo.TileType)
                {
                    case TileType.Wall:

                        GameObject gameObject;
                        gameObject = freeObjects.FirstOrDefault(a => a.GetComponent<LevelInfo>()?.TileType == TileType.Wall);
                        if (gameObject is null)
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
                    //case TileType.Player:
                    //    gameObject = GameObject.Instantiate(GamePlayPrefabsSettings.Prefabs.Player, gameObjectParent);
                    //    gameObject.transform.position = new Vector3(i, j, 0);
                    //    var movement = gameObject.GetComponent<PlayerMovement>();
                    //    movement.WallCollisionEvent += soundController.PlayerWallCollisionEvent;
                    //    movement.PlayerMovedUp += ArcadeLevelGeneration.PlayerMovedUp;

                    //    mainCamera.transform.SetParent(gameObject.transform);
                    //    mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.position.z);

                    //    var audio = gameObject.GetComponent<AudioSource>();
                    //    audio.volume = SavingGlobalSettings.Settings.GameSettings.SoundVolume;
                    //    break;
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

                        var audio = gameObject.GetComponent<AudioSource>();
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


