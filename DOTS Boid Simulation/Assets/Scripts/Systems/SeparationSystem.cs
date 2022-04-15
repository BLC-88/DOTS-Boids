using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class SeparationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        EntityQuery translationEQ = GetEntityQuery(ComponentType.ReadOnly<SeparationData>(), ComponentType.ReadOnly<Translation>());
        NativeArray<Translation> allTrans = translationEQ.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities.ForEach((
            ref VelocityData vel,
            in SeparationData separationData,
            in Translation trans,
            in BoidData boidData,
            in ObservationData observationData
            ) =>
        {
            float3 separationSum = float3.zero;
            int nearbyCount = 1;
            for (int i = 0; i < allTrans.Length; i++)
            {
                if (trans.Value.Equals(allTrans[i].Value))
                {
                    continue;
                }
                if (math.distance(trans.Value, allTrans[i].Value) <= observationData.observationDistance)
                {
                    separationSum += (trans.Value - allTrans[i].Value);
                    nearbyCount++;
                }
            }
            float3 v = (separationSum / nearbyCount);
            float3 dir = v.ClampMagnitude(boidData.maxSteerForce);
            vel.velocity += dir * separationData.separationWeight * deltaTime;

        }).ScheduleParallel();

        allTrans.Dispose();
    }
}