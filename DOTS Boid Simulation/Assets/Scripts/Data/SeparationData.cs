using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct SeparationData : IComponentData 
{
    public float separationWeight;
}
