﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneBodyPrefabs : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject vectorVelocityXPrefab;
    [SerializeField] private GameObject vectorVelocityYPrefab;
    [SerializeField] private GameObject vectorVelocityZPrefab;
    [SerializeField] private GameObject vectorVelocityTotalPrefab;
    [SerializeField] private GameObject coordinateTriadPrefab;
    [SerializeField] private GameObject orbitPrefab;

    [HideInInspector] public Transform positionBody;
    [HideInInspector] public Vector positionVectorVelocityX;
    [HideInInspector] public Vector positionVectorVelocityY;
    [HideInInspector] public Vector positionVectorVelocityZ;
    [HideInInspector] public Vector positionVectorVelocityTotal;
    [HideInInspector] public Transform positionCoordinateTriad;
    [HideInInspector] public LineRenderer orbit;

    public void SetBodyVisibility(bool isVisible)
    {
        if (positionBody)
        {
            positionBody.gameObject.SetActive(isVisible);
        }
    }

    public void SetVectorVelocityXVisibility(bool isVisible)
    {
        if (positionVectorVelocityX)
        {
            positionVectorVelocityX.gameObject.SetActive(isVisible);
        }
    }

    public bool GetVectorVelocityXVisibility() 
    {
        if (positionVectorVelocityX)
        {
            return positionVectorVelocityX.gameObject.activeInHierarchy;
        }
        return false;
    }

    public void SetVectorVelocityYVisibility(bool isVisible)
    {
        if (positionVectorVelocityY)
        {
            positionVectorVelocityY.gameObject.SetActive(isVisible);
        }
    }

    public bool GetVectorVelocityYVisibility() 
    {
        if (positionVectorVelocityY)
        {
            return positionVectorVelocityY.gameObject.activeInHierarchy;
        }
        return false;
    }

    public void SetVectorVelocityZVisibility(bool isVisible)
    {
        if (positionVectorVelocityZ)
        {
            positionVectorVelocityZ.gameObject.SetActive(isVisible);
        }
    }

    public bool GetVectorVelocityZVisibility() 
    {
        if (positionVectorVelocityZ)
        {
            return positionVectorVelocityZ.gameObject.activeInHierarchy;
        }
        return false;
    }

    public void SetVectorVelocityTotalVisibility(bool isVisible)
    {
        if (positionVectorVelocityTotal)
        {
            positionVectorVelocityTotal.gameObject.SetActive(isVisible);
        }
    }

    public bool GetVectorVelocityTotalVisibility() 
    {
        if (positionVectorVelocityTotal)
        {
            return positionVectorVelocityTotal.gameObject.activeInHierarchy;
        }
        return false;
    }

    public void SetCoordinateTriadVisibility(bool isVisible)
    {
        if (positionCoordinateTriad)
        {
            positionCoordinateTriad.gameObject.SetActive(isVisible);
        }
    }

    public void SetOrbitVisibility(bool isVisible)
    {
        if (orbit)
        {
            orbit.gameObject.SetActive(isVisible);
        }
    }

    public void InstantiateAllPrefabs()
    {
        if (bodyPrefab)
        {
            positionBody = Instantiate(bodyPrefab, transform).transform;
            positionBody.name = "Body 1";
        }

        if (vectorVelocityXPrefab)
        {
            positionVectorVelocityX = Instantiate(vectorVelocityXPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            positionVectorVelocityX.SetPositions(Vector3.zero, Vector3.zero);
            positionVectorVelocityX.name = "Velocity X Vector";
        }

        if (vectorVelocityYPrefab)
        {
            positionVectorVelocityY = Instantiate(vectorVelocityYPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            positionVectorVelocityY.SetPositions(Vector3.zero, Vector3.zero);
            positionVectorVelocityY.name = "Velocity Y Vector";
        }

        if (vectorVelocityZPrefab)
        {
            positionVectorVelocityZ = Instantiate(vectorVelocityZPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            positionVectorVelocityZ.SetPositions(Vector3.zero, Vector3.zero);
            positionVectorVelocityZ.name = "Velocity Z Vector";
        }

        if (vectorVelocityTotalPrefab)
        {
            positionVectorVelocityTotal = Instantiate(vectorVelocityTotalPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Vector>();
            positionVectorVelocityTotal.SetPositions(Vector3.zero, Vector3.zero);
            positionVectorVelocityTotal.name = "Velocity Total Vector";
        }

        if (coordinateTriadPrefab)
        {
            positionCoordinateTriad = Instantiate(coordinateTriadPrefab, transform).transform;
            positionCoordinateTriad.position = new Vector3(-7, -2, 0);
            positionCoordinateTriad.localScale = new Vector3(1f, 1f, 1f);
            positionCoordinateTriad.name = "Coordinate Triad";
        }

        if (orbitPrefab)
        {
            orbit = Instantiate(orbitPrefab, transform).GetComponent<LineRenderer>();
            orbit.positionCount = 0;
            orbit.name = "Orbit";
        }
    }

    public void UpdateVectors(float velocityX, float velocityY, float velocityZ, Vector3 velocityTotal)
    {
        if (positionVectorVelocityX)
        {
            positionVectorVelocityX.SetPositions(positionBody.position, positionBody.position+Vector3.right*velocityX);
            positionVectorVelocityX.Redraw();
        }

        if (positionVectorVelocityY)
        {
            positionVectorVelocityY.SetPositions(positionBody.position, positionBody.position+Vector3.up*velocityY);
            positionVectorVelocityY.Redraw();
        }

        if (positionVectorVelocityZ)
        {
            positionVectorVelocityZ.SetPositions(positionBody.position, positionBody.position+Vector3.forward*velocityZ);
            positionVectorVelocityZ.Redraw();
        }

        if (positionVectorVelocityTotal)
        {
            positionVectorVelocityTotal.SetPositions(positionBody.position, positionBody.position+velocityTotal);
            positionVectorVelocityTotal.Redraw();
        }
    }
}
