using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
/*
public class SeperationSystem : JobComponentSystem 
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) 
    {
        float deltaTime = Time.DeltaTime;

        EntityQuery translationEQ = GetEntityQuery(ComponentType.ReadOnly<SeparationData>(), ComponentType.ReadOnly<Translation>());
        NativeArray<Translation> allTrans = translationEQ.ToComponentDataArray<Translation>(Allocator.TempJob);
        
        JobHandle myJob = Entities.ForEach((
            ref VelocityData vel,
            in Translation trans, 
            in BoidData boidData,
            in SeparationData separationData, 
            in ObservationData observationData, 
            in FaceTargetData faceTargetData
            ) => 
        {
            for (int i = 0; i < allTrans.Length; i++) 
            {
                if (trans.Value.Equals(allTrans[i].Value))
                {
                    continue;
                }
                if (math.distance(trans.Value, allTrans[i].Value) <= observationData.observationDistance) 
                {
                    //float3 dir = trans.Value - allTrans[i].Value;
                    //quaternion targetRot = quaternion.LookRotationSafe(dir, math.up());
                    //rot.Value = math.slerp(rot.Value, targetRot, seperationData.seperationWeight * deltaTime);
                    float3 v = math.normalize(trans.Value - allTrans[i].Value) * boidData.maxSpeed - vel.velocity;
                    //float3 dir = v.ClampMagnitude(boidData.maxSteerForce);
                    vel.velocity += v * separationData.separationWeight * deltaTime;
                }
            }

        }).WithDeallocateOnJobCompletion(allTrans).Schedule(inputDeps);

        return myJob;
    }
}
*/

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