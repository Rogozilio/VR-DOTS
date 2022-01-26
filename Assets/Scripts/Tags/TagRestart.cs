using Unity.Entities;

namespace DOTS.Tags
{
    [GenerateAuthoringComponent]
    public struct TagRestart : IComponentData
    {
        public bool isRestartReady;
    }
}