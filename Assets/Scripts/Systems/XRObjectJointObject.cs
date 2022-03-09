using Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    // [DisableAutoCreation]
    // public class XRObjectJointObject : ComponentSystem
    // {
    //     
    //     private RigidTransform GetRigidBody(Entity entity)
    //     {
    //         var physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
    //         var index = physicsWorld.GetRigidBodyIndex(entity);
    //         return physicsWorld.Bodies[index].WorldFromBody;
    //     }
    //     protected override void OnUpdate()
    //     {
    //         Entities.ForEach((ref HandsObjectComponent hands) =>
    //         {
    //             if (hands.isReadyJoint)
    //             {
    //                 Entity entity = Entity.Null;
    //                 var translate = GetComponentDataFromEntity<Translation>();
    //                 var rotation = GetComponentDataFromEntity<Rotation>();
    //                 var interactive = GetComponentDataFromEntity<Interactive>();
    //                 var bodyFrameA = new BodyFrame(new RigidTransform());
    //                 var bodyFrameB = new BodyFrame(new RigidTransform());
    //                 var entityA = interactive[hands.objectInLeftHand];
    //                 var entityB = interactive[hands.objectInRightHand];
    //                 entityA.isJointedWithHand = true; entityB.isJointedWithHand = true;
    //                 interactive[hands.objectInLeftHand] = entityA;
    //                 interactive[hands.objectInRightHand] = entityB;
    //                 if (hands.activeHand == HandType.Left)
    //                     entity = hands.objectInLeftHand;
    //                 else if (hands.activeHand == HandType.Right)
    //                     entity = hands.objectInRightHand;
    //                 RigidTransform ghost = new RigidTransform() {
    //                     pos = translate[interactive[entity].ghost].Value,
    //                     rot = rotation[interactive[entity].ghost].Value};
    //                 if (hands.activeHand == HandType.Left)
    //                 {
    //                     RigidTransform bFromA = math.mul(
    //                         math.inverse(GetRigidBody(hands.raycastEntity)), ghost);
    //                     bodyFrameB = new BodyFrame(new RigidTransform() {
    //                         pos = bFromA.pos,
    //                         rot = bFromA.rot});
    //                     var _entityHands = EntityManager.CreateEntity();
    //                     EntityManager.AddComponentData(_entityHands,
    //                         new PhysicsConstrainedBodyPair(hands.objectInLeftHand,
    //                             hands.raycastEntity, false));
    //                     EntityManager.AddComponentData(_entityHands,
    //                         PhysicsJoint.CreateFixed(bodyFrameA, bodyFrameB));
    //                 }
    //                 else if (hands.activeHand == HandType.Right)
    //                 {
    //                     RigidTransform bFromA = math.mul(
    //                         math.inverse(ghost), GetRigidBody(hands.raycastEntity));
    //                     bodyFrameB = new BodyFrame(new RigidTransform() {
    //                         pos = bFromA.pos,
    //                         rot = bFromA.rot});
    //                     var _entityHands = EntityManager.CreateEntity();
    //                     EntityManager.AddComponentData(_entityHands,
    //                         new PhysicsConstrainedBodyPair(hands.raycastEntity,
    //                             hands.objectInRightHand, false));
    //                     EntityManager.AddComponentData(_entityHands,
    //                         PhysicsJoint.CreateFixed(bodyFrameA, bodyFrameB));
    //                 }
    //                 if (hands.activeHand == HandType.Left)
    //                     hands.objectInLeftHand = Entity.Null;
    //                 else if (hands.activeHand == HandType.Right)
    //                     hands.objectInRightHand = Entity.Null;
    //                 hands.activeHand = HandType.None;
    //                 hands.isReadyJoint = false;
    //             }
    //         });
    //     }
    // }
}