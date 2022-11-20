using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossMark : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Color color = Color.black;
    [SerializeField] private float length = 1f;
    [SerializeField] private float width = 0.2f;
    [SerializeField] private LineRenderer lineStartHigh;
    [SerializeField] private LineRenderer lineStartLow;

    private Vector2 position;

    private void Awake() 
    {
        Redraw();
    }

    private void OnValidate() 
    {
        Redraw();
    }

    public void SetColor(Color newColor, bool redraw = true)
    {
        color = newColor;
        if (redraw) Redraw();
    }

    public void SetLength(float newLength, bool redraw = true)
    {
        length = newLength;
        if (redraw) Redraw();
    }

    public void SetWidth(float newWidth, bool redraw = true)
    {
        width = newWidth;
        if (redraw) Redraw();
    }

    public void SetPositionInGraph(Vector2 newPosition)
    {
        position = newPosition;
    }

    public Vector2 GetPositionInGraph()
    {
        return position;
    }

    private void Redraw()
    {
        // Color part
        lineStartHigh.startColor = color;
        lineStartHigh.endColor = color;
        lineStartLow.startColor = color;
        lineStartLow.endColor = color;

        // Width part
        lineStartHigh.startWidth = width;
        lineStartHigh.endWidth = width;
        lineStartLow.startWidth = width;
        lineStartLow.endWidth = width;

        // Length part
        lineStartHigh.SetPositions(new Vector3[] {Vector3.up * length, Vector3.right * length});
        lineStartLow.SetPositions(new Vector3[] {Vector3.zero, Vector3.right * length + Vector3.up * length});

        // Center cross
        lineStartHigh.transform.position = Vector3.down * length/2 + Vector3.left * length/2;
        lineStartLow.transform.position = Vector3.down * length/2 + Vector3.left * length/2;
    }
}
