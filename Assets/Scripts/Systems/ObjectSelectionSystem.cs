using System.ComponentModel;
using System.Linq;
using Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(XRHandTriggerEvent))]
    public class ObjectSelectionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var minDistanceHandToObject = new NativeArray<float>(2, Allocator.TempJob);
            var transformObject = new NativeArray<Translation>(2, Allocator.TempJob);
            var entityGhost = new NativeArray<Entity>(2, Allocator.TempJob);

            var GetMinDistanceJob = Entities.ForEach(
                    (ref Interactive interactive, ref Translation translation) =>
                    {
                        if (interactive.inHand == HandType.None)
                            return;

                        if (interactive.inHand == HandType.Left)
                        {
                            if (minDistanceHandToObject[0] == 0f ||
                                minDistanceHandToObject[0] > interactive.distance)
                            {
                                minDistanceHandToObject[0] = interactive.distance;
                                transformObject[0] = translation;
                                entityGhost[0] = interactive.ghost;
                                
                            }
                        }
                        else
                        {
                            if (minDistanceHandToObject[1] == 0f ||
                                minDistanceHandToObject[1] > interactive.distance)
                            {
                                minDistanceHandToObject[1] = interactive.distance;
                                transformObject[1] = translation;
                                entityGhost[1] = interactive.ghost;
                            }
                        }
                    }).WithoutBurst().WithName("GetMinDistanceJob")
                .Schedule(Dependency);

            var selectedObject = Entities.WithAll<TagGhost>().ForEach(
                    (Entity entity, ref Translation translation, ref NonUniformScale scale) =>
                    {
                        for (int i = 0; i < entityGhost.Length; i++)
                        {
                            if (entity.Index == entityGhost[i].Index &&
                                entity.Version == entityGhost[i].Version)
                            {
                                translation = transformObject[i];
                                scale.Value += 0.001f;
                            }
                        }
                    })
                .WithoutBurst().WithName("selectedObject").Schedule(GetMinDistanceJob);
            selectedObject.Complete();

            minDistanceHandToObject.Dispose();
            transformObject.Dispose();
            entityGhost.Dispose();
        }
    }
}