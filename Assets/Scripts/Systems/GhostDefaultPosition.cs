using DOTS.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class GhostDefaultPosition : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var translate = GetComponentDataFromEntity<Translation>();
            Entities.ForEach((ref Interactive interactive) =>
            {
                if (!translate[interactive.ghost].Value.Equals(float3.zero))
                {
                    var pos = translate[interactive.ghost];
                    pos.Value = float3.zero;
                    translate[interactive.ghost] = pos;
                }
            });
        }
    }
}