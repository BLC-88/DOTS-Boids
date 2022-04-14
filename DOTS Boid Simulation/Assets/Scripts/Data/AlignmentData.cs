using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct AlignmentData : IComponentData 
{
    public float alignmentWeight;
}
