using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public class BoidJobs : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float minSpeed = 2;
    [SerializeField] float maxSpeed = 5;
    [SerializeField] float sightRadius = 2.5f;
    [SerializeField] float maxSteerForce = 3;

    [Header("Weights")]
    public float separateWeight = 1;
    public float alignWeight = 1;
    public float cohesionWeight = 1;
    public float targetWeight = 1;

    Transform target;
    float3 targetPos;
    List<GameObject> allBoids = new List<GameObject>();

    NativeList<float3> allBoidsPosition = new NativeList<float3>();
    NativeList<float3> allBoidsVelocities = new NativeList<float3>();

    void Start()
    {
        allBoids = FindObjectOfType<BoidSpawner>().boids;
        target = FindObjectOfType<BoidTarget>().transform;
        
        allBoidsPosition = new NativeList<float3>(allBoids.Count, Allocator.Persistent);
        allBoidsVelocities = new NativeList<float3>(allBoids.Count, Allocator.Persistent);
    }

    void Update()
    {

        if (target != null)
        {
            targetPos = target.position;
        }
        else
        {
            targetPos = float3.zero;
        }
        
        for (int i = 0; i < allBoids.Count; i++)
        {
            if (i >= allBoidsPosition.Length)
            {
                allBoidsPosition.Add(allBoids[i].transform.position);
                allBoidsVelocities.Add(float3.zero);
            }
            else
            {
                allBoidsPosition[i] = allBoids[i].transform.position;
            }
        }
        
        //Seperation job
        SeparationJob seperationJob = new SeparationJob
        {
            allBoidsPositions = allBoidsPosition,
            allBoidsVelocities = allBoidsVelocities,
            sightRadius = sightRadius,
            maxSteerForce = maxSteerForce,
            separateWeight = separateWeight,
            deltaTime = Time.deltaTime
        };
        JobHandle seperationJobHandle = seperationJob.Schedule(allBoids.Count, 100);
        seperationJobHandle.Complete();
        
        //Alignment job
        AlignmentJob alignmentJob = new AlignmentJob
        {
            allBoidsPositions = allBoidsPosition,
            allBoidsVelocities = allBoidsVelocities,
            sightRadius = sightRadius,
            maxSteerForce = maxSteerForce,
            alignWeight = alignWeight,
            deltaTime = Time.deltaTime
        };
        JobHandle alignmentJobHandle = alignmentJob.Schedule(allBoids.Count, 100);
        alignmentJobHandle.Complete();
        
        //Cohesion job
        CohesionJob cohesionJob = new CohesionJob
        {
            allBoidsPositions = allBoidsPosition,
            allBoidsVelocities = allBoidsVelocities,
            sightRadius = sightRadius,
            maxSteerForce = maxSteerForce,
            cohesionWeight = cohesionWeight,
            deltaTime = Time.deltaTime
        };
        JobHandle cohesionJobHandle = cohesionJob.Schedule(allBoids.Count, 100);
        cohesionJobHandle.Complete();

        //Target Steer job
        TargetSteerJob targetSteerJob = new TargetSteerJob
        {
            allBoidsPositions = allBoidsPosition,
            allBoidsVelocities = allBoidsVelocities,
            targetPos = targetPos,
            maxSteerForce = maxSteerForce,
            targetWeight = targetWeight,
            deltaTime = Time.deltaTime
        };
        JobHandle targetSteerJobHandle = targetSteerJob.Schedule(allBoids.Count, 100);
        targetSteerJobHandle.Complete();

        //Move job
        MoveJob moveJob = new MoveJob
        {
            allBoidsPositions = allBoidsPosition,
            allBoidsVelocities = allBoidsVelocities,
            minSpeed = minSpeed,
            maxSpeed = maxSpeed,
            deltaTime = Time.deltaTime
        };
        JobHandle moveJobHandle = moveJob.Schedule(allBoids.Count, 100);
        moveJobHandle.Complete();

        //Update positions of boids
        for (int i = 0; i < allBoids.Count; i++)
        {
            allBoids[i].transform.position = allBoidsPosition[i];
            allBoids[i].transform.rotation = quaternion.LookRotationSafe(math.normalizesafe(allBoidsVelocities[i]), math.up());
        }
    }
    
    private void OnDisable()
    {
        allBoidsPosition.Dispose();
        allBoidsVelocities.Dispose();
    }
}

[BurstCompile]
public struct MoveJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> allBoidsPositions;
    [ReadOnly]
    public NativeArray<float3> allBoidsVelocities;

    public float minSpeed;
    public float maxSpeed;
    public float deltaTime;

    public void Execute(int index)
    {
        float3 dir = math.normalizesafe(allBoidsVelocities[index]);
        float speed = math.clamp(allBoidsVelocities[index].Magnitude(), minSpeed, maxSpeed);

        allBoidsPositions[index] += dir * speed * deltaTime;
    }
}

[BurstCompile]
public struct SeparationJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float3> allBoidsPositions;
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> allBoidsVelocities;

    public float sightRadius;
    public float maxSteerForce;
    public float separateWeight;
    public float deltaTime;

    public void Execute(int index)
    {
        if (allBoidsPositions.Length > 0)
        {
            float3 seperationSum = float3.zero;
            int nearbyCount = 1;
            for (int i = 0; i < allBoidsPositions.Length; i++)
            {
                if (allBoidsPositions[i].Equals(allBoidsPositions[index]))
                {
                    continue;
                }
                if (math.distance(allBoidsPositions[index], allBoidsPositions[i]) < sightRadius)
                {
                    seperationSum += allBoidsPositions[index] - allBoidsPositions[i];
                    nearbyCount++;
                }
            }
            float3 v = (seperationSum / nearbyCount);
            float3 dir = v.ClampMagnitude(maxSteerForce);
            allBoidsVelocities[index] += dir * separateWeight * deltaTime;
        }
    }
}

[BurstCompile]
public struct AlignmentJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float3> allBoidsPositions;
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> allBoidsVelocities;

    public float sightRadius;
    public float maxSteerForce;
    public float alignWeight;
    public float deltaTime;

    public void Execute(int index)
    {
        if (allBoidsPositions.Length > 0)
        {
            float3 alignmentSum = float3.zero;
            int nearbyCount = 1;
            for (int i = 0; i < allBoidsPositions.Length; i++)
            {
                if (allBoidsPositions[i].Equals(allBoidsPositions[index]))
                {
                    continue;
                }
                if (math.distance(allBoidsPositions[index], allBoidsPositions[i]) < sightRadius)
                {
                    alignmentSum += allBoidsVelocities[i];
                    nearbyCount++;
                }
            }
            float3 v = (alignmentSum / nearbyCount);
            float3 dir = v.ClampMagnitude(maxSteerForce);
            allBoidsVelocities[index] += dir * alignWeight * deltaTime;
        }
    }
}

[BurstCompile]
public struct CohesionJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float3> allBoidsPositions;
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> allBoidsVelocities;

    public float sightRadius;
    public float maxSteerForce;
    public float cohesionWeight;
    public float deltaTime;

    public void Execute(int index)
    {
        if (allBoidsPositions.Length > 0)
        {
            int nearbyCount = 1;
            float3 centerOfMass = allBoidsPositions[index];
            for (int i = 0; i < allBoidsPositions.Length; i++)
            {
                if (allBoidsPositions[i].Equals(allBoidsPositions[index]))
                {
                    continue;
                }
                if (math.distance(allBoidsPositions[index], allBoidsPositions[i]) < sightRadius)
                {
                    centerOfMass += allBoidsPositions[i];
                    nearbyCount++;
                }
            }
            centerOfMass /= nearbyCount + 1;
            float3 v = (centerOfMass - allBoidsPositions[index]);
            float3 dir = v.ClampMagnitude(maxSteerForce);
            allBoidsVelocities[index] += dir * cohesionWeight * deltaTime;
        }
    }
}

[BurstCompile]
public struct TargetSteerJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<float3> allBoidsPositions;
    [NativeDisableParallelForRestriction]
    public NativeArray<float3> allBoidsVelocities;

    public float3 targetPos;
    public float maxSteerForce;
    public float targetWeight;
    public float deltaTime;

    public void Execute(int index)
    {
        float3 v = (targetPos - allBoidsPositions[index]);
        float3 dir = v.ClampMagnitude(maxSteerForce);
        allBoidsVelocities[index] += dir * targetWeight * deltaTime;
    }
}