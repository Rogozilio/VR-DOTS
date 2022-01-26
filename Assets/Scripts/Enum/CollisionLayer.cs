using System;

namespace DOTS.Enum
{
    [Flags]
    public enum CollisionLayer
    {
        IgnoreRaycast = 1 << 0,
        ToHand = 1 << 1,
    }
}