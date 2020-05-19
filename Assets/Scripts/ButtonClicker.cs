using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClicker : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Player;
    public GameObject Enemy;
    public GameObject Collectible;


    private void Update()
    {

    }
    public void OnTileButtonClick(string tileType)
    {
        var UIConstractor = GameObject.Find("ConstractorUI");
        var constractorUIComponent = UIConstractor.GetComponent<ConstractorUI>();
        constractorUIComponent.CurrentTileType =  (TileType)Enum.Parse(typeof(TileType), tileType, true);
    }

    public void OnGenerateLevelButtonClick()
    {
        var mainGameObject = ConstractorUI.MainGame.transform;

        var constractorObject = GameObject.Find("ConstractorUI").GetComponent<ConstractorUI>();
        var levelStructure = constractorObject.LevelStructure;

        for(var i = 0; i < levelStructure.GetLength(0); i++)
        {
            for (var j = 0; j < levelStructure.GetLength(1); j++)
            {
                var tileType = levelStructure[i, j].TileType;
                switch (tileType)
                {
                    case TileType.Wall:
                        var gameObject = Instantiate(Wall, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        break;
                    case TileType.Player:
                        gameObject = Instantiate(Player, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        var movement = gameObject.GetComponent<PlayerMovement>();
                        movement.LevelStructure = levelStructure;

                        var mainCamera = ConstractorUI.MainCamera;
                        mainCamera.transform.SetParent(gameObject.transform);
                        mainCamera.transform.localPosition = new Vector3(0, 0, mainCamera.transform.position.z);
                        break;
                    case TileType.Collectible:
                        gameObject = Instantiate(Collectible, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        break;
                    case TileType.Enemy:
                        gameObject = Instantiate(Enemy, mainGameObject);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        break;
                }
            }
        }

        var canvas = ConstractorUI.Canvas;
        canvas.SetActive(false);

        var mainGame = ConstractorUI.MainGame;
        mainGame.SetActive(true);
    }

}
