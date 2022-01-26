using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public class XRHandJointObject : ComponentSystem
    {
        private RigidTransform GetRigidBody(Entity entity)
        {
            var physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
            var index = physicsWorld.GetRigidBodyIndex(entity);
            return physicsWorld.Bodies[index].WorldFromBody;
        }
        [BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (Entity entityObject, ref Interactive interactive, ref PhysicsVelocity velocity) =>
                {
                    if (interactive.Hand != Entity.Null && interactive.inHand == HandType.None)
                    {
                        interactive.isJointed = true;
                        interactive.inHand =
                            GetComponentDataFromEntity<XRHandInputControllerComponent>()
                            [interactive.Hand].handType;

                        var entityHand = interactive.Hand;
                        var bodyFrameA = new BodyFrame(new RigidTransform());
                        RigidTransform bFromA =
                            math.mul(math.inverse(GetRigidBody(entityObject)),
                                GetRigidBody(entityHand));
                        var bodyFrameB = new BodyFrame(new RigidTransform()
                        {
                            pos = bFromA.pos,
                            rot = bFromA.rot
                        });

                        var _entityHands = EntityManager.CreateEntity();
                        EntityManager.AddComponentData(_entityHands,
                            new PhysicsConstrainedBodyPair(entityHand,
                                entityObject, true));
                        EntityManager.AddComponentData(_entityHands,
                            PhysicsJoint.CreateFixed(bodyFrameA, bodyFrameB));
                    }
                });
        }
    }
}