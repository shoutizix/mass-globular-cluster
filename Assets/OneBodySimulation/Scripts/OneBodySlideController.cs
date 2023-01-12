using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneBodySlideController : SimulationSlideController
{
    [Header("Motion parameters")]
    [SerializeField] private float frequency;
    [SerializeField] private float amplitude;

    [Header("Vectors parameters")]
    [SerializeField] private float vectorLength;

    [Header("Fade Out Image")]
    [SerializeField] private FadeOutUI handRotate;

    [Header("Equation parts")]
    [SerializeField] private Image Vx;
    [SerializeField] private Image Vy;
    [SerializeField] private Image Vz;
    [SerializeField] private Image Vtot;
    [SerializeField] private byte colorIntensitySelected = 255;
    [SerializeField] private byte colorIntensityUnselected = 150;

    private List<Image> equationImages;
    private List<Outline> equationOutlines;
    private OneBodySimulation sim;

    private void Awake() 
    {
        equationImages = new List<Image>(){Vx, Vy, Vz, Vtot}; 
    }

    public override void InitializeSlide()
    {
        sim = simulation as OneBodySimulation;
        sim.frequency = frequency;
        sim.amplitude = amplitude;
        sim.vectorLength = vectorLength;
        
        if (handRotate)
        {
            handRotate.TriggerReset();
        }
    }

    public void HandleCameraHasRotated()
    {
        if (handRotate)
        {
            handRotate.TriggerFadeOut();
        }
    }

    public void HighlightCurrentImageAndVector(string name, bool isMouseOver)
    {
        List<Image> clonedList = new List<Image>(equationImages);

        switch (name)
        {
            case "Vx":
                clonedList.RemoveAt(0);
                break;
            
            case "Vy":
                clonedList.RemoveAt(1);
                break;

            case "Vz":
                clonedList.RemoveAt(2);
                break;

            case "Vtotal":
                clonedList.RemoveAt(3);
                break;
        }

        if (isMouseOver)
        {
            // Decrease the intensity of other images to highlight the current one
            DecreaseIntensityImages(clonedList);
        } else 
        {
            // Increase the intensity back to normal when the mouse is not over the current image
            IncreaseIntensityImages(clonedList);
        }

        sim.HighlightCurrentVector(name, isMouseOver);
    }

    private void IncreaseIntensityImages(List<Image> list)
    {
        foreach (Image currImage in list)
        {
            Color32 currColor = currImage.color;
            currImage.color = new Color32(currColor.r, currColor.g, currColor.b, colorIntensitySelected);
            
            // Outline part
            Outline currOutline;
            if(currImage.gameObject.TryGetComponent(out currOutline))
            {
                Color32 currOutlineColor = currOutline.effectColor;
                currOutline.effectColor = new Color32(currOutlineColor.r, currOutlineColor.g, currOutlineColor.b, colorIntensitySelected);
            }
        }
    }

    private void DecreaseIntensityImages(List<Image> list)
    {
        foreach (Image currImage in list)
        {
            Color32 currColor = currImage.color;
            currImage.color = new Color32(currColor.r, currColor.g, currColor.b, colorIntensityUnselected);

            // Outline part
            Outline currOutline;
            if(currImage.gameObject.TryGetComponent(out currOutline))
            {
                Color32 currOutlineColor = currOutline.effectColor;
                currOutline.effectColor = new Color32(currOutlineColor.r, currOutlineColor.g, currOutlineColor.b, colorIntensityUnselected);
            }
        }
    }
}
