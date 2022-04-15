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
            in BoidData boidData,
            in LocalToWorld ltw
            ) => 
        {
            float3 dir = math.normalizesafe(velocityData.velocity);
            float speed = math.clamp(velocityData.velocity.Magnitude(), boidData.minSpeed, boidData.maxSpeed);
            
            trans.Value += dir * speed * deltaTime;
            rot.Value = quaternion.LookRotationSafe(dir, math.up());
            
            /*
            quaternion dirRot = quaternion.LookRotation(dir, math.up());
            trans.Value += ltw.Forward * speed * deltaTime;
            rot.Value = math.slerp(rot.Value, dirRot, boidData.maxSteerForce * deltaTime);
            */
        }).ScheduleParallel();
    }
}
