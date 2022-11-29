using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisX : MonoBehaviour
{
    //public bool isMarkAtOrigin = true;
    public GameObject markAtOriginPrefab;

    private Transform markAtOrigin;

    private void Awake() {
        if (markAtOrigin)
        {
            markAtOrigin = Instantiate(markAtOriginPrefab, transform).transform;
            markAtOrigin.name = "Mark At Origin";
        }
    }
}
