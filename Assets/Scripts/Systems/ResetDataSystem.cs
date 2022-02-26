using System;
using Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class ResetDataSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //Сброс значений интерактивных объектов
            Entities.ForEach(
                (ref Interactive interactive) =>
                {
                    interactive.inHand = HandType.None;
                    interactive.distance = 0f;
                }).Schedule();
            //Сброс значений призрачных проекций
            Entities.WithAll<TagGhost>().ForEach(
                (ref NonUniformScale scale, ref Translation translation, in LocalToWorld local) =>
                {
                    translation.Value = float3.zero;
                    float3 localScale = new float3(local.Value.c0.x, local.Value.c1.y, local.Value.c2.z);
                    scale.Value = math.floor(localScale * 10f) * 0.1f;
                }).Schedule();
        }
    }
}