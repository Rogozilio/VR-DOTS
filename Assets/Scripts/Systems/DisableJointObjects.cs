using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Entities;
using Unity.Physics;

namespace DOTS.Systems
{
    public class DisableJointObjects : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var handInput = GetComponentDataFromEntity<XRHandInputControllerComponent>();
            var interactive = GetComponentDataFromEntity<Interactive>();
            
            Entities.ForEach((Entity entity, ref PhysicsConstrainedBodyPair bodyPair) =>
            {
                //Hand -> object
                if (handInput.HasComponent(bodyPair.EntityA))
                {
                    if (interactive[bodyPair.EntityB].inHand == HandType.None)
                    {
                        var entityB = interactive[bodyPair.EntityB];
                        entityB.Hand = Entity.Null;
                        entityB.isJointed = false;
                        interactive[bodyPair.EntityB] = entityB;
                        EntityManager.DestroyEntity(entity);
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
                        EntityManager.DestroyEntity(entity);
                    }
                }
            });
        }
    }
}