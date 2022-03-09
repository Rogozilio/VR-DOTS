using Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace DOTS.Systems
{
    //[DisableAutoCreation]
    // public class XRHandRaycast : ComponentSystem
    // {
    //     private bool Raycast(Entity start, Entity end, out RaycastHit hit)
    //     {
    //         var buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
    //         var colWorld = buildPhysicsWorld.PhysicsWorld.CollisionWorld;
    //         RaycastInput raycastInput = new RaycastInput()
    //         {
    //             Start = GetComponentDataFromEntity<Translation>()[start].Value,
    //             End = GetComponentDataFromEntity<Translation>()[end].Value,
    //             Filter = new CollisionFilter()
    //             {
    //                 CollidesWith = (uint) CollisionLayer.ToHand,
    //                 BelongsTo = ~0u,
    //             }
    //         };
    //         SetFilterBelongsTo(start, CollisionLayer.IgnoreRaycast);
    //         var result = colWorld.CastRay(raycastInput, out hit);
    //         SetFilterBelongsTo(start, CollisionLayer.ToHand);
    //         return result;
    //     }
    //     private void SetFilterBelongsTo(Entity entity, CollisionLayer layer)
    //     {
    //         var physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
    //         var index = physicsWorld.GetRigidBodyIndex(entity);
    //         var filter = physicsWorld.GetCollisionFilter(index);
    //         filter.BelongsTo = (uint) layer; 
    //         physicsWorld.Bodies[index].Collider.Value.Filter = filter;
    //     }
    //     protected override void OnUpdate()
    //     {
    //         Entities.ForEach((ref HandsObjectComponent hands) =>
    //         {
    //             if (hands.objectInLeftHand != Entity.Null &&
    //                 hands.objectInRightHand != Entity.Null &&
    //                 hands.activeHand != HandType.None)
    //             {
    //                 var position = GetComponentDataFromEntity<Translation>();
    //                 var rotate = GetComponentDataFromEntity<Rotation>();
    //                 var interactive = GetComponentDataFromEntity<Interactive>();
    //                 Entity entityStart;
    //                 Entity entityEnd;
    //                 if (hands.activeHand == HandType.Left)
    //                 {
    //                     entityStart = hands.objectInLeftHand;
    //                     entityEnd = hands.objectInRightHand;
    //                 }
    //                 else
    //                 {
    //                     entityStart = hands.objectInRightHand;
    //                     entityEnd = hands.objectInLeftHand;
    //                 }
    //                 var buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
    //                 RaycastHit hit = new RaycastHit();
    //                 RaycastHit hit2 = new RaycastHit();
    //                 float3 posSurfaceStartEntity = float3.zero;
    //                 if (Raycast(entityStart, entityEnd, out hit))
    //                 {
    //                     float3 newPosGhost = float3.zero;
    //                     var hitEntity = buildPhysicsWorld.PhysicsWorld
    //                         .Bodies[hit.RigidBodyIndex].Entity;
    //                     hands.raycastEntity = hitEntity;
    //                     if (Raycast(hitEntity, entityStart, out hit2))
    //                     {
    //                         var hitEntity2 = buildPhysicsWorld.PhysicsWorld
    //                             .Bodies[hit2.RigidBodyIndex].Entity;
    //                         posSurfaceStartEntity =
    //                             hit2.Position - position[hitEntity2].Value;
    //                     }
    //                     newPosGhost = hit.Position;
    //                     var interactiveEntity = interactive[entityStart].ghost;
    //                     var translate = position[interactiveEntity];
    //                     var rotation = rotate[interactiveEntity];
    //                     translate.Value = newPosGhost - posSurfaceStartEntity;
    //                     rotation.Value = rotate[entityStart].Value;
    //                     position[interactiveEntity] = translate;
    //                     rotate[interactiveEntity] = rotation;
    //                 }
    //             }
    //         });
    //     }
    // }
}