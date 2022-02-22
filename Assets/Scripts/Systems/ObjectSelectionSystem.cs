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
    public class ObjectSelectionSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var minDistanceHandToObject = new NativeArray<float>(2, Allocator.TempJob);
            var transformObject = new NativeArray<Translation>(2, Allocator.TempJob);
            var entityGhost = new NativeArray<Entity>(2, Allocator.TempJob);

            var getMinDistanceJob = Entities.ForEach(
                (ref Interactive interactive, in Translation translation) =>
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
                }).Schedule(Dependency);

            var selectedObject = Entities.WithAll<TagGhost>().ForEach(
                    (Entity entity, ref Translation translation, ref NonUniformScale scale) =>
                    {
                        if (entity.Index == entityGhost[0].Index &&
                            entity.Version == entityGhost[0].Version)
                        {
                            translation = transformObject[0];
                        }

                        if (entity.Index == entityGhost[1].Index &&
                            entity.Version == entityGhost[1].Version)
                        {
                            translation = transformObject[1];
                        }
                    })
                .Schedule(getMinDistanceJob);
            selectedObject.Complete();

            minDistanceHandToObject.Dispose();
            transformObject.Dispose();
            entityGhost.Dispose();
        }
    }
}