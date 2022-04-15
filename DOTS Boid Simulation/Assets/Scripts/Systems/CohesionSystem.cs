using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class CohesionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        EntityQuery translationEQ = GetEntityQuery(ComponentType.ReadOnly<CohesionData>(), ComponentType.ReadOnly<Translation>());
        NativeArray<Translation> allTrans = translationEQ.ToComponentDataArray<Translation>(Allocator.TempJob);

        Entities.ForEach((
            ref VelocityData vel,
            in CohesionData cohesionData,
            in Translation trans,
            in BoidData boidData,
            in ObservationData observationData
            ) =>
        {
            int nearbyCount = 1;
            float3 centerOfMass = trans.Value;
            for (int i = 0; i < allTrans.Length; i++)
            {
                if (trans.Value.Equals(allTrans[i].Value))
                {
                    continue;
                }
                if (math.distance(trans.Value, allTrans[i].Value) < observationData.observationDistance)
                {
                    centerOfMass += allTrans[i].Value;
                    nearbyCount++;
                }
            }
            centerOfMass /= nearbyCount + 1;
            float3 v = (centerOfMass - trans.Value);
            float3 dir = v.ClampMagnitude(boidData.maxSteerForce);
            vel.velocity += v * cohesionData.cohesionWeight * deltaTime;

        }).ScheduleParallel();

        allTrans.Dispose();
    }
}