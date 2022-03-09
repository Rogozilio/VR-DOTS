using Components;
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
    public class CollisionEventInteractive : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;

        protected override void OnCreate()
        {
            base.OnCreate();
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        private RigidTransform GetRigidBody(Entity entity)
        {
            var physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
            var index = physicsWorld.GetRigidBodyIndex(entity);
            return physicsWorld.Bodies[index].WorldFromBody;
        }

        private void CreateJointEntity(Entity entityHand, Entity entityObject)
        {
            if (entityHand == Entity.Null || entityObject == Entity.Null)
                return;

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

        private struct CollisionSystem : ICollisionEventsJob
        {
            public ComponentDataFromEntity<Interactive> interactiveGroup;

            public void Execute(CollisionEvent collision)
            {
                if (!interactiveGroup.HasComponent(collision.EntityA)
                    || !interactiveGroup.HasComponent(collision.EntityB))
                    return;

                Entity entityA = Entity.Null;
                Entity entityB = Entity.Null;

                if (interactiveGroup[collision.EntityA].withHand == JointState.On &&
                    interactiveGroup[collision.EntityB].withHand == JointState.On)
                {
                    entityA = collision.EntityA;
                    entityB = collision.EntityB;
                }
                else if (interactiveGroup[collision.EntityB].withHand == JointState.On &&
                         interactiveGroup[collision.EntityA].withHand == JointState.On)
                {
                    entityA = collision.EntityB;
                    entityB = collision.EntityA;
                }
                else
                {
                    return;
                }

                var interactiveA = interactiveGroup[entityA];
                var interactiveB = interactiveGroup[entityB];
                interactiveA.withInteractive = JointState.InProgress;
                interactiveB.withInteractive = JointState.InProgress;
                interactiveGroup[entityA] = interactiveA;
                interactiveGroup[entityB] = interactiveB;
            }
        }

        protected override void OnUpdate()
        {
            var CollisionSystem = new CollisionSystem
            {
                interactiveGroup = GetComponentDataFromEntity<Interactive>()
            };
            CollisionSystem.Schedule(_stepPhysicsWorld.Simulation,
                ref _buildPhysicsWorld.PhysicsWorld, Dependency).Complete();
        }
    }
}