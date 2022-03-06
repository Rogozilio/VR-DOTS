﻿using DOTS.Enum;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct Interactive : IComponentData
    {
        public Entity ghost;
        [HideInInspector] public bool isJointedWithHand;
        [HideInInspector] public bool isJointedWithObject;
        [HideInInspector] public bool isClosest;
        [HideInInspector] public HandType nearHand;
        [HideInInspector] public HandType inHand;
        [HideInInspector] public Entity Hand;
        [HideInInspector] public float distance;
    }
}