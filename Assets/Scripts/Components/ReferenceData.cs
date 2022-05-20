using Unity.Entities;

namespace DOTS.Components
{
    public struct ReferenceData : IComponentData
    {
        public BlobAssetReference<BlobData> data;
    }
}