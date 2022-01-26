using DOTS.Enum;
using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct HandsObjectComponent : IComponentData
    {
        public Entity objectInLeftHand;
        public Entity objectInRightHand;
        public Entity raycastEntity;
        public HandType activeHand;
        public bool isReadyJoint;
    }
}