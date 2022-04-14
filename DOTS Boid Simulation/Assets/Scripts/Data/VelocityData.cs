using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct VelocityData : IComponentData
{
    public float3 velocity;
}
