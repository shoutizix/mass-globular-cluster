using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBody : MonoBehaviour
{
    public Texture2D handCursor;
    private Vector2 hotspot = new Vector2(14, 6);
    private bool interactable = false;
    
    private void OnMouseEnter() 
    {
        if (!interactable) return;

        if (handCursor)
        {
            Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
        }

        // TODO : Update graphs 
    }

    private void OnMouseExit() 
    {
        if (!interactable) return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        // TODO : Update graphs 
    }

    public void SetInteractable(bool newVal)
    {
        interactable = newVal;

        if (!interactable)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}
