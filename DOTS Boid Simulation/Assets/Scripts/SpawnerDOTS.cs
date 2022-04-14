using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnerDOTS : MonoBehaviour {

    [SerializeField] GameObject prefab;
    [SerializeField] int spawnCount;
    [SerializeField] bool spawnRandom = false;
    [SerializeField] bool randomRot = false;
    [SerializeField] Vector3 size = new Vector3(5, 5, 5);
    [SerializeField] float spacing = 1f;
    [SerializeField] bool centerPivot = true;

    private Entity entityPrefab;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    private void Awake()
    {
        FPSDisplay fpsDisplay = FindObjectOfType<FPSDisplay>();
        if (fpsDisplay != null)
        {
            fpsDisplay.boidCount = spawnCount;
        }
    }

    void OnEnable() 
    {
        blobAssetStore = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        
        if (spawnRandom)
        {
            SpawnRandom();
        }
        else
        {
            SpawnGrid();
        }
    }

    void SpawnGrid()
    {
        for (int z = 0; z < size.z; z++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    Vector3 spawnPos = transform.position + new Vector3(x * spacing, y * spacing, z * spacing);
                    if (centerPivot)
                    {
                        spawnPos.x -= size.x * spacing * 0.5f;
                        spawnPos.y -= size.y * spacing * 0.5f;
                        spawnPos.z -= size.z * spacing * 0.5f;
                    }
                    //Instantiate(prefab, spawnPos, Quaternion.identity);
                    Quaternion spawnRot = Quaternion.identity;
                    if (randomRot)
                    {
                        spawnRot = Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f));
                    }
                    InstantiateEntity(new float3(spawnPos.x, spawnPos.y, spawnPos.z), spawnRot);
                }
            }
        }
    }

    void SpawnRandom()
    {
        Vector3[] spawnPositions = new Vector3[spawnCount];

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (centerPivot)
            {
                spawnPositions[i] = new Vector3(
                    UnityEngine.Random.Range(-size.x / 2f, size.x / 2f),
                    UnityEngine.Random.Range(-size.y / 2f, size.y / 2f),
                    UnityEngine.Random.Range(-size.z / 2f, size.z / 2f)
                );
            }
            else
            {
                spawnPositions[i] = new Vector3(
                    UnityEngine.Random.Range(0f, size.x),
                    UnityEngine.Random.Range(0f, size.y),
                    UnityEngine.Random.Range(0f, size.z)
                );
            }
            spawnPositions[i] += transform.position;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            Quaternion spawnRot = Quaternion.identity;
            if (randomRot)
            {
                spawnRot = Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f));
            }
            //GameObject boid = Instantiate(prefab, spawnPositions[i], spawnRot);
            InstantiateEntity(new float3(spawnPositions[i].x, spawnPositions[i].y, spawnPositions[i].z), spawnRot);
        }
    }

    private void InstantiateEntity(float3 position, quaternion rotation) 
    {
        Entity myEntity = entityManager.Instantiate(entityPrefab);
        entityManager.SetComponentData(myEntity, new Translation 
        {
            Value = position
        });
        entityManager.SetComponentData(myEntity, new Rotation
        {
            Value = rotation
        });
    }

    void OnDisable() 
    {
        blobAssetStore.Dispose();
    }
}
