using Components;
using DefaultNamespace;
using DOTS.Components;
using DOTS.Tags;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace DOTS.Systems
{
    public class EndGameSystem : JobComponentSystem
    {
        private BuildPhysicsWorld _buildPhysicsWorld;
        private StepPhysicsWorld _stepPhysicsWorld;
        protected override void OnCreate()
        {
            _buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
            _stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        private struct ApplicationJob : ITriggerEventsJob
        {
            public ComponentDataFromEntity<Interactive> interactiveGroup;
            public ComponentDataFromEntity<TagFinish> finishGroup;
            public void Execute(TriggerEvent triggerEvent)
            {
                if (finishGroup.HasComponent(triggerEvent.EntityA)
                    && interactiveGroup.HasComponent(triggerEvent.EntityB))
                {
                    LoadScene.IsEnd = true;
                }
                else if (finishGroup.HasComponent(triggerEvent.EntityB)
                         && interactiveGroup.HasComponent(triggerEvent.EntityA))
                {
                    LoadScene.IsEnd = true;
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var applicationJob = new ApplicationJob
            {
                interactiveGroup = GetComponentDataFromEntity<Interactive>(),
                finishGroup = GetComponentDataFromEntity<TagFinish>()
            };
            return applicationJob.Schedule(_stepPhysicsWorld.Simulation,
                ref _buildPhysicsWorld.PhysicsWorld, inputDeps);
        }
    }
}