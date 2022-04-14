using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SpawnOverTime : MonoBehaviour {

    [SerializeField] GameObject prefab;
    [SerializeField] float timeInterval = 0.5f;
    [SerializeField] float spacing = 1f;
    float elapsedTime = 0f;

    private Entity entityPrefab;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    void OnEnable() 
    {
        blobAssetStore = new BlobAssetStore();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
    }

    void Update() 
    {
        if (elapsedTime <= 0) 
        {
            Vector3 spawnPos = transform.position + UnityEngine.Random.insideUnitSphere * spacing;
            //Instantiate(prefab, spawnPos, Quaternion.identity);
            InstantiateEntity(new float3(spawnPos.x, spawnPos.y, spawnPos.z));
            elapsedTime = timeInterval;
        }
        elapsedTime -= Time.deltaTime;
    }

    private void InstantiateEntity(float3 position) 
    {
        Entity myEntity = entityManager.Instantiate(entityPrefab);
        entityManager.SetComponentData(myEntity, new Translation 
        {
            Value = position
        });
    }

    void OnDisable() 
    {
        blobAssetStore.Dispose();
    }
}
