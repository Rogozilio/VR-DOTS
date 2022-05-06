using Unity.Entities;
using Unity.Mathematics;

namespace DOTS.Components
{
    [GenerateAuthoringComponent]
    public struct DefaultInverseInertia : IComponentData
    {
        public float3 value;
    }
}