using DOTS.Components;
using DOTS.Enum;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Random = UnityEngine.Random;

namespace DOTS.Systems.DebugTest
{
    [DisableAutoCreation]
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class DebugJointGroup : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;
        private float4[] colors = new float4[256];

        protected override void OnStartRunning()
        {
            _endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            Entities.ForEach(
                (Entity entity, in InteractiveComponent interactive) =>
                {
                    cbs.AddComponent(entity,
                        new URPMaterialPropertyBaseColor());
                }).Schedule();

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new float4((float)Random.Range(0, 256) / 256,
                    (float)Random.Range(0, 256) / 256, (float)Random.Range(0, 256) / 256, 1);
            }
        }

        protected override void OnUpdate()
        {
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

            Entities.ForEach((Entity entity, in JointGroup jointGroup,
                in InteractiveComponent interactive) =>
            {
                var i = (jointGroup.index == 0) ? 1 : jointGroup.index;
                cbs.SetComponent(entity,
                    new URPMaterialPropertyBaseColor { Value = colors[i] });
            }).WithoutBurst().Run();
        }
    }
}