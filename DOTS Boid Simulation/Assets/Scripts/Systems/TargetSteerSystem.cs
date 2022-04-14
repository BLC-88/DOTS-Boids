using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class TargetSteerSystem : SystemBase
{
    Entity target;

    protected override void OnStartRunning()
    {
        EntityQuery targetQuery = GetEntityQuery(ComponentType.ReadOnly<BoidTargetData>());
        target = targetQuery.GetSingletonEntity();
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entity t = target;

        Entities.ForEach((
            ref VelocityData vel,
            ref TargetSteerData targetSteerData,
            in Translation trans,
            in BoidData boidData
            ) =>
        {
            if (t == Entity.Null)
            {
                return;
            }
            else
            {
                targetSteerData.target = t;
            }

            ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(true);
            Translation targetPos = allTranslations[targetSteerData.target];

            float3 v = math.normalizesafe(targetPos.Value - trans.Value);
            float3 dir = v.ClampMagnitude(boidData.maxSteerForce);
            vel.velocity += dir * targetSteerData.targetWeight * deltaTime;

        }).ScheduleParallel();
    }
}