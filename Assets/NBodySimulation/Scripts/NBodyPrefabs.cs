using System.Collections.Generic;
using UnityEngine;

public class NBodyPrefabs : MonoBehaviour
{
    [SerializeField] private List<FastNBodySlideController> slideControllers;

    [Header("Prefabs")]
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject centerOfMassPrefab;
    [SerializeField] private GameObject coordinateOriginPrefab;
    [SerializeField] private GameObject angularMomentumVectorPrefab;
    [SerializeField] private GameObject[] lightPrefabs;
    [SerializeField] private GameObject graphPrefab;
    [SerializeField] private GameObject vectorVelocityXPrefab;
    [SerializeField] private GameObject vectorVelocityYPrefab;
    [SerializeField] private GameObject vectorVelocityZPrefab;

    [HideInInspector] public List<Transform> bodies;
    [HideInInspector] public Transform centerOfMass;
    [HideInInspector] public Transform coordinateOrigin;
    [HideInInspector] public Vector angularMomentumVector;
    [HideInInspector] public List<Transform> lights;
    [HideInInspector] public Arrow positionVectorVelocityX;
    [HideInInspector] public Arrow positionVectorVelocityY;
    [HideInInspector] public Arrow positionVectorVelocityZ;

    private Transform bodyContainer;

    public void InstantiateAllPrefabs(int numBodies)
    {
        CreateBodies(numBodies);

        if (centerOfMassPrefab)
        {
            centerOfMass = Instantiate(centerOfMassPrefab, transform).transform;
            centerOfMass.name = "Center of Mass";
        }

        if (coordinateOriginPrefab)
        {
            coordinateOrigin = Instantiate(coordinateOriginPrefab, transform).transform;
            coordinateOrigin.name = "Coordinate Origin";
        }

        if (angularMomentumVectorPrefab)
        {
            angularMomentumVector = Instantiate(angularMomentumVectorPrefab, transform).GetComponent<Vector>();
            angularMomentumVector.SetPositions(Vector3.zero, Vector3.zero);
            angularMomentumVector.Redraw();
            angularMomentumVector.name = "Angular Momentum Vector";
        }

        lights = new List<Transform>();
        foreach (GameObject lightPrefab in lightPrefabs)
        {
            Transform light = Instantiate(lightPrefab, transform).transform;
            lights.Add(light);
        }

        if (vectorVelocityXPrefab)
        {
            positionVectorVelocityX = Instantiate(vectorVelocityXPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Arrow>();
            positionVectorVelocityX.SetComponents(Vector3.zero);
            positionVectorVelocityX.lineWidth = 0.12f;
            positionVectorVelocityX.name = "Velocity X Vector";
        }

        if (vectorVelocityYPrefab)
        {
            positionVectorVelocityY = Instantiate(vectorVelocityYPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Arrow>();
            positionVectorVelocityY.SetComponents(Vector3.zero);
            positionVectorVelocityY.lineWidth = 0.12f;
            positionVectorVelocityY.name = "Velocity Y Vector";
        }

        if (vectorVelocityZPrefab)
        {
            positionVectorVelocityZ = Instantiate(vectorVelocityZPrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<Arrow>();
            positionVectorVelocityZ.SetComponents(Vector3.zero);
            positionVectorVelocityZ.lineWidth = 0.12f;
            positionVectorVelocityZ.name = "Velocity Z Vector";
        }
    }

    public void SetCenterOfMassVisibility(bool visible)
    {
        if (centerOfMass)
        {
            centerOfMass.gameObject.SetActive(visible);
        }
    }

    public void SetCoordinateOriginVisibility(bool visible)
    {
        if (coordinateOrigin)
        {
            coordinateOrigin.gameObject.SetActive(visible);
        }
    }

    public void SetAngularMomentumVectorVisibility(bool visible)
    {
        if (angularMomentumVector)
        {
            angularMomentumVector.gameObject.SetActive(visible);
        }
    }

    public void SetLightsVisibility(bool visible)
    {
        foreach (Transform light in lights)
        {
            light.gameObject.SetActive(visible);
        }
    }

    public Material GetBodyMaterial()
    {
        // TODO should check that the object exists etc.
        return bodyPrefab.GetComponent<MeshRenderer>().material;
    }

    public void CreateBodies(int numBodies)
    {
        if (bodyPrefab)
        {
            bodies = new List<Transform>(numBodies);
            bodyContainer = new GameObject("Bodies").transform;
            bodyContainer.SetParent(transform);

            for (int i = 0; i < numBodies; i++)
            {
                bodies.Add(Instantiate(bodyPrefab, bodyContainer).transform);
                bodies[i].name = "Body " + i;

                InteractableBody interBody;
                if (bodies[i].gameObject.TryGetComponent(out interBody))
                {
                    interBody.SetSlideControllers(slideControllers); 
                    interBody.SetIndex(i);
                }
            }
        }
        else
        {
            Debug.LogWarning("No body prefab assigned!");
        }
    }

    public void DestroyBodies()
    {
        foreach (Transform body in bodies)
        {
            Destroy(body.gameObject);
        }

        if (bodyContainer)
        {
            Destroy(bodyContainer.gameObject);
            bodyContainer = null;
        }
    }

    public bool BodiesHaveLabels()
    {
        return bodyPrefab.transform.Find("Label") != null;
    }
}
