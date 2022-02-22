using Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

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
                (ref Translation translation) =>
                {
                    translation.Value = float3.zero;
                }).Schedule();
        }
    }
}