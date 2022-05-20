using Unity.Entities;
using UnityEngine.UI;

namespace DOTS.Components
{
    [GenerateAuthoringComponent]
    public class UIText : IComponentData
    {
        public Text text;
    }
}