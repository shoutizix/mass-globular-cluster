using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNextToText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMass;
    [SerializeField] private float offsetX = 10f;
    [SerializeField] private float offsetY = -0.5f;
    private Image solarMassUnitImage;
    private TextMeshProUGUI powerText;
    private void OnValidate()
    {
        if (!solarMassUnitImage)
        {
            TryGetComponent(out solarMassUnitImage);
        }

        TryGetComponent(out powerText);
        
        if (textMass)
        {
            UpdateUnitPosition();
        }
    }

    public void SetTextPower(string power)
    {
        powerText.text = power;
    }

    public void UpdateUnitPosition()
    {
        if (!textMass) return;

        float xPos = textMass.rectTransform.offsetMin.x + textMass.preferredWidth + offsetX;
        float yPos = textMass.rectTransform.offsetMax.y + offsetY;
        if (solarMassUnitImage)
        {
            solarMassUnitImage.rectTransform.anchoredPosition = new Vector2(xPos, yPos);
        }
        if (powerText)
        {
            powerText.rectTransform.anchoredPosition = new Vector2(xPos, yPos);
        }
    }
}
