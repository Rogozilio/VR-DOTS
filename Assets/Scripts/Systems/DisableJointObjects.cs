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
    [UpdateAfter(typeof(GrabSelectedObjectSystem))]
    public class DisableJointObjects : SystemBase
    {
        protected override void OnUpdate()
        {
            var handInput = GetComponentDataFromEntity<XRHandInputControllerComponent>();
            var interactive = GetComponentDataFromEntity<Interactive>();
            NativeArray<Entity> entityJointObjects = new NativeArray<Entity>(1, Allocator.TempJob);

            var disableJointObjectsJob = Entities.ForEach((Entity entity, ref PhysicsConstrainedBodyPair bodyPair) =>
            {
                //Hand -> object
                if (handInput.HasComponent(bodyPair.EntityA))
                {
                    if (!handInput[bodyPair.EntityA].isOccupied)
                        return;
                    
                    if (handInput[bodyPair.EntityA].selectValue < 0.9f)
                    {
                        var entityA = handInput[bodyPair.EntityA];
                        entityA.isOccupied = false;
                        handInput[bodyPair.EntityA] = entityA;
                        
                        var entityB = interactive[bodyPair.EntityB];
                        entityB.Hand = Entity.Null;
                        entityB.isJointed = false;
                        entityB.inHand = HandType.None;
                        interactive[bodyPair.EntityB] = entityB;
                        
                        entityJointObjects[0] = entity;
                    }

                    return;
                }

                //Object -> object
                if (interactive.HasComponent(bodyPair.EntityA)
                    && interactive.HasComponent(bodyPair.EntityB))
                {
                    var entityA = interactive[bodyPair.EntityA];
                    var entityB = interactive[bodyPair.EntityB];
                    if (entityA.inHand == HandType.Left
                        && entityB.inHand == HandType.Right
                        || entityA.inHand == HandType.Right
                        && entityB.inHand == HandType.Left)
                    {
                        entityA.isJointed = false;
                        entityB.isJointed = false;
                        entityJointObjects[0] = entity;
                    }
                }
            }).Schedule(Dependency);
            disableJointObjectsJob.Complete();
            EntityManager.DestroyEntity(entityJointObjects[0]);

            entityJointObjects.Dispose();
        }
    }
}