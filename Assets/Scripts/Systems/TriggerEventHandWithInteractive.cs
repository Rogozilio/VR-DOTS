using Components;
using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(MoveHandSystem))]
    public class TriggerEventHandWithInteractive : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        protected override void OnCreate()
        {
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        private struct ApplicationJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<InputControllerComponent> handGroup;
            public ComponentDataFromEntity<Interactive> interactiveGroup;
            public ComponentDataFromEntity<Translation> translate;
            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityHand = Entity.Null;
                Entity entityInHand = Entity.Null;
                if (handGroup.HasComponent(triggerEvent.EntityA)
                    && interactiveGroup.HasComponent(triggerEvent.EntityB))
                {
                    entityHand = triggerEvent.EntityA;
                    entityInHand = triggerEvent.EntityB;
                }
                else if (handGroup.HasComponent(triggerEvent.EntityB)
                         && interactiveGroup.HasComponent(triggerEvent.EntityA))
                {
                    entityHand = triggerEvent.EntityB;
                    entityInHand = triggerEvent.EntityA;
                }
                else
                {
                    return;
                }
                var hand = handGroup[entityHand];
                var interactive = interactiveGroup[entityInHand];

                interactive.nearHand = hand.handType;
                interactive.distance = math.distance(translate[entityHand].Value, translate[entityInHand].Value);
                
                handGroup[entityHand] = hand;
                interactiveGroup[entityInHand] = interactive;
            }
        }
        protected override void OnUpdate()
        {
            var applicationJob = new ApplicationJob
            {
                handGroup = GetComponentDataFromEntity<InputControllerComponent>(),
                interactiveGroup = GetComponentDataFromEntity<Interactive>(),
                translate = GetComponentDataFromEntity<Translation>(),
            };
            applicationJob.Schedule(_stepPhysicsWorld.Simulation,
                ref _buildPhysicsWorld.PhysicsWorld, Dependency).Complete();
        }
    }
}