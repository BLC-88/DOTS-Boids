using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerOOP : MonoBehaviour
{
    [SerializeField] int spawnCount = 100;
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 size = new Vector3(5, 5, 5);
    [SerializeField] bool centerPivot = true;
    [SerializeField] bool randomRot = false;
    public List<GameObject> boids = new List<GameObject>();

    void Awake()
    {
        FPSDisplay fpsDisplay = FindObjectOfType<FPSDisplay>();
        if (fpsDisplay != null)
        {
            fpsDisplay.boidCount = spawnCount;
        }
    }

    private void OnEnable()
    {
        SpawnRandom();
    }

    void SpawnRandom()
    {
        Vector3[] spawnPositions = new Vector3[spawnCount];

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (centerPivot)
            {
                spawnPositions[i] = new Vector3(
                    Random.Range(-size.x / 2f, size.x / 2f),
                    Random.Range(-size.y / 2f, size.y / 2f),
                    Random.Range(-size.z / 2f, size.z / 2f)
                );
            }
            else
            {
                spawnPositions[i] = new Vector3(
                    Random.Range(0f, size.x),
                    Random.Range(0f, size.y),
                    Random.Range(0f, size.z)
                );
            }
            spawnPositions[i] += transform.position;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Quaternion spawnRot = Quaternion.identity;
            if (randomRot)
            {
                spawnRot = Quaternion.Euler(Random.Range(-180f, 180f), Random.Range(-180f, 180f), Random.Range(-180f, 180f));
            }
            GameObject boid = Instantiate(prefab, spawnPositions[i], spawnRot);
            boids.Add(boid);
        }
    }
}
