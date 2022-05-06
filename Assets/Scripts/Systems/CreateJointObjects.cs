using DOTS.Components;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Systems;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DisableJointObjects))]
    public partial class CreateJointObjects : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private BuildPhysicsWorld _buildPhysicsWorld;

        struct Joint
        {
            private EntityCommandBuffer _cbs;

            [ReadOnly]
            private PhysicsWorld _physicsWorld;

            public Joint(ref EntityCommandBuffer cbs, ref PhysicsWorld physicsWorld)
                => (_cbs, _physicsWorld) = (cbs, physicsWorld);

            public void Create(Entity entityA, Entity entityB)
            {
                var rigidTransformHand = _physicsWorld
                    .Bodies[_physicsWorld.GetRigidBodyIndex(entityA)].WorldFromBody;
                var rigidTransformObject = _physicsWorld
                    .Bodies[_physicsWorld.GetRigidBodyIndex(entityB)].WorldFromBody;
                var bodyFrameA = new BodyFrame(new RigidTransform());
                RigidTransform bFromA = math.mul(math.inverse(rigidTransformObject),
                    rigidTransformHand);
                var bodyFrameB = new BodyFrame(new RigidTransform()
                    { pos = bFromA.pos, rot = bFromA.rot });
                var entityHands = _cbs.CreateEntity();
                _cbs.AddComponent(entityHands, new PhysicsConstrainedBodyPair(entityA,
                    entityB, true));
                _cbs.AddComponent(entityHands,
                    PhysicsJoint.CreateFixed(bodyFrameA, bodyFrameB));
                _cbs.AddSharedComponent(entityHands, new PhysicsWorldIndex());
            }
        }

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            World.GetOrCreateSystem<EndJointConversionSystem>();
        }

        protected override void OnUpdate()
        {
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var physicsWorld = _buildPhysicsWorld.PhysicsWorld;
            var jointGroup = GetComponentDataFromEntity<JointGroup>();
            var joint = new Joint(ref cbs, ref physicsWorld);

            //Hand -> Interactive
            var createHandJointInteractive = Entities.ForEach(
                (Entity entity, ref InputControllerComponent input) =>
                {
                    if (input.inHand == Entity.Null || input.isJoint)
                        return;

                    input.isJoint = true;

                    var jointComponent = jointGroup[input.inHand];
                    jointComponent.isOriginIndex = true;
                    jointGroup[input.inHand] = jointComponent;

                    joint.Create(entity, input.inHand);
                }).Schedule(Dependency);

            //Interactive -> Interactive
            Entities.ForEach((Entity entity, ref InteractiveComponent interactive) =>
            {
                if (interactive.CollisionWith == Entity.Null)
                    return;

                if (interactive.inHand == HandType.None &&
                    interactive.CollisionState == CollisionState.ReadyToJoint)
                {
                    joint.Create(interactive.CollisionWith, entity);
                }

                if (interactive.CollisionState == CollisionState.Yes)
                {
                    interactive.CollisionState = CollisionState.ReadyToJoint;
                }
                else if (interactive.CollisionState == CollisionState.ReadyToJoint)
                {
                    interactive.CollisionWith = Entity.Null;
                    interactive.CollisionState = CollisionState.None;
                }
            }).Schedule(createHandJointInteractive).Complete();
        }
    }
}