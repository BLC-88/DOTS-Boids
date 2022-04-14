using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour {

    [SerializeField] int count = 100;
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 size = new Vector3(5, 5, 5);
    [SerializeField] bool centerPivot = true;
    [SerializeField] bool randomRot = false;
    public List<GameObject> boids = new List<GameObject>();
    
    void Awake()
    {
        count = FindObjectOfType<FPSCounter>().boidIncreaseSpawnAmount;
        SpawnRandom();
    }
    
    private void OnEnable()
    {
        FPSCounter.OnSpawnBoids += SpawnRandom;
    }

    private void OnDisable()
    {
        FPSCounter.OnSpawnBoids -= SpawnRandom;
    }

    void SpawnRandom()
    {
        FindObjectOfType<FPSCounter>().boidCount += count;
        Vector3[] spawnPositions = new Vector3[count];

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

        for (int i = 0; i < count; i++)
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
