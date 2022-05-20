using Unity.Entities;

namespace DOTS.Components
{
    [GenerateAuthoringComponent]
    public class NameRoom : IComponentData
    {
        public string Name;
    }
}