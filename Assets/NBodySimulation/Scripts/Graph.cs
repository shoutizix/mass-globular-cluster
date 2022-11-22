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
    [SerializeField] private bool showOriginY = false;
    [SerializeField] private bool showOriginX = false;
    [SerializeField] private float lengthPositiveAxisY = 1f;
    [SerializeField] private float lengthPositiveAxisX = 1f;
    [SerializeField] private float lengthNegativeAxisY = 0f;
    [SerializeField] private float lengthNegativeAxisX = 0f;
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

    public void SetLengthPositiveAxisX(float length)
    {
        graphAxis.SetLengthPositiveAxisX(length);
    }

    public void SetLengthPositiveAxisY(float length)
    {
        graphAxis.SetLengthPositiveAxisY(length);
    }

    public void SetLengthNegativeAxisX(float length)
    {
        graphAxis.SetLengthNegativeAxisX(length);
    }

    public void SetLengthNegativeAxisY(float length)
    {
        graphAxis.SetLengthNegativeAxisY(length);
    }

    public void SetVisibilityOriginX(bool isVisible)
    {
        graphAxis.SetVisibilityOriginX(isVisible);
    }

    public void SetVisibilityOriginY(bool isVisible)
    {
        graphAxis.SetVisibilityOriginY(isVisible);
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
        crossMark.transform.Rotate(Vector3.up, 90f);
        // MAYBE CHANGE !
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
        SetLengthPositiveAxisX(lengthPositiveAxisX);
        SetLengthPositiveAxisY(lengthPositiveAxisY);
        SetLengthNegativeAxisX(lengthNegativeAxisX);
        SetLengthNegativeAxisY(lengthNegativeAxisY);
        SetVisibilityOriginX(showOriginX);
        SetVisibilityOriginY(showOriginY);
    }
}
