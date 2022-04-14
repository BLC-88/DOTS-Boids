using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
/*
public class CohesionSystem : JobComponentSystem 
{
    protected override JobHandle OnUpdate(JobHandle inputDeps) 
    {
        float deltaTime = Time.DeltaTime;

        EntityQuery eq = GetEntityQuery(ComponentType.ReadOnly<CohesionData>(), ComponentType.ReadOnly<Translation>());
        NativeArray<Translation> allTrans = eq.ToComponentDataArray<Translation>(Allocator.TempJob);

        JobHandle myJob = Entities.ForEach((
            ref VelocityData vel, 
            in Translation trans,
            in BoidData boidData,
            in CohesionData cohesionData, 
            in ObservationData observationData, 
            in FaceTargetData faceTargetData
            ) =>
        {
            //int nearbyCount = 0;
            float3 centerOfMass = trans.Value;
            for (int i = 0; i < allTrans.Length; i++)
            {
                if (trans.Value.Equals(allTrans[i].Value))
                {
                    continue;
                }
                //if (math.distance(trans.Value, allTrans[i].Value) < observationData.observationDistance)
                //{
                //    centerOfMass += allTrans[i].Value;
                //    nearbyCount++;
                //}
                centerOfMass += allTrans[i].Value;
            }
            centerOfMass /= allTrans.Length + 1;

            //float3 direction = centerOfMass - trans.Value;
            //quaternion targetRotation = quaternion.LookRotationSafe(direction, math.up());
            //rot.Value = math.slerp(rot.Value, targetRotation, cohesionData.cohesionWeight * deltaTime);

            float3 v = math.normalize(centerOfMass - trans.Value) * boidData.maxSpeed - vel.velocity;
            vel.velocity += v * cohesionData.cohesionWeight * deltaTime;

        }).WithDeallocateOnJobCompletion(allTrans).Schedule(inputDeps);

        return myJob;
    }
}
*/

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