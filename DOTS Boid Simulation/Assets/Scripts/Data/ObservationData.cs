using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct ObservationData : IComponentData
{
    public float observationDistance;
}
