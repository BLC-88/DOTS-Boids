using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct WaveData : IComponentData
{
    public float moveSpeed;
    public float amplitude;
    public float xOffset;
    public float yOffset;
    public float zOffset;
    public float3 startPos;
    public bool instantiated;
}
