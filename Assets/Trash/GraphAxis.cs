using UnityEngine;

public class GraphAxis : MonoBehaviour
{
    [Header("Arrows")]
    [SerializeField] private Arrow axisY;
    [SerializeField] private Arrow axisX;
    [SerializeField] private Vector3 directionAxisY = Vector3.up;
    [SerializeField] private Vector3 directionAxisX = Vector3.right;
    [SerializeField] private float lengthPositiveAxisY = 1f;
    [SerializeField] private float lengthPositiveAxisX = 1f;
    [SerializeField] private float lengthNegativeAxisY = 0f;
    [SerializeField] private float lengthNegativeAxisX = 0f;
    [SerializeField] private bool showAxisY = true;
    [SerializeField] private bool showAxisX = true;
    [SerializeField] private LineRenderer originXLR;
    [SerializeField] private LineRenderer originYLR;
    [SerializeField] private bool showOriginY = false;
    [SerializeField] private bool showOriginX = false;

    private Vector3 originAxisX;
    private Vector3 originAxisY;

    private void Awake() {
        Redraw();
    }

    private void OnValidate() 
    {
        Redraw();
    }

    public void SetLengthPositiveAxisX(float length, bool redraw = true)
    {
        lengthPositiveAxisX = length;
        if (redraw) Redraw();
    }

    public void SetLengthPositiveAxisY(float length, bool redraw = true)
    {
        lengthPositiveAxisY = length;
        if (redraw) Redraw();
    }

    public void SetLengthNegativeAxisX(float length, bool redraw = true)
    {
        lengthNegativeAxisX = length;
        if (redraw) Redraw();
    }

    public void SetLengthNegativeAxisY(float length, bool redraw = true)
    {
        lengthNegativeAxisY = length;
        axisX.transform.Translate(Vector3.down * length);
        if (redraw) Redraw();
    }

    public void UpdateLength(bool showAxisX, bool showAxisY)
    {
        if (showAxisX)
        {
            SetOriginAxisX(axisX.transform.position);
            // Move the arrow to the left
            axisX.transform.Translate(Vector3.left * lengthNegativeAxisX);
            axisX.SetComponents(axisX.components * (lengthNegativeAxisX + lengthPositiveAxisX));
        }
        if (showAxisY)
        {
            SetOriginAxisY(axisY.transform.position);
            // Move the arrow to the down
            axisY.transform.Translate(Vector3.down * lengthNegativeAxisY);
            axisY.SetComponents(axisY.components * (lengthNegativeAxisY + lengthPositiveAxisY));
        }
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

    public void SetVisibilityOriginX(bool isVisible, bool redraw = true)
    {
        showOriginX = isVisible;
        if (redraw) Redraw();
    }

    public void SetVisibilityOriginY(bool isVisible, bool redraw = true)
    {
        showOriginY = isVisible;
        if (redraw) Redraw();
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

    public void DrawOriginAxisX(bool showOriginX)
    {
        if (!originXLR) return;

        originXLR.gameObject.SetActive(showOriginX);

        if (!showOriginX) return;
        
        Vector3 pos1 = axisX.headLR.GetPosition(0);
        Vector3 pos2 = axisX.headLR.GetPosition(2);

         // Color part
        originXLR.startColor = Color.black;
        originXLR.endColor = Color.black;

        // Width part
        originXLR.startWidth = 0.1f;
        originXLR.endWidth = 0.1f;

        // Length and Position part
        originXLR.SetPositions(new Vector3[] {Vector3.up * pos1.y, Vector3.up * pos2.y});
    }

    public void DrawOriginAxisY(bool showOriginY)
    {
        if (!originYLR) return;

        originYLR.gameObject.SetActive(showOriginY);

        if (!showOriginY) return;
        
        Vector3 pos1 = axisY.headLR.GetPosition(0);
        Vector3 pos2 = axisY.headLR.GetPosition(2);

         // Color part
        originYLR.startColor = Color.black;
        originYLR.endColor = Color.black;

        // Width part
        originYLR.startWidth = 0.1f;
        originYLR.endWidth = 0.1f;

        // Length and Position part
        originYLR.SetPositions(new Vector3[] {Vector3.right * pos1.x, Vector3.right * pos2.x});
    }

    public void Redraw()
    {
        axisY.transform.position = Vector3.zero;
        axisX.transform.position = Vector3.zero;

        axisY.SetComponents(Vector3.zero);
        axisX.SetComponents(Vector3.zero);
    
        if (showAxisY)
        {
            SetDirectionAxisY(directionAxisY, false);
        }
        if (showAxisX)
        {
            SetDirectionAxisX(directionAxisX, false);
        }

        UpdateLength(showAxisX, showAxisY);
        
        DrawOriginAxisX(showOriginX);
        DrawOriginAxisY(showOriginY);
    }
}
