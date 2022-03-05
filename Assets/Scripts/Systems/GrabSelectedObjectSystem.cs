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
    [UpdateAfter(typeof(ObjectSelectionSystem))]
    public class GrabSelectedObjectSystem : SystemBase
    {
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


        protected override void OnUpdate()
        {
            NativeArray<Entity> entityHands = new NativeArray<Entity>(2, Allocator.TempJob);
            NativeArray<Entity> entityObjects = new NativeArray<Entity>(2, Allocator.TempJob);

            var activeHandJob = Entities.ForEach((Entity entity, ref XRHandInputControllerComponent input) =>
            {
                if(input.isOccupied)
                    return;
                
                if (input.selectValue > 0.9f)
                {
                    input.isOccupied = true;
                    var index = (int) input.handType - 1;
                    entityHands[index] = entity;
                }
            }).Schedule(Dependency);

            var grabSelectedObjectJob = Entities.ForEach((Entity entity, ref Interactive interactive) =>
            {
                if (interactive.nearHand == HandType.None ||
                    interactive.inHand != HandType.None ||
                    !interactive.isClosest)
                    return;

                var index = (int) interactive.nearHand - 1;
                entityObjects[index] = entity;
                
                if (entityHands[index] != Entity.Null && entityObjects[index] != Entity.Null)
                {
                    interactive.isJointed = true;
                    interactive.inHand = interactive.nearHand;
                }
            }).Schedule(activeHandJob);
            grabSelectedObjectJob.Complete();

            for (int i = 0; i < entityHands.Length; i++)
            {
                CreateJointEntity(entityHands[i], entityObjects[i]);
            }

            entityHands.Dispose();
            entityObjects.Dispose();
        }
    }
}