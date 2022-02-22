using DOTS.Components;
using DOTS.Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
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
    public class XRInputActionSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle inputLeftHandJob = Entities.ForEach(
                (ref Translation transform, ref Rotation rotation,
                    ref XRHandInputControllerComponent input, ref PhysicsVelocity velocity) =>
                {
                    float3 antiGravity = new float3(0, 0.1635f, 0);
                    velocity.Linear = (input.position - transform.Value) * 60 + antiGravity;
                    velocity.Angular = math.mul(math.inverse(rotation.Value), input.rotation).value.xyz * 60;
                }).Schedule(inputDeps);
            inputLeftHandJob.Complete();
            
            return inputDeps;
        }
    } 
}