using System;
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
    //[DisableAutoCreation]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ResetDataSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem
            _endInitializationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            _endInitializationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
        }
        protected override void OnStartRunning()
        {
            var cbs = _endInitializationEntityCommandBufferSystem.CreateCommandBuffer();
            
            Entities.WithAll<InteractiveComponent>().WithNone<JointGroup>().ForEach(
                (Entity entity) =>
                {
                    cbs.AddComponent(entity, new JointGroup {index = 0});
                }).WithoutBurst().Run();
        }

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
                (ref InteractiveComponent interactive) =>
                {
                    interactive.isClosest = false;
                    interactive.nearHand = HandType.None;
                    interactive.distance = 0f;
                }).Schedule();
            //Сброс значений призрачных проекций
            Entities.WithAll<TagGhost>().ForEach(
                (ref Translation translation) => { translation.Value = float3.zero; }).Schedule();
        }
    }
}