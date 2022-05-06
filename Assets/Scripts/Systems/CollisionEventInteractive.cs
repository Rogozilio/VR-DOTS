using DOTS.Components;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(TriggerEventHandWithInteractive))]
    public partial class CollisionEventInteractive : SystemBase
    {
        private StepPhysicsWorld _stepPhysicsWorld;
        private EndFixedStepSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            commandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        }

        private struct CollisionSystem : ICollisionEventsJob
        {
            public ComponentDataFromEntity<InteractiveComponent> interactiveGroup;

            public void Execute(CollisionEvent collision)
            {
                if (!interactiveGroup.HasComponent(collision.EntityA)
                    || !interactiveGroup.HasComponent(collision.EntityB))
                    return;

                if (interactiveGroup[collision.EntityA].inHand == HandType.Left &&
                    interactiveGroup[collision.EntityB].inHand == HandType.Right ||
                    interactiveGroup[collision.EntityB].inHand == HandType.Left &&
                    interactiveGroup[collision.EntityA].inHand == HandType.Right)
                {
                    var interactiveA = interactiveGroup[collision.EntityA];
                    var interactiveB = interactiveGroup[collision.EntityB];
                    interactiveA.CollisionWith = collision.EntityB;
                    interactiveB.CollisionWith = collision.EntityA;
                    interactiveA.CollisionState = CollisionState.Yes;
                    interactiveB.CollisionState = CollisionState.Yes;
                    interactiveGroup[collision.EntityA] = interactiveA;
                    interactiveGroup[collision.EntityB] = interactiveB;
                }
            }
        }

        protected override void OnUpdate()
        {
            var CollisionSystem = new CollisionSystem
            {
                interactiveGroup = GetComponentDataFromEntity<InteractiveComponent>()
            };
            Dependency = CollisionSystem.Schedule(_stepPhysicsWorld.Simulation, Dependency);
            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}