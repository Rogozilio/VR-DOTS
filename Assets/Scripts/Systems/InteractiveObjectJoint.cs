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
    public class InteractiveObjectJoint : SystemBase
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        
        protected override void OnCreate()
        {
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
            public NativeArray<Entity> entities;

            public void Execute(CollisionEvent collision)
            {
                NativeArray<Interactive> interactives = new NativeArray<Interactive>(2, Allocator.Temp);

                if (!interactiveGroup.HasComponent(collision.EntityA)
                    || !interactiveGroup.HasComponent(collision.EntityB))
                    return;

                if (interactiveGroup[collision.EntityA].inHand == HandType.Left &&
                    interactiveGroup[collision.EntityB].inHand == HandType.Right)
                {
                    entities[0] = collision.EntityA;
                    entities[1] = collision.EntityB;
                }
                else if (interactiveGroup[collision.EntityB].inHand == HandType.Left &&
                         interactiveGroup[collision.EntityA].inHand == HandType.Right)
                {
                    entities[0] = collision.EntityB;
                    entities[1] = collision.EntityA;
                }

                for (int i = 0; i < interactives.Length; i++)
                {
                    var interactive = interactiveGroup[entities[i]];
                    interactive.isJointedWithObject = true;
                    interactiveGroup[entities[i]] = interactive;
                }
                
                interactives.Dispose();
            }
        }

        protected override void OnUpdate()
        {
            NativeArray<bool> isReadyJoint = new NativeArray<bool>(1, Allocator.TempJob);
            NativeArray<Entity> entityInteractive = new NativeArray<Entity>(2, Allocator.TempJob);
            
            var CollisionSystem = new CollisionSystem
            {
                interactiveGroup = GetComponentDataFromEntity<Interactive>(),
                entities = entityInteractive
            };
            var collisionJob = CollisionSystem.Schedule(_stepPhysicsWorld.Simulation,
                ref _buildPhysicsWorld.PhysicsWorld, Dependency);

            var jointWithObjectJob = Entities.ForEach((in XRHandInputControllerComponent input) =>
            {
                if(input.isOccupied)
                    return;
                
                isReadyJoint[0] = true;
            }).Schedule(collisionJob);
            jointWithObjectJob.Complete();
            
            if(isReadyJoint[0])
                CreateJointEntity(entityInteractive[0], entityInteractive[1]);

            isReadyJoint.Dispose();
            entityInteractive.Dispose();
        }
    }
}