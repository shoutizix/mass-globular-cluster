using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject crossMarkPrefab;
    [SerializeField] private GraphAxis graphAxis;
    
    [Header("Axis parameters")]
    [SerializeField] private bool showAxisY;
    [SerializeField] private bool showAxisX;

    private float lengthAxisX = 1f;
    private float lengthAxisY = 1f;
    private bool middeXAxis = false;
    private List<CrossMark> crossMarksList = new List<CrossMark>();
    
    private void Awake() 
    {

    }

    private void OnValidate() 
    {
        DisplayGraph();
        //PutCrossMarkAtGraphPos(new Vector2(Random.Range(0f,1f), Random.Range(0f,1f)));
    }

    public void SetAxisXVisibility(bool isVisible)
    {
        graphAxis.SetVisibilityAxisX(isVisible);
    }

    public void SetAxisYVisibility(bool isVisible)
    {
        graphAxis.SetVisibilityAxisY(isVisible);
    }

    public void SetAxisXLength(float length)
    {
        graphAxis.SetLengthAxisX(length);
    }

    public void SetAxisYLength(float length)
    {
        graphAxis.SetLengthAxisY(length);
    }

    public void SetAxisYAtMiddleAxisX(bool newValue)
    {
        graphAxis.SetAxisYAtMiddleAxisX(newValue);
    }

    public void PutCrossMarkAtGraphPos(Vector2 position, float width, float length, Color color)
    {
        if (!crossMarkPrefab)
        {
            Debug.LogWarning("No Cross Mark Prefab");
            return;
        }

        CrossMark crossMark = Instantiate(crossMarkPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<CrossMark>();
        crossMark.SetPositionInGraph(position);
        crossMark.SetWidth(width);
        crossMark.SetLength(length);
        crossMark.SetColor(color);
        crossMark.transform.localPosition = new Vector3(position.x, position.y, 0);
        crossMarksList.Add(crossMark);
    }

    public void ClearCrossMarks()
    {
        crossMarksList.ForEach(crossMark => Destroy(crossMark));
        crossMarksList.Clear();
    }

    public void DisplayGraph()
    {
        SetAxisXVisibility(showAxisX);
        SetAxisYVisibility(showAxisY);
        SetAxisXLength(lengthAxisX);
        SetAxisYLength(lengthAxisY);
        SetAxisYAtMiddleAxisX(middeXAxis);
    }
}
