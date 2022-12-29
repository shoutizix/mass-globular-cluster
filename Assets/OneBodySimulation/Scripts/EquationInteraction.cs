using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquationInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("Color")]
    //[SerializeField] private byte colorIntensitySelected;
    //[SerializeField] private byte colorIntensityUnselected;
    [SerializeField] private Color colorSelected;
    [SerializeField] private Color colorUnselected;

    [Header("OneBodyPrefabs")]
    [SerializeField] private OneBodyPrefabs prefabs;

    private Color32 currColor;
    private Image currImage;

    private void Awake() 
    {
        if (!currImage)
        {
            currImage = gameObject.GetComponent<Image>();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (prefabs)
        {
            bool newIsVisible = true;
            if (gameObject.name == "Vtotal")
            {
                newIsVisible = !prefabs.GetVectorVelocityTotalVisibility();
                prefabs.SetVectorVelocityTotalVisibility(newIsVisible);
            }

            if (gameObject.name == "Vx")
            {
                newIsVisible = !prefabs.GetVectorVelocityXVisibility();
                prefabs.SetVectorVelocityXVisibility(newIsVisible);
            }

            if (gameObject.name == "Vy")
            {
                newIsVisible = !prefabs.GetVectorVelocityYVisibility();
                prefabs.SetVectorVelocityYVisibility(newIsVisible);
            }

            if (gameObject.name == "Vz")
            {
                newIsVisible = !prefabs.GetVectorVelocityZVisibility();
                prefabs.SetVectorVelocityZVisibility(newIsVisible);
            }

            currColor = currImage.color;

            if (newIsVisible)
            {
                currImage.color = colorSelected;
                //currImage.color = new Color32(currColor.r, currColor.g, currColor.b, colorIntensitySelected);
            } else 
            {
                currImage.color = colorUnselected;
                //currImage.color = new Color32(currColor.r, currColor.g, currColor.b, colorIntensityUnselected);
            }
            
        }
    }
}
