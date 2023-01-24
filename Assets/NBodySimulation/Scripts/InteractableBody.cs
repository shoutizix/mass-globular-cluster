using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBody : MonoBehaviour
{
    public Texture2D handCursor;
    private Vector2 hotspot = new Vector2(14, 6);
    private bool interactable = false;
    private int index = 0;
    private List<FastNBodySlideController> slideControllers;

    private void OnMouseEnter() 
    {
        if (!interactable) return;

        if (handCursor)
        {
            Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
        }

        if (slideControllers.Count > 0)
        {
            foreach (FastNBodySlideController slideController in slideControllers)
            {
                if (slideController.gameObject.activeInHierarchy)
                {
                    slideController.DisplayBodyVelocitiesAtIndex(index);
                }
            }
        }
    }

    private void OnMouseExit() 
    {
        if (!interactable) return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        if (slideControllers.Count > 0)
        {
            foreach (FastNBodySlideController slideController in slideControllers)
            {
                if (slideController.gameObject.activeInHierarchy)
                {
                    slideController.ChangeAlphaOtherBodies(index, false);
                }
            }
        }
    }

    public void SetInteractable(bool newVal)
    {
        interactable = newVal;

        if (!interactable)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void SetIndex(int newVal)
    {
        index = newVal;
    }

    public void SetSlideControllers(List<FastNBodySlideController> sControllers)
    {
        slideControllers = sControllers;
    }

}
