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
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ObjectSelectionSystem))]
    public class GripSelectedObjectSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            _endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> entityHands = new NativeArray<Entity>(2, Allocator.TempJob);
            NativeArray<Entity> entityObjects = new NativeArray<Entity>(2, Allocator.TempJob);
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer()
                .AsParallelWriter();

            var grabSelectedObjectJob = Entities.ForEach(
                (Entity entity, int entityInQueryIndex, ref Interactive interactive) =>
                {
                    if (interactive.nearHand == HandType.None ||
                        !interactive.isClosest)
                        return;

                    var index = (int) interactive.nearHand - 1;
                    entityObjects[index] = entity;
                }).Schedule(Dependency);
            
            var activeHandJob = Entities.ForEach(
                (Entity entity, ref InputControllerComponent input) =>
                {
                    if (input.inHand != Entity.Null || !input.IsGripPressed)
                        return;
                    
                    var index = (int) input.handType - 1;
                    input.inHand = entityObjects[index];
                }).Schedule(grabSelectedObjectJob);
            activeHandJob.Complete();

            entityHands.Dispose();
            entityObjects.Dispose();
        }
    }
}