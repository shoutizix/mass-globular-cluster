using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBodySlideController : SimulationSlideController
{
    [Header("Motion parameters")]
    [SerializeField] private float frequency;
    [SerializeField] private float amplitude;

    public override void InitializeSlide()
    {
        OneBodySimulation sim = simulation as OneBodySimulation;
        sim.frequency = frequency;
        sim.amplitude = amplitude;
    }
}
