using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNextToText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMass;
    [SerializeField] private float offsetX = 10f;
    [SerializeField] private float offsetY = -0.5f;
    private Image solarMassUnitImage;
    private void OnEnable() 
    {
        if (!solarMassUnitImage)
        {
            solarMassUnitImage = GetComponent<Image>();
        }
        if (textMass)
        {
            UpdateUnitPosition();
        }
    }

    public void UpdateUnitPosition()
    {
        if (!textMass) return;

        float xPos = textMass.rectTransform.offsetMin.x + textMass.preferredWidth + offsetX;
        float yPos = textMass.rectTransform.offsetMax.y + offsetY;
        solarMassUnitImage.rectTransform.anchoredPosition = new Vector2(xPos, yPos);
    }
}
