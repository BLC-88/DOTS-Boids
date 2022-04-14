using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct FaceTargetData : IComponentData 
{
    public Entity target;
    public float turnSpeed;
}