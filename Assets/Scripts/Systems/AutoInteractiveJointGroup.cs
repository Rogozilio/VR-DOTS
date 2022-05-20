using DOTS.Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine.PlayerLoop;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(ResetDataSystem))]
    public partial class AutoInteractiveJointGroup : SystemBase
    {
        protected override void OnUpdate()
        {
            var inputController = GetComponentDataFromEntity<InputControllerComponent>();
            var jointGroup = GetComponentDataFromEntity<JointGroup>();
            
            Entities.ForEach((Entity entity, ref JointGroup joint) =>
            {
                if (joint.isOriginIndex)
                {
                    joint.index = entity.Index;
                    joint.isOriginIndex = false;
                }
                    

            }).Schedule();

            Entities.ForEach((ref PhysicsConstrainedBodyPair bodyPair) =>
            {
                //JointIndex for interactive with interactive
                if (jointGroup.HasComponent(bodyPair.EntityA) &&
                    jointGroup.HasComponent(bodyPair.EntityB))
                {
                    var jointA = jointGroup[bodyPair.EntityA];
                    var jointB = jointGroup[bodyPair.EntityB];

                    if (jointA.index == 0 && jointB.index == 0)
                        return;

                    if (jointA.index > jointB.index)
                    {
                        jointB.index = jointA.index;
                    }
                    else if (jointB.index > jointA.index)
                    {
                        jointA.index = jointB.index;
                    }

                    jointGroup[bodyPair.EntityA] = jointA;
                    jointGroup[bodyPair.EntityB] = jointB;
                }
            }).Schedule();
        }
    }
}