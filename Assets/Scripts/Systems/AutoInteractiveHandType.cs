using DOTS.Components;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(AutoInteractiveJointGroup))]
    [UpdateBefore(typeof(ResetDataSystem))]
    public partial class AutoInteractiveHandType : SystemBase
    {
        protected override void OnUpdate()
        {
            var jointGroup = GetComponentDataFromEntity<JointGroup>();
            NativeArray<int> hand = new NativeArray<int>(2, Allocator.TempJob);

            var createHandJointInteractive = Entities.ForEach(
                (Entity entity, in InputControllerComponent input) =>
                {
                    if (input.inHand == Entity.Null)
                        return;

                    var index = (int)input.handType - 1;
                    hand[index] = jointGroup[input.inHand].index;
                }).Schedule(Dependency);

            Entities.ForEach((Entity entity,
                ref InteractiveComponent interactive, in JointGroup jointGroup) =>
            {
                interactive.inHand = HandType.None;
               
                if (jointGroup.index == hand[0] && hand[0] != 0)
                {
                    interactive.inHand = HandType.Left;
                }
                else if (jointGroup.index == hand[1] && hand[1] != 0)
                {
                    interactive.inHand = HandType.Right;
                }
                else if (hand[0] == hand[1] && hand[0] != 0 && hand[1] != 0 &&
                         (jointGroup.index == hand[0] || jointGroup.index == hand[1]))
                {
                    interactive.inHand = HandType.Both;
                }
            }).Schedule(createHandJointInteractive).Complete();

            hand.Dispose();
        }
    }
}