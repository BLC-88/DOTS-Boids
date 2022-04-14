using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public struct MovespeedData : IComponentData
{
    public float Value;
}

public struct DirectionData : IComponentData
{
    public float3 Value;
}

public class MovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        /*
        Entities.ForEach((
            ref Translation trans,
            in MovespeedData moveSpeed,
            in DirectionData direction
            ) =>
        {
            trans.Value += direction.Value * moveSpeed.Value * deltaTime;

        }).ScheduleParallel();*/

        Entities.ForEach((ref Translation trans) =>
        {
            // job code
        }).ScheduleParallel();
    }
}
