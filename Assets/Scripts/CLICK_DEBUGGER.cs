using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CLICK_DEBUGGER : MonoBehaviour
{
    public bool active;

    private void Update()
    {
        Mouse mouse = Mouse.current;

        //List all the UI objects below mouse click that are set to receive raycasts (.raycastTarget == true)
        if (active && mouse.leftButton.isPressed)
        {

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = new(mouse.position.x.ReadValue(), mouse.position.y.ReadValue());
            List<RaycastResult> results = new List<RaycastResult>();

            Debug.Log($"Clicked {eventData.position}");

            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count > 0)
            {
                string objectsClicked = "";
                foreach (RaycastResult result in results)
                {
                    objectsClicked += result.gameObject.name;

                    //If not the last element, add a comma
                    if (result.gameObject != results[^1].gameObject)
                    {
                        objectsClicked += ", ";
                    }
                }
                Debug.Log("Clicked on: " + objectsClicked);
            }
        }
    }
}