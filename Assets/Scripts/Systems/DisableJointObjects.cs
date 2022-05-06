using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GripSelectedObjectSystem))]
    public partial class DisableJointObjects : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var handInput = GetComponentDataFromEntity<InputControllerComponent>();
            var interactiveGroup = GetComponentDataFromEntity<InteractiveComponent>();
            var jointGroup = GetComponentDataFromEntity<JointGroup>();
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var jointGroupQuery = GetEntityQuery(typeof(InteractiveComponent))
                .ToEntityArray(Allocator.TempJob);
            var entityHand = GetEntityQuery(typeof(InputControllerComponent))
                .ToComponentDataArray<InputControllerComponent>(Allocator.TempJob);

            Entities.ForEach((Entity entity, ref PhysicsConstrainedBodyPair bodyPair) =>
            {
                //Hand -> object
                if (handInput.HasComponent(bodyPair.EntityA))
                {
                    if (!handInput[bodyPair.EntityA].IsGripPressed)
                    {
                        cbs.DestroyEntity(entity);
                    }

                    return;
                }

                // Object -> object
                if (interactiveGroup.HasComponent(bodyPair.EntityA)
                    && interactiveGroup.HasComponent(bodyPair.EntityB))
                {
                    if (entityHand[0].inHand == bodyPair.EntityA &&
                        entityHand[1].inHand == bodyPair.EntityB ||
                        entityHand[0].inHand == bodyPair.EntityB &&
                        entityHand[1].inHand == bodyPair.EntityA)
                    {
                        for (var i = 0; i < jointGroupQuery.Length; i++)
                        {
                            var joint = jointGroup[jointGroupQuery[i]];
                            if (joint.index != 0)
                                joint.isOriginIndex = true;
                            jointGroup[jointGroupQuery[i]] = joint;
                        }

                        cbs.DestroyEntity(entity);
                    }
                }
            }).Schedule(Dependency).Complete();
            jointGroupQuery.Dispose();
            entityHand.Dispose();
        }
    }
}