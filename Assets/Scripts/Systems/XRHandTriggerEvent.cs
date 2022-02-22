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
    public class XRHandTriggerEvent : JobComponentSystem
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        private Entity _entityHands;
        protected override void OnCreate()
        {
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        private struct ApplicationJob : ITriggerEventsJob
        {
            public Entity entityHands;
            public ComponentDataFromEntity<XRHandInputControllerComponent> handGroup;
            public ComponentDataFromEntity<Interactive> interactiveGroup;
            public ComponentDataFromEntity<Translation> translate;
            public ComponentDataFromEntity<HandsObjectComponent> handsGroup;
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
                var hands = handsGroup[entityHands];
                var interactive = interactiveGroup[entityInHand];

                interactive.inHand = hand.handType;
                interactive.distance = math.distance(translate[entityHand].Value, translate[entityInHand].Value);
                
                if (!hand.isOccupied && hand.selectValue > 0.9f && interactive.inHand == HandType.None)
                {
                    hand.isOccupied = true;
                    interactive.Hand = entityHand;
                    if (hand.handType == HandType.Left)
                        hands.objectInLeftHand = entityInHand;
                    else if (hand.handType == HandType.Right) 
                        hands.objectInRightHand = entityInHand;
                }
                else if (hand.selectValue > 0.9f && interactive.inHand != HandType.None)
                {
                    if (hand.activateValue > 0.9f) 
                        hands.activeHand = hand.handType;
                    else {
                        if (hand.handType == hands.activeHand) {
                            Entity ghost = Entity.Null;
                            if (hands.activeHand == HandType.Left) 
                                ghost = interactiveGroup[hands.objectInLeftHand].ghost; 
                            else if (hands.activeHand == HandType.Right) 
                                ghost = interactiveGroup[hands.objectInRightHand].ghost; 
                            var pos = translate[ghost];
                            pos.Value = float3.zero;
                            translate[ghost] = pos;
                            hands.activeHand = HandType.None; 
                        } 
                    }
                }
                else if (hand.isOccupied && hand.selectValue < 0.9f && interactive.inHand != HandType.None)
                {
                    hand.isOccupied = false;
                    interactive.inHand = HandType.None;
                    if (hand.handType == hands.activeHand)
                    {
                        hands.isReadyJoint = true;
                    }
                }
                handGroup[entityHand] = hand;
                interactiveGroup[entityInHand] = interactive;
                handsGroup[entityHands] = hands;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            Entities.WithAll<HandsObjectComponent>().ForEach((Entity e) =>
            {
                _entityHands = e;
            }).WithoutBurst().Run();
            var applicationJob = new ApplicationJob
            {
                entityHands = _entityHands,
                handGroup = GetComponentDataFromEntity<XRHandInputControllerComponent>(),
                interactiveGroup = GetComponentDataFromEntity<Interactive>(),
                translate = GetComponentDataFromEntity<Translation>(),
                handsGroup = GetComponentDataFromEntity<HandsObjectComponent>(),
            };
            return applicationJob.Schedule(_stepPhysicsWorld.Simulation,
                ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        }
    }
}