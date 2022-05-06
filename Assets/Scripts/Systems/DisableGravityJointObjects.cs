using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public class DisableGravityJointObjects : ComponentSystem
    {
        private EndFixedStepSimulationEntityCommandBufferSystem
            _endFixedStepSimulationEntityCommandBufferSystem;

        protected override void OnStartRunning()
        {
            _endFixedStepSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
            var cbs = _endFixedStepSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            Entities
                .WithAll<InteractiveComponent>()
                .WithNone<PhysicsGravityFactor>()
                .ForEach((entity) => { cbs.AddComponent(entity, new PhysicsGravityFactor()); });
            Entities.ForEach((Entity entity, ref PhysicsMass mass) =>
            {
                cbs.AddComponent(entity,
                    new DefaultInverseInertia() { value = mass.InverseInertia });
            });
        }

        protected override void OnUpdate()
        {
            Entities.ForEach(
                (ref PhysicsGravityFactor gravity,
                    ref InteractiveComponent interactive, ref PhysicsMass mass, ref DefaultInverseInertia defaultInverseInertia) =>
                {
                    if (interactive.inHand == HandType.None)
                    {
                        gravity.Value = 1;
                        mass.InverseInertia = defaultInverseInertia.value;
                    }
                    else
                    {
                        gravity.Value = 0;
                        mass.InverseInertia = new float3(1, 1, 1);
                    }
                });
        }
    }
}