using DOTS.Enum;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Components
{
    [GenerateAuthoringComponent]
    public struct InteractiveComponent : IComponentData
    {
        public Entity ghost;
        public HandType inHand;
        public Entity CollisionWith;
        public CollisionState CollisionState;
        public bool isClosest;
        public HandType nearHand;
        public float distance;
    }
}