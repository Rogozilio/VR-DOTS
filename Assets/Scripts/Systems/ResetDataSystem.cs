using System;
using Components;
using DOTS.Components;
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
            Entities.ForEach((ref InputControllerComponent input) =>
            {
                if (!input.IsGripPressed)
                {
                    input.inHand = Entity.Null;
                    input.isJoint = false;
                }
                    
            }).Schedule();

        //Сброс значений интерактивных объектов
            Entities.ForEach(
                (ref Interactive interactive) =>
                {
                    interactive.isClosest = false;
                    interactive.nearHand = HandType.None;
                    interactive.distance = 0f;
                }).Schedule();
            //Сброс значений призрачных проекций
            Entities.WithAll<TagGhost>().ForEach(
                (ref NonUniformScale scale, ref Translation translation, in LocalToWorld local) =>
                {
                    translation.Value = float3.zero;
                }).Schedule();
        }
    }
}