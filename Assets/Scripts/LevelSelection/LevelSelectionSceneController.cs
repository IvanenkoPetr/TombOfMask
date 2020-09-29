using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelSelectionSceneController : MonoBehaviour
{
    private GameObject ScrollContent;
    private GameObject Canvas;

    private int starHeight = 300;
    private int distance = 600;
    private int buttonHeight = 400;
    // Start is called before the first frame update
    void Start()
    {
        ScrollContent = GameObject.Find("ScrollContent");
        Canvas = GameObject.Find("Canvas");

        var scrollRectTransform = ScrollContent.GetComponent<RectTransform>();
        scrollRectTransform.anchoredPosition = new Vector2(0, 100000);

        GenerateScene();
    }

    private void InstantiateLevelButton(GameObject gameObject, int level)
    {
        var canvasRectTransform = ScrollContent.GetComponent<RectTransform>();
        var canvasWidth = canvasRectTransform.rect.width;

        var RectTransform = gameObject.GetComponent<RectTransform>();
        var YPosition = LevelToYPosition(level);
        RectTransform.anchoredPosition = new Vector2(canvasWidth / 2, YPosition);

        var levelSelectionButtonInfo = gameObject.GetComponent<LevelSelectionButtonInfo>();
        levelSelectionButtonInfo.Level = level;

        var button = gameObject.GetComponent<Button>();
        //button.onClick.AddListener(levelSelectionButtonInfo.OnLevelButtonClick);
        button.onClick.AddListener(GenerateScene);

        var textGameObject = gameObject.transform.Find("Text");
        textGameObject.GetComponent<Text>().text = $"Level {level}";
        gameObject.SetActive(true);

    }

    private void GenerateScene()
    {
        var screenHeight = Canvas.GetComponent<RectTransform>().rect.height;

        var contentRectTransform = ScrollContent.GetComponent<RectTransform>();

        var contentYPosition = (100000 - screenHeight / 2) - contentRectTransform.anchoredPosition.y;
        var upperBorder = contentYPosition + 0.75 * screenHeight;
        var lowerBorder = contentYPosition - 0.75 * screenHeight;

        var totalButtonHeight = (distance - buttonHeight) + buttonHeight;

        foreach (Transform child in ScrollContent.transform)
        {
            var childRectTransform = child.gameObject.GetComponent<RectTransform>();
            if (childRectTransform.anchoredPosition.y > upperBorder || childRectTransform.anchoredPosition.y < lowerBorder)
            {
                child.gameObject.SetActive(false);
            }
        };

        var levelInCenter = Mathf.RoundToInt(contentYPosition / totalButtonHeight);
        var activeLevel = new List<int>();
        var curentLevel = levelInCenter;
        while (LevelToYPosition(curentLevel) < upperBorder)
        {
            activeLevel.Add(curentLevel);
            curentLevel++;
        }

        curentLevel = levelInCenter;
        while (LevelToYPosition(curentLevel) > lowerBorder)
        {
            activeLevel.Add(curentLevel);
            curentLevel--;
        }
        activeLevel = activeLevel.Distinct().ToList();
        var freeButtons = new List<GameObject>();

        foreach (Transform child in ScrollContent.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                var levelButtonInfo = child.gameObject.GetComponent<LevelSelectionButtonInfo>();
                activeLevel.Remove(levelButtonInfo.Level);
            }
            else
            {
                freeButtons.Add(child.gameObject);
            }
        };

        foreach (var level in activeLevel)
        {
            var buttonGameObject = freeButtons.FirstOrDefault();

            if (buttonGameObject is null)
            {
                buttonGameObject = Instantiate(LevelSelectionPrefabsSettings.Prefabs.LevelButton, ScrollContent.transform);
            }
            else
            {
                freeButtons.Remove(buttonGameObject);
            }

            InstantiateLevelButton(buttonGameObject, level);
        }

    }



    private float LevelToYPosition(int level)
    {
        var result = starHeight + distance * (level - 1);
        return result;
    }

    public void OnScroll(Vector2 position)
    {
        GenerateScene();
    }

}
