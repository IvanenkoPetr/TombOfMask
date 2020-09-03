using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawInInterface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ConstractorUI.IsEditorInSlideMode)
        {
            ProcessTouches();
        }
    }

    private void ProcessTouches()
    {
        var touches = Enumerable.Empty<Touch>();
        
        if (Application.isEditor)
        {
            touches = InputHelper.GetTouches();
        }
        else
        {
            touches = Input.touches;
        }

        foreach (var touch in touches)
        {            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    ProcessTouch(touch.position);
                    break;
                case TouchPhase.Moved:
                    ProcessTouch(touch.position);
                    break;
                case TouchPhase.Ended:
                    ConstractorUI.TilesPressedInTouch.Clear();
                    break;
            }
        }
    }

    private void ProcessTouch(Vector2 touchPosition)
    {
        var touchedGameObject = CheckLevelItemTapEligible(touchPosition);
        if(touchedGameObject != null)
        {
            TileOnCanvas.SetTileInfo(touchedGameObject);
            ConstractorUI.TilesPressedInTouch.Add(touchedGameObject);
        }
    }

    private GameObject CheckLevelItemTapEligible(Vector2 touchPosition)
    {
        GameObject tileObject = null;

        GraphicRaycaster gr = ConstractorUI.EditorCanvas.transform.parent.GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = touchPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);

        tileObject = results.FirstOrDefault(a => a.gameObject.GetComponent<TileOnCanvas>()).gameObject; 

        return tileObject;
    }


}
