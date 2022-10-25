using UnityEngine;

public class OneBodySimulation : Simulation
{

    public float frequency;
    public float amplitude;
    public float vectorLength;
    private OneBodyPrefabs prefabs;

    private float x, y, z;
    Vector3 velocityTotal;

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

    private void FixedUpdate() 
    {
        Vector3 currPos = prefabs.positionBody.position;
        
        x = Mathf.Cos(Time.time * frequency) * amplitude;
        y = Mathf.Sin(Time.time * frequency) * amplitude;
        z = Mathf.Cos(Time.time * frequency) * amplitude;

        Vector3 newPos = new Vector3(x, y, z);

        prefabs.positionBody.position = newPos;

        velocityTotal = vectorLength * (newPos-currPos);
    }
    void Update()
    {
        if(!prefabs)
        {
            return;
        }
        prefabs.UpdateVectors(velocityTotal.x, velocityTotal.y, velocityTotal.z, velocityTotal);
    }
}
