using DOTS.Enum;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Tags
{
    [GenerateAuthoringComponent]
    public struct Interactive : IComponentData
    {
        public Entity ghost;
        [HideInInspector] public bool isJointed;
        [HideInInspector] public HandType inHand;
        [HideInInspector] public Entity Hand;
    }
}