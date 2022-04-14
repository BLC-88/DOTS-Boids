using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class BoidSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((
            ref Translation trans,
            ref Rotation rot,
            in VelocityData velocityData,
            in BoidData boidData
            ) => 
        {
            float3 dir = math.normalizesafe(velocityData.velocity);
            float speed = math.clamp(velocityData.velocity.Magnitude(), boidData.minSpeed, boidData.maxSpeed);

            trans.Value += dir * speed * deltaTime;
            rot.Value = quaternion.LookRotationSafe(dir, math.up());

        }).ScheduleParallel();
    }
}
