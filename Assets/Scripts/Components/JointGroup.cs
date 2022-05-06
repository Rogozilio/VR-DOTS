using Unity.Entities;

namespace DOTS.Components
{
    public struct JointGroup : IComponentData//ISharedComponentData
    {
        public int index;
        public bool isOriginIndex;
    }
}