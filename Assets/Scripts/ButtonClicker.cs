using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClicker : MonoBehaviour
{
    public GameObject Wall;
    public GameObject Player;

    public void OnTileButtonClick(string tileType)
    {
        var UIConstractor = GameObject.Find("ConstractorUI");
        var constractorUIComponent = UIConstractor.GetComponent<ConstractorUI>();
        constractorUIComponent.CurrentTileType =  (TileType)Enum.Parse(typeof(TileType), tileType, true);
    }

    public void OnGenerateLevelButtonClick()
    {
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
                        var gameObject = Instantiate(Wall);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        break;
                    case TileType.Player:
                        gameObject = Instantiate(Player);
                        gameObject.transform.position = new Vector3(i, j, 0);
                        var movement = gameObject.GetComponent<PlayerMovement>();
                        movement.LevelStructure = levelStructure;
                        break;
                }
            }
        }

        var canvas = GameObject.Find("Canvas");
        canvas.SetActive(false);
    }

}
