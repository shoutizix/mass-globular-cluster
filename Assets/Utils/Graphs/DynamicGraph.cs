using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO Make a parent class for this and CoordinateSpace2D
public class DynamicGraph : MonoBehaviour
{
    [Header("Histograms")]
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Color blockColor;
    [SerializeField] private float widthBlock = 0.5f;

    [Header("Plots")]
    [SerializeField] private GameObject linePlotPrefab;
    [SerializeField] private float markerSize = 0.1f;
    [SerializeField] private float lineWidth = 1f;

    [Header("Axes")]
    [SerializeField] private Image xAxis;
    [SerializeField] private Image yAxis;
    [SerializeField] private float axisLineWidth = 2;

    [Header("Extent")]
    [SerializeField] private Vector2 xRange = new Vector2(-1, 1);
    [SerializeField] private Vector2 yRange = new Vector2(-1, 1);
    private Vector2 xRangeInit;
    private Vector2 yRangeInit;

    [Header("Behavior")]
    [SerializeField] private bool rolling = false;
    [SerializeField] private float xMax;
    private float xMaxInit;

    private RectTransform rect;
    private List<LinePlot> lines;
    private List<Image> blocks;
    private List<float> posXBlocks;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        lines = new List<LinePlot>();
        blocks = new List<Image>();
        posXBlocks = new List<float>();

        xRangeInit = xRange;
        yRangeInit = yRange;
        xMaxInit = xMax;
    }

    private void OnValidate()
    {
        if (!rect) rect = GetComponent<RectTransform>();

        Vector2 pos = CoordinateToRectPosition(Vector2.zero);
        if (xAxis)
        {
            xAxis.rectTransform.sizeDelta = new Vector2(rect.rect.width, axisLineWidth);
            xAxis.rectTransform.anchoredPosition = pos.y * Vector2.up;
        }
        if (yAxis)
        {
            yAxis.rectTransform.sizeDelta = new Vector2(axisLineWidth, rect.rect.height);
            yAxis.rectTransform.anchoredPosition = pos.x * Vector2.right;
        }
    }

    public Vector2 GetXRange()
    {
        return xRange;
    }

    public Vector2 GetYRange()
    {
        return yRange;
    }

    public void DrawExteriorBorder(List<Vector2> positions, Color color)
    {
        if (!rect) rect = GetComponent<RectTransform>();

        int indexOfLine = lines.Count-1;
        if (indexOfLine >= 0)
        {
            lines[indexOfLine].Clear();
            Destroy(lines[indexOfLine].gameObject);
            lines.RemoveAt(indexOfLine);
        }

        CreateLine(color, false, "exterior");

        for (int i = 0; i < positions.Count; i++)
        {
            float xPos1 = positions[i].x - widthBlock/2f;
            float xPos2 = positions[i].x + widthBlock/2f;
            float yPos = positions[i].y;
            if (i == 0)
            {
                PlotPointOnLastLine(Vector2.right * xPos1, true);
            }
            PlotPointOnLastLine(new Vector2(xPos1, yPos), true);
            if (i == positions.Count - 1)
            {
                PlotPointOnLastLine(new Vector2(positions[i].x, yPos), true);
            } else 
            {
                PlotPointOnLastLine(new Vector2(xPos2, yPos), true);
            }
        }
    }

    public void CreateBlock(Vector2 position, string label = "")
    {
        if (!blockPrefab) return;
        // Convert width from Coordinate to Rect width
        if (!rect) rect = GetComponent<RectTransform>();

        float width = widthBlock * rect.rect.width / (Mathf.Abs(xRange.x)+Mathf.Abs(xRange.y));
        Vector2 convertedPos = CoordinateToRectPosition(position);
        Vector2 newAnchoredPos = Vector2.right * convertedPos.x + Vector2.up * convertedPos.y / 2f;

        if (posXBlocks.Contains(position.x))
        {
            Image currblock = blocks[posXBlocks.IndexOf(position.x)];
            currblock.rectTransform.anchoredPosition = newAnchoredPos;
            currblock.rectTransform.sizeDelta = new Vector2(width, convertedPos.y);
            return;
        }

        Image block = Instantiate(blockPrefab, transform).GetComponent<Image>();
        
        // The position of the block should be at the x coord of the graph
        block.rectTransform.anchoredPosition = newAnchoredPos;
        // The height of the block should be the converted y coord
        block.rectTransform.sizeDelta = new Vector2(width, convertedPos.y);
        block.color = blockColor;
        block.name = "Block " + label;

        // Puts to the back of UI scene
        block.rectTransform.SetAsFirstSibling();

        // Add block to the list
        blocks.Add(block);
        posXBlocks.Add(position.x);
    }

    public void ClearBlocks()
    {
        foreach (Image block in blocks)
        {
            Destroy(block.gameObject);
        }

        blocks.Clear();
        posXBlocks.Clear();
    }

    public void CreateLine(Color color, bool showMarkers, string label = "")
    {
        if (!linePlotPrefab) return;

        var linePlot = Instantiate(linePlotPrefab, transform).GetComponent<LinePlot>();
        linePlot.color = color;
        linePlot.showMarkers = showMarkers;
        linePlot.lineWidth = lineWidth;
        linePlot.markerSize = markerSize;
        linePlot.name = "Line " + label;

        lines.Add(linePlot);
    }

    public void PlotPointOnLastLine(Vector2 position, bool allowSameX = false, bool coordToRect = true)
    {
        // Do nothinng if no line has been created
        if (lines.Count == 0) return;

        PlotPoint(lines.Count-1, position, allowSameX, coordToRect);
    }

    public void PlotPoint(int lineIndex, Vector2 position, bool allowSameX = false, bool coordToRect = true)
    {
        if (lineIndex < 0 || lineIndex >= lines.Count) return;
        if (!lines[lineIndex].gameObject.activeInHierarchy) return;
        if (position.x > xMax && !rolling) return;

        if (rolling && position.x > xMax)
        {
            float deltaX = position.x - xMax;
            xRange.x += deltaX;
            xRange.y += deltaX;
            Vector2 delta = CoordinateToRectDisplacement(deltaX * Vector2.right);
            foreach (LinePlot line in lines)
            {
                line.ShiftPoints(-delta, 0);
            }
            xMax += deltaX;
        }

        if (coordToRect) position = CoordinateToRectPosition(position);
        lines[lineIndex].PlotPoint(position, true, allowSameX);
    }

    public void Clear()
    {
        foreach (var line in lines)
        {
            line.Clear();
        }

        ClearBlocks();

        xRange = xRangeInit;
        yRange = yRangeInit;
        xMax = xMaxInit;
    }

    private Vector2 ScreenPositionToUV(Vector2 position, Camera camera)
    {
        if (!rect) rect = GetComponent<RectTransform>();

        Vector2 normalizedPosition = default;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, position, camera, out var localPosition))
        {
            normalizedPosition = Rect.PointToNormalized(rect.rect, localPosition);
        }

        return normalizedPosition;
    }

    public void SetLineVisibility(int lineIndex, bool visible)
    {
        if (lines == null) return;
        if (lineIndex < 0 || lineIndex >= lines.Count) return;

        lines[lineIndex].gameObject.SetActive(visible);
    }

    private Vector2 UVToCoordinatePosition(Vector2 uv)
    {
        float x = (xRange.y - xRange.x) * uv.x + xRange.x;
        float y = (yRange.y - yRange.x) * uv.y + yRange.x;
        return new Vector2(x, y);
    }

    private Vector2 CoordinatePositionToUV(Vector2 coordinatePosition)
    {
        float u = (coordinatePosition.x - xRange.x) / (xRange.y - xRange.x);
        float v = (coordinatePosition.y - yRange.x) / (yRange.y - yRange.x);
        return new Vector2(u, v);
    }

    private Vector2 CoordinateDisplacementToUV(Vector2 coordinateDisplacement)
    {
        float deltaU = coordinateDisplacement.x / (xRange.y - xRange.x);
        float deltaV = coordinateDisplacement.y / (yRange.y - yRange.x);
        return new Vector2(deltaU, deltaV);
    }

    private Vector2 UVToRectPosition(Vector2 uv)
    {
        if (!rect) rect = GetComponent<RectTransform>();

        return uv * rect.rect.size;
    }

    private Vector2 UVToRectDisplacement(Vector2 uvDisplacement)
    {
        if (!rect) rect = GetComponent<RectTransform>();

        return uvDisplacement * rect.rect.size;
    }

    private Vector2 RectPositionToUV(Vector2 rectPosition)
    {
        if (!rect) rect = GetComponent<RectTransform>();

        return (rectPosition / rect.rect.size) + 0.5f * Vector2.one;
    }

    private Vector2 CoordinateToRectPosition(Vector2 coordinatePosition)
    {
        Vector2 uv = CoordinatePositionToUV(coordinatePosition);
        return UVToRectPosition(uv);
    }

    private Vector2 CoordinateToRectDisplacement(Vector2 coordinateDisplacement)
    {
        Vector2 deltaUV = CoordinateDisplacementToUV(coordinateDisplacement);
        return UVToRectDisplacement(deltaUV);
    }
}
