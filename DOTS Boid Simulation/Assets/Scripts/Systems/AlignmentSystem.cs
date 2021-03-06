using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
public class AlignmentSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        EntityQuery translationEQ = GetEntityQuery(ComponentType.ReadOnly<AlignmentData>(), ComponentType.ReadOnly<Translation>());
        NativeArray<Translation> allTrans = translationEQ.ToComponentDataArray<Translation>(Allocator.TempJob);

        EntityQuery velocitiesEQ = GetEntityQuery(ComponentType.ReadOnly<AlignmentData>(), ComponentType.ReadOnly<VelocityData>());
        NativeArray<VelocityData> allVelocities = velocitiesEQ.ToComponentDataArray<VelocityData>(Allocator.TempJob);

        Entities.ForEach((
            ref VelocityData vel,
            in AlignmentData alignmentData,
            in Translation trans,
            in BoidData boidData,
            in ObservationData observationData
            ) =>
        {
            float3 alignmentSum = float3.zero;
            int nearbyCount = 1;
            for (int i = 0; i < allTrans.Length; i++)
            {
                if (trans.Value.Equals(allTrans[i].Value))
                {
                    continue;
                }
                if (math.distance(trans.Value, allTrans[i].Value) < observationData.observationDistance)
                {
                    alignmentSum += allVelocities[i].velocity;
                    nearbyCount++;
                }
            }
            float3 v = (alignmentSum / nearbyCount);
            float3 dir = v.ClampMagnitude(boidData.maxSteerForce);
            vel.velocity += dir * alignmentData.alignmentWeight * deltaTime;

        }).ScheduleParallel();

        allTrans.Dispose();
        allVelocities.Dispose();
    }
}