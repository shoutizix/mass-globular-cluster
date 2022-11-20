using UnityEngine;

public class GraphAxis : MonoBehaviour
{
    [Header("Arrows")]
    [SerializeField] private Arrow axisY;
    [SerializeField] private Arrow axisX;
    [SerializeField] private Vector3 directionAxisY = Vector3.up;
    [SerializeField] private Vector3 directionAxisX = Vector3.right;
    [SerializeField] private float lengthAxisY = 1f;
    [SerializeField] private float lengthAxisX = 1f;
    [SerializeField] private bool showAxisY = true;
    [SerializeField] private bool showAxisX = true;
    [SerializeField] private bool axisYAtMiddleAxisX = false;

    private Vector3 originAxisX;
    private Vector3 originAxisY;

    private void Awake() {
        Redraw();
    }

    private void OnValidate() 
    {
        Redraw();
    }

    public void SetAxisYAtMiddleAxisX(bool newValue, bool redraw = true)
    {
        axisYAtMiddleAxisX = newValue;
        if (redraw) Redraw();
    }

    public void SetLengthAxisX(float length, bool redraw = true)
    {
        axisX.SetComponents(axisX.components * length);
        if (redraw) Redraw();
    }

    public void SetLengthAxisY(float length, bool redraw = true)
    {
        axisY.SetComponents(axisY.components * length);
        if (redraw) Redraw();
    }

    public void SetVisibilityAxisX(bool isVisible, bool redraw = true)
    {
        showAxisX = isVisible;
        if (redraw) Redraw();
    }

    public void SetVisibilityAxisY(bool isVisible, bool redraw = true)
    {
        showAxisY = isVisible;
        if (redraw) Redraw();
    }

    public void SetOriginAxisX(Vector3 origin)
    {
        originAxisX = origin;
    }

    public void SetOriginAxisY(Vector3 origin)
    {
        originAxisY = origin;
    }

    public Vector3 GetOriginAxisX()
    {
        return originAxisX;
    }

    public Vector3 GetOriginAxisY()
    {
        return originAxisY;
    }

    public void SetDirectionAxisX(Vector3 direction, bool redraw = true)
    {
        axisX.SetComponents(direction);
        if (redraw) Redraw();
    }

    public void SetDirectionAxisY(Vector3 direction, bool redraw = true)
    {
        axisY.SetComponents(direction);
        if (redraw) Redraw();
    }

    // Update so that the axis cross at the middle of the X axis
    private void UpdatePositionPositiveYQuadrants()
    {
        SetLengthAxisX(axisX.components.x * 2, false);

        Vector3 oldPos = axisX.transform.position;
        axisX.transform.position = oldPos - (Vector3.right * axisX.components.x / 2);

        SetOriginAxisX(oldPos);
    }

    public void Redraw()
    {
        // MAYBE CHANGE THIS
        axisY.transform.position = Vector3.zero;
        axisX.transform.position = Vector3.zero;

        axisY.SetComponents(Vector3.zero);
        axisX.SetComponents(Vector3.zero);

        if (showAxisY)
        {
            SetDirectionAxisY(directionAxisY, false);
            SetLengthAxisY(lengthAxisY, false);
        }
        if (showAxisX)
        {
            SetDirectionAxisX(directionAxisX, false);
            SetLengthAxisX(lengthAxisX, false);
        }

        SetOriginAxisX(axisX.transform.position);
        SetOriginAxisY(axisY.transform.position);

        if (axisYAtMiddleAxisX) UpdatePositionPositiveYQuadrants();
    }
}
