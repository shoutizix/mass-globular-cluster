using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDistribution : MonoBehaviour
{
    public static float NormalPDF(float x, float sigma, float mu)
    {
        return ((1f / Mathf.Sqrt(2f * Mathf.PI * sigma * sigma)) * Mathf.Exp(-((x - mu) * (x - mu)) / (2 * sigma * sigma)));
    }
}
