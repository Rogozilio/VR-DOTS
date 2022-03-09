using Components;
using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public class DisableGravityJointObjects : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach(( ref PhysicsVelocity velocity, ref Interactive interactive) =>
            {
                if (interactive.withHand == JointState.On)
                {
                    velocity.Linear += new float3(0, 0.1635f, 0);
                }
            });
        }
    }
}