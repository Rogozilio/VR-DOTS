using DOTS.Enum;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct Interactive : IComponentData
    {
        public Entity ghost;
        [HideInInspector] public JointState withHand;
        [HideInInspector] public JointState withInteractive;
        [HideInInspector] public bool isClosest;
        [HideInInspector] public HandType nearHand;
        [HideInInspector] public float distance;
    }
}