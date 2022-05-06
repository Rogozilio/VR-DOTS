using DOTS.Components;
using DOTS.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.PlayerLoop;
using UnityEngine.XR;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(TriggerEventHandWithInteractive))]
    public partial class MoveHandSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var deltaTime = Time.DeltaTime;
            
            Entities.ForEach(
                (ref PhysicsVelocity velocity, ref PhysicsMass mass, in Translation transform,
                    in Rotation rotation, in InputControllerComponent input) =>
                {
                    velocity.Linear = (input.position - transform.Value) / deltaTime;
                    velocity.Angular =
                        math.mul(math.inverse(rotation.Value), input.rotation).value.xyz / deltaTime;
                    mass.InverseInertia = new float3(1, 1, 1);
                }).Schedule(Dependency).Complete();
        }
    }
}