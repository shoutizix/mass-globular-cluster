using UnityEngine;

public class OneBodySimulation : Simulation
{
    public float frequency;
    public float amplitude;
    public float vectorLength;
    private OneBodyPrefabs prefabs;

    private float x, y, z;
    Vector3 velocityTotal;
    private float BOUNDARY = 25f; 
    private Vector3 positionTriad = Vector3.zero;
    private Camera currCamera;

    private void Awake() 
    {
        if (!TryGetComponent(out prefabs))
        {
            Debug.LogWarning("No OneBodyPrefabs component found");
            Pause();
            return;
        }
        prefabs.InstantiateAllPrefabs();
    }

    private void Start() 
    {
        currCamera = Camera.main;
        if (prefabs.orbit) 
        {
            int numSteps = 360;

            Vector3[] positions = new Vector3[numSteps];
            for (int i = 0; i < numSteps; i++)
            {
                float theta = i * 2f * Mathf.PI / numSteps;
                float r = amplitude;
                positions[i] = r * (Mathf.Cos(theta) * Vector3.right + Mathf.Sin(theta) * Vector3.up + Mathf.Cos(theta) * Vector3.forward);
            }

            prefabs.orbit.positionCount = numSteps;
            prefabs.orbit.SetPositions(positions);
            prefabs.orbit.loop = true;
        }
    }

    private void FixedUpdate() 
    {
        Vector3 currPos = prefabs.positionBody.position;
        
        x = Mathf.Cos(Time.time * frequency) * amplitude;
        y = Mathf.Sin(Time.time * frequency) * amplitude;
        z = Mathf.Cos(Time.time * frequency) * amplitude;

        Vector3 newPos = new Vector3(x, y, z);

        prefabs.positionBody.position = newPos;

        Vector3 diff = (newPos-currPos);
        velocityTotal = vectorLength * diff;
        // Bound Velocity Vector 
        if (velocityTotal.magnitude > BOUNDARY)
        {
            velocityTotal = Vector3.zero;
        }

    }
    void Update()
    {
        if(!prefabs)
        {
            return;
        }
        prefabs.UpdateVectors(velocityTotal.x, velocityTotal.y, velocityTotal.z, velocityTotal);
        prefabs.UpdateTriad(prefabs.GetNewPosTriad(currCamera));
    }
}
