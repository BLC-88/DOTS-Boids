using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveSystem : SystemBase 
{
    protected override void OnUpdate() 
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((
            ref Translation trans, 
            in MoveSpeedData moveSpeed, 
            in LocalToWorld ltw
            ) => 
        {
            trans.Value += ltw.Forward * moveSpeed.Value * deltaTime;

        }).ScheduleParallel();
    }
}
