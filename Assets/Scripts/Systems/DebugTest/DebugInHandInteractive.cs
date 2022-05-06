using DOTS.Components;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace DOTS.Systems.DebugTest
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class DebugInHandInteractive : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnStartRunning()
        {
            _endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            Entities.ForEach(
                (Entity entity, in InteractiveComponent interactive) =>
                {
                    cbs.AddComponent(entity,
                        new URPMaterialPropertyBaseColor());
                }).Schedule();
        }

        protected override void OnUpdate()
        {
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            
            Entities.ForEach((Entity entity, in InteractiveComponent interactive) =>
            {
                float4 color = new float4();
                
                if (interactive.inHand == HandType.None)
                {
                    color = new float4(1, 1, 1, 1);
                }
                else if (interactive.inHand == HandType.Left)
                {
                    color = new float4(1, 0, 0, 1);
                }
                else if (interactive.inHand == HandType.Right)
                {
                    color = new float4(0, 0, 1, 1);
                }
                else if (interactive.inHand == HandType.Both)
                {
                    color = new float4(1, 0, 1, 1);
                }

                cbs.SetComponent(entity,
                    new URPMaterialPropertyBaseColor { Value = color });
            }).Schedule(Dependency).Complete();
        }
    }
}