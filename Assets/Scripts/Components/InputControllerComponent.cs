using DOTS.Enum;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DOTS.Components
{
    [GenerateAuthoringComponent]
    public struct InputControllerComponent : IComponentData
    {
        public HandType handType;
        public Entity inHand;
        public bool isJoint;
        [HideInInspector] public float3 position;
        [HideInInspector] public quaternion rotation;
        [HideInInspector] public int trackingState;
        [HideInInspector]public float select;
        [HideInInspector]public float selectValue;
        [HideInInspector]public float activate;
        [HideInInspector]public float activateValue;
        [HideInInspector]public float uiPress;
        [HideInInspector]public float uiPressValue;
        [HideInInspector]public float3 hapticDevice;
        [HideInInspector]public float teleportSelect;
        [HideInInspector]public float2 teleportModeActivate;
        [HideInInspector]public float teleportModeCancel;
        [HideInInspector]public float2 turn;
        [HideInInspector]public float2 move;
        [HideInInspector]public float2 rotateAnchor;
        [HideInInspector]public float2 translateAnchor;

        public bool IsGripPressed => selectValue > 0.9f;
        public bool IsTriggerPressed => activateValue > 0.9f;
    }
}