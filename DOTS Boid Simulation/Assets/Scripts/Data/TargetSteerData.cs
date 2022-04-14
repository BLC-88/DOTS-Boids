using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct TargetSteerData : IComponentData
{
    public Entity target;
    public float targetWeight;
}
