using Components;
using DOTS.Components;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DisableJointObjects))]
    public class CreateJointObjects : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private BuildPhysicsWorld _buildPhysicsWorld;

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        }

        protected override void OnUpdate()
        {
            var interactiveGroup = GetComponentDataFromEntity<Interactive>();
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var physicsWorld = _buildPhysicsWorld.PhysicsWorld;
            NativeArray<Entity> entityInteractive = new NativeArray<Entity>(2, Allocator.TempJob);
            //Hand -> Interactive
            var createHandJointInteractive = Entities.WithReadOnly(physicsWorld).ForEach((Entity entity, ref InputControllerComponent input) =>
            {
                if(input.inHand == Entity.Null || input.isJoint)
                    return;

                input.isJoint = true;
                var interactive = interactiveGroup[input.inHand];
                interactive.withHand = JointState.On;
                interactiveGroup[input.inHand] = interactive;
                
                var rigidTransformHand = physicsWorld.Bodies[physicsWorld.GetRigidBodyIndex(entity)].WorldFromBody;
                var rigidTransformObject = physicsWorld.Bodies[physicsWorld.GetRigidBodyIndex(input.inHand)].WorldFromBody;
                var bodyFrameA = new BodyFrame(new RigidTransform());

                RigidTransform bFromA =
                    math.mul(math.inverse(rigidTransformObject),
                        rigidTransformHand);
                var bodyFrameB = new BodyFrame(new RigidTransform()
                {
                    pos = bFromA.pos,
                    rot = bFromA.rot
                });
                var _entityHands = cbs.CreateEntity();
                cbs.AddComponent(_entityHands,
                    new PhysicsConstrainedBodyPair(entity,
                        input.inHand, true));
                cbs.AddComponent(_entityHands,
                    PhysicsJoint.CreateFixed(bodyFrameA, bodyFrameB));
            }).Schedule(Dependency);

            //Interactive -> Interactive
            var getInteractiveForJoint = Entities.ForEach((Entity entity,
                ref Interactive interactive) =>
            {
                if(interactive.withInteractive != JointState.InProgress)
                    return;
                
                if (interactive.withHand == JointState.On)
                    entityInteractive[0] = entity;
                else if (interactive.withHand == JointState.Off)
                    entityInteractive[1] = entity;

                interactive.withInteractive = JointState.Off;
            }).Schedule(createHandJointInteractive);

            Job.WithReadOnly(physicsWorld).WithCode(() =>
            {
                if(entityInteractive[0] == Entity.Null ||
                   entityInteractive[1] == Entity.Null)
                    return;
                
                var rigidTransformHand = physicsWorld.Bodies[physicsWorld.GetRigidBodyIndex(entityInteractive[0])].WorldFromBody;
                var rigidTransformObject = physicsWorld.Bodies[physicsWorld.GetRigidBodyIndex(entityInteractive[1])].WorldFromBody;
                var bodyFrameA = new BodyFrame(new RigidTransform());
            
                RigidTransform bFromA =
                    math.mul(math.inverse(rigidTransformObject),
                        rigidTransformHand);
                var bodyFrameB = new BodyFrame(new RigidTransform()
                {
                    pos = bFromA.pos,
                    rot = bFromA.rot
                });
                var _entityHands = cbs.CreateEntity();
                cbs.AddComponent(_entityHands,
                    new PhysicsConstrainedBodyPair(entityInteractive[0],
                        entityInteractive[1], true));
                cbs.AddComponent(_entityHands,
                    PhysicsJoint.CreateFixed(bodyFrameA, bodyFrameB));
            }).Schedule(getInteractiveForJoint).Complete();
            entityInteractive.Dispose();
        }
    }
}