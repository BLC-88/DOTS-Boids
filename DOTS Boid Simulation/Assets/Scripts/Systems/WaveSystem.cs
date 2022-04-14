using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class WaveSystem : SystemBase 
{
    protected override void OnUpdate() 
    {
        float elapsedTime = (float)Time.ElapsedTime;
        
        Entities.ForEach((
            ref Translation trans, 
            ref WaveData waveData
            ) => 
        {
            if (waveData.instantiated == false) 
            {
                waveData.instantiated = true;
                waveData.startPos = trans.Value;
            }

            float xPos = waveData.amplitude * math.sin(elapsedTime * waveData.moveSpeed + trans.Value.y * waveData.yOffset + trans.Value.z * waveData.zOffset);
            float yPos = waveData.amplitude * math.sin(elapsedTime * waveData.moveSpeed + trans.Value.x * waveData.xOffset + trans.Value.z * waveData.zOffset);
            float zPos = waveData.amplitude * math.sin(elapsedTime * waveData.moveSpeed + trans.Value.x * waveData.xOffset + trans.Value.y * waveData.yOffset);
            trans.Value = waveData.startPos + new float3(xPos, yPos, zPos);

        }).ScheduleParallel();
    }
}
