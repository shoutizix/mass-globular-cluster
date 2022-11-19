using UnityEngine;

public class GraphAxis : MonoBehaviour
{
    [Header("Arrows")]
    [SerializeField] private Arrow axisY;
    [SerializeField] private Arrow axisX;
    [SerializeField] private float axisXLength = 1f;
    [SerializeField] private float axisYLength = 1f;
    [SerializeField] private bool showAxisX = true;
    [SerializeField] private bool showAxisY = true;
    [SerializeField] private bool axisYAtMiddleAxisX = false;

    private void Awake() {
        InitializeAllAxis();
    }

    private void OnValidate() 
    {
        InitializeAllAxis();
    }

    private void SetAxisXLength(float length)
    {
        axisX.SetComponents(Vector3.right * length);
    }

    private void SetAxisYLength(float length)
    {
        axisY.SetComponents(Vector3.up * length);
    }

    // Update so that the axis cross at the middle of the X axis
    private void UpdatePositionPositiveYQuadrants()
    {
        SetAxisXLength(axisX.components.x * 2);

        Vector3 oldPos = axisX.transform.position;
        axisX.transform.position = oldPos - (Vector3.right * axisX.components.x / 2);
    }

    private void InitializeAllAxis()
    {
        axisY.transform.position = Vector3.zero;
        axisX.transform.position = Vector3.zero;

        axisY.SetComponents(Vector3.zero);
        axisX.SetComponents(Vector3.zero);

        if (showAxisY)  SetAxisYLength(axisYLength);
        if (showAxisX)  SetAxisXLength(axisXLength);

        if (axisYAtMiddleAxisX) UpdatePositionPositiveYQuadrants();
    }
}
