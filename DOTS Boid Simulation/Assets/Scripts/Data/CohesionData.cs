using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct CohesionData : IComponentData 
{
    public float cohesionWeight;
}
