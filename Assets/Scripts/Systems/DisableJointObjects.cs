using Components;
using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(GripSelectedObjectSystem))]
    public class DisableJointObjects : SystemBase
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
            var interactiveGroup = GetComponentDataFromEntity<Interactive>();
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            
            Entities.ForEach((Entity entity, ref PhysicsConstrainedBodyPair bodyPair) =>
            {
                //Hand -> object
                if (handInput.HasComponent(bodyPair.EntityA))
                {
                    if (!handInput[bodyPair.EntityA].IsGripPressed)
                    {
                        var interactive = interactiveGroup[bodyPair.EntityB];
                        interactive.withHand = JointState.Off;
                        interactiveGroup[bodyPair.EntityB] = interactive;
                        
                        cbs.DestroyEntity(entity);
                    }
                    return;
                }

                //Object -> object
                if (interactiveGroup.HasComponent(bodyPair.EntityA)
                    && interactiveGroup.HasComponent(bodyPair.EntityB))
                {
                    var interactiveA = interactiveGroup[bodyPair.EntityA];
                    var interactiveB = interactiveGroup[bodyPair.EntityB];
                    if (interactiveA.withHand == JointState.On &&
                        interactiveB.withHand == JointState.On)
                    {
                        cbs.DestroyEntity(entity);
                    }
                }
            }).Schedule();
        }
    }
}