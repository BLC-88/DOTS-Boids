using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class FaceTargetSystem : SystemBase 
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
            ref Rotation rot, 
            ref FaceTargetData faceTargetData, 
            in Translation trans
            ) => 
        {
            if (t == Entity.Null) 
            {
                return;
            }
            else 
            {
                faceTargetData.target = t;
            }

            ComponentDataFromEntity<Translation> allTranslations = GetComponentDataFromEntity<Translation>(true);
            Translation targetPos = allTranslations[faceTargetData.target];
            float3 dir = targetPos.Value - trans.Value;

            quaternion targetRot = quaternion.LookRotation(dir, math.up());
            rot.Value = math.slerp(rot.Value, targetRot, faceTargetData.turnSpeed * deltaTime);

        }).ScheduleParallel();
    }
}
