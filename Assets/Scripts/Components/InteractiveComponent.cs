using System;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace DOTS.Components
{
    [GenerateAuthoringComponent]
    public struct InteractiveComponent : IComponentData
    {
        public int id;
        public Entity ghost;
        public HandType inHand;
        public Entity CollisionWith;
        public CollisionState CollisionState;
        public bool isClosest;
        public HandType nearHand;
        public float distance;
    }
}