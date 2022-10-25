using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBodySlideController : SimulationSlideController
{
    [Header("Motion parameters")]
    [SerializeField] private float frequency;
    [SerializeField] private float amplitude;

    [Header("Vectors parameters")]
    [SerializeField] private float vectorLength;

    public override void InitializeSlide()
    {
        OneBodySimulation sim = simulation as OneBodySimulation;
        sim.frequency = frequency;
        sim.amplitude = amplitude;
        sim.vectorLength = vectorLength;
    }
}
