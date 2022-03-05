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
            var rotateObject = new NativeArray<Rotation>(2, Allocator.TempJob);
            var entityObject = new NativeArray<Entity>(2, Allocator.TempJob);
            var entityGhost = new NativeArray<Entity>(2, Allocator.TempJob);

            var GetMinDistanceJob = Entities.ForEach(
                    (Entity entity, ref Interactive interactive, in Translation translation, in Rotation rotate) =>
                    {
                        if (interactive.nearHand == HandType.None
                            || interactive.inHand != HandType.None)
                            return;

                        var index = (int) interactive.nearHand - 1;

                        if (minDistanceHandToObject[index] == 0f ||
                            minDistanceHandToObject[index] > interactive.distance)
                        {
                            minDistanceHandToObject[index] = interactive.distance;
                            transformObject[index] = translation;
                            rotateObject[index] = rotate;
                            entityGhost[index] = interactive.ghost;
                            entityObject[index] = entity;
                        }
                    }).WithName("GetMinDistanceJob")
                .Schedule(Dependency);

            var setIsClosestForInteractive = Job.WithCode(() =>
            {
                var interactive = GetComponentDataFromEntity<Interactive>();
                foreach (var entity in entityObject)
                {
                    if (entity != Entity.Null)
                    {
                        var entityInteractive = interactive[entity];
                        entityInteractive.isClosest = true;
                        interactive[entity] = entityInteractive;
                    }
                }
            }).WithName("setIsClosestForInteractive").Schedule(GetMinDistanceJob);

            var selectedObject = Entities.WithAll<TagGhost>().ForEach(
                    (Entity entity, ref Translation translation, ref Rotation rotate, ref NonUniformScale scale) =>
                    {
                        for (int i = 0; i < entityGhost.Length; i++)
                        {
                            if (entity.Index == entityGhost[i].Index &&
                                entity.Version == entityGhost[i].Version)
                            {
                                translation = transformObject[i];
                                rotate = rotateObject[i];
                                //scale.Value += 0.001f;
                            }
                        }
                    })
                .WithName("selectedObject").Schedule(setIsClosestForInteractive);
            selectedObject.Complete();

            minDistanceHandToObject.Dispose();
            transformObject.Dispose();
            rotateObject.Dispose();
            entityGhost.Dispose();
            entityObject.Dispose();
        }
    }
}