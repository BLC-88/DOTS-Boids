using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BoidData : IComponentData
{
    public float minSpeed;
    public float maxSpeed;
    public float maxSteerForce;
}
