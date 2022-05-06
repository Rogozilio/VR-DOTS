using DOTS.Components;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ObjectSelectionSystem))]
    public partial class GripSelectedObjectSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            NativeArray<Entity> entityHands = new NativeArray<Entity>(2, Allocator.TempJob);
            NativeArray<Entity> entityObjects = new NativeArray<Entity>(2, Allocator.TempJob);

            var grabSelectedObjectJob = Entities.ForEach(
                (Entity entity, int entityInQueryIndex, in InteractiveComponent interactive) =>
                {
                    if (interactive.nearHand == HandType.None ||
                        !interactive.isClosest)
                        return;

                    var index = (int) interactive.nearHand - 1;
                    entityObjects[index] = entity;
                }).Schedule(Dependency);
            
            Entities.ForEach(
                (Entity entity, ref InputControllerComponent input) =>
                {
                    if (input.inHand != Entity.Null || !input.IsGripPressed)
                        return;
                    
                    var index = (int) input.handType - 1;
                    input.inHand = entityObjects[index];
                }).Schedule(grabSelectedObjectJob).Complete();
            
            entityHands.Dispose();
            entityObjects.Dispose();
        }
    }
}