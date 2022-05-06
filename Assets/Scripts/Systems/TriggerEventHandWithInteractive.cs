using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.GraphicsIntegration;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(CopyPhysicsVelocityToSmoothing))]
    public partial class TriggerEventHandWithInteractive : SystemBase
    {
        private StepPhysicsWorld _stepPhysicsWorld;
        private EndFixedStepSimulationEntityCommandBufferSystem commandBufferSystem;

        protected override void OnCreate()
        {
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            commandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        }
        private struct ApplicationJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<InputControllerComponent> handGroup;
            [ReadOnly] public ComponentDataFromEntity<Translation> translate;
            public ComponentDataFromEntity<InteractiveComponent> interactiveGroup;
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
                if(handGroup[entityHand].isJoint)
                    return;
                var interactive = interactiveGroup[entityInHand];
                interactive.nearHand = handGroup[entityHand].handType;
                interactive.distance = math.distance(translate[entityHand].Value,
                    translate[entityInHand].Value);
                interactiveGroup[entityInHand] = interactive;
            }
        }
        protected override void OnUpdate()
        {
            var applicationJob = new ApplicationJob
            {
                handGroup = GetComponentDataFromEntity<InputControllerComponent>(),
                interactiveGroup = GetComponentDataFromEntity<InteractiveComponent>(),
                translate = GetComponentDataFromEntity<Translation>(),
            };
            Dependency = applicationJob.Schedule(_stepPhysicsWorld.Simulation, Dependency);
            commandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}