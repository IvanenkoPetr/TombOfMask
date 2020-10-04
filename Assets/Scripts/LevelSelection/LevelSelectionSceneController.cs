using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelSelectionSceneController : MonoBehaviour
{
    private GameObject ScrollContent;
    private GameObject ButtonLayer;
    private GameObject Canvas;
    private GameObject LineLayer;

    private int starHeight = 300;
    private int distance = 600;
    private int buttonHeight = 400;
    // Start is called before the first frame update
    void Start()
    {
        ScrollContent = GameObject.Find("ScrollContent");
        ButtonLayer = GameObject.Find("ButtonLayer");
        LineLayer = GameObject.Find("LineLayer");
        Canvas = GameObject.Find("Canvas");

        var scrollRectTransform = ScrollContent.GetComponent<RectTransform>();
        scrollRectTransform.anchoredPosition = new Vector2(0, 100000);

        GenerateScene();
    }

    private void InstantiateLevelButton(GameObject gameObject, int level, ref List<GameObject> freeLines)
    {
        var canvasRectTransform = ScrollContent.GetComponent<RectTransform>();
        var canvasWidth = canvasRectTransform.rect.width;

        var buttonRectTransform = gameObject.GetComponent<RectTransform>();
        var YPosition = LevelToYPosition(level);
        var XPosition = canvasWidth / 2;
        if (level % 4 == 2)
        {
            XPosition = XPosition - canvasWidth / 4;
        }
        else if (level % 4 == 0)
        {
            XPosition = XPosition + canvasWidth / 4;
        }

        buttonRectTransform.anchoredPosition = new Vector2(XPosition, YPosition);

        var levelSelectionButtonInfo = gameObject.GetComponent<LevelSelectionButtonInfo>();
        levelSelectionButtonInfo.Level = level;

        var button = gameObject.GetComponent<Button>();
        //button.onClick.AddListener(levelSelectionButtonInfo.OnLevelButtonClick);
        button.onClick.AddListener(GenerateScene);

        var textGameObject = gameObject.transform.Find("Text");
        textGameObject.GetComponent<Text>().text = $"Level {level}";
        gameObject.SetActive(true);

        InstantiateLineButton(gameObject, ref freeLines, level, canvasWidth, buttonRectTransform);

    }

    private void InstantiateLineButton(GameObject levelButton, ref List<GameObject> freeLines, int level, float canvasWidth, RectTransform buttonRectTransform)
    {
        var levelButtonRectTransform = levelButton.GetComponent<RectTransform>();
        var startLinePoint = new Vector2(levelButtonRectTransform.anchoredPosition.x,
            levelButtonRectTransform.anchoredPosition.y + buttonRectTransform.rect.height / 2);

        var endLinePoint = Vector2.zero;
        switch ((level + 1) % 4)
        {
            case 1:
                endLinePoint = new Vector2(levelButtonRectTransform.anchoredPosition.x - canvasWidth / 4, levelButtonRectTransform.anchoredPosition.y + distance - buttonRectTransform.rect.height/2);
                break;
            case 2:
                endLinePoint = new Vector2(levelButtonRectTransform.anchoredPosition.x - canvasWidth / 4, levelButtonRectTransform.anchoredPosition.y + distance - buttonRectTransform.rect.height/2);
                break;
            case 3:
                endLinePoint = new Vector2(levelButtonRectTransform.anchoredPosition.x + canvasWidth / 4, levelButtonRectTransform.anchoredPosition.y + distance - buttonRectTransform.rect.height/2);
                break;
            case 0:
                endLinePoint = new Vector2(levelButtonRectTransform.anchoredPosition.x + canvasWidth / 4, levelButtonRectTransform.anchoredPosition.y + distance - buttonRectTransform.rect.height/2);
                break;
        }
        var line = freeLines.FirstOrDefault();
        if (line is null)
        {
            line = GameObject.Instantiate(LevelSelectionPrefabsSettings.Prefabs.LevelLine, LineLayer.transform);
        }
        else
        {
            freeLines.Remove(line);
        }

        var levelButtonInfo = line.gameObject.GetComponent<LevelSelectionButtonInfo>();
        levelButtonInfo.Level = level;

        var lineRectTransform = line.GetComponent<RectTransform>();
        lineRectTransform.anchoredPosition = new Vector2((startLinePoint.x + endLinePoint.x) / 2, (startLinePoint.y + endLinePoint.y) / 2);
        lineRectTransform.sizeDelta = new Vector2(Mathf.Pow(Mathf.Pow((startLinePoint.x - endLinePoint.x), 2) + Mathf.Pow((startLinePoint.y - endLinePoint.y), 2), 0.5f) + 100, 50);
        var rotation = Mathf.Atan((endLinePoint.y - startLinePoint.y) / (endLinePoint.x - startLinePoint.x)) * (180 / Mathf.PI);

        lineRectTransform.eulerAngles = new Vector3(0, 0, rotation);
        line.SetActive(true);
    }

    private void GenerateScene()
    {
        var activeLevel = new List<int>();
        var freeButtons = new List<GameObject>();
        var freeLines = new List<GameObject>();
        HideUnActiveElements(ref activeLevel, ref freeButtons, ref freeLines);

        foreach (var level in activeLevel)
        {
            if(level< 1)
            {
                continue;
            }

            var buttonGameObject = freeButtons.FirstOrDefault();

            if (buttonGameObject is null)
            {
                buttonGameObject = Instantiate(LevelSelectionPrefabsSettings.Prefabs.LevelButton, ButtonLayer.transform);
            }
            else
            {
                freeButtons.Remove(buttonGameObject);
            }

            InstantiateLevelButton(buttonGameObject, level, ref freeLines);
           

        }
    }

    private void HideUnActiveElements(ref List<int> activeLevel, ref List<GameObject> freeButtons, ref List<GameObject> freeLines)
    {
        var screenHeight = Canvas.GetComponent<RectTransform>().rect.height;

        var contentRectTransform = ScrollContent.GetComponent<RectTransform>();

        var contentYPosition = (100000 - screenHeight / 2) - contentRectTransform.anchoredPosition.y;
        var upperBorder = contentYPosition + 0.75 * screenHeight;
        var lowerBorder = contentYPosition - 0.75 * screenHeight;

        var totalButtonHeight = (distance - buttonHeight) + buttonHeight;

        foreach (Transform child in ButtonLayer.transform)
        {
            var childRectTransform = child.gameObject.GetComponent<RectTransform>();
            if (childRectTransform.anchoredPosition.y > upperBorder || childRectTransform.anchoredPosition.y < lowerBorder)
            {
                child.gameObject.SetActive(false);
            }

        };

        var levelInCenter = Mathf.RoundToInt(contentYPosition / totalButtonHeight);

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

        foreach (Transform child in ButtonLayer.transform)
        {
            var levelButtonInfo = child.gameObject.GetComponent<LevelSelectionButtonInfo>();

            GameObject line = null;
            foreach (Transform lineChild in LineLayer.transform)
            {
                var levelLineInfo = lineChild.gameObject.GetComponent<LevelSelectionButtonInfo>();
                if (levelButtonInfo.Level == levelLineInfo.Level)
                {
                    line = lineChild.gameObject;
                    break;
                }
            }

            if (child.gameObject.activeInHierarchy)
            {
                activeLevel.Remove(levelButtonInfo.Level);
            }
            else
            {
                freeButtons.Add(child.gameObject);
                line.SetActive(false);
                freeLines.Add(line);
            }




        };
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
