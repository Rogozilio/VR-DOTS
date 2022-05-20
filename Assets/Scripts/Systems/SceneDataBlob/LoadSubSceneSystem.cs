using DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

namespace DOTS.Systems.SceneDataBlob
{
    //[UpdateBefore(typeof(SetDataSubSceneSystem))]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class LoadSubSceneSystem : SystemBase
    {
        private SceneSystem _sceneSystem;

        protected override void OnCreate()
        {
            _sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        }

        protected override void OnUpdate()
        {
            var sceneQuery = GetEntityQuery(ComponentType.ReadOnly<SceneReference>())
                .ToEntityArray(Allocator.Temp);

            foreach (var scene in sceneQuery)
            {
                if(!_sceneSystem.IsSceneLoaded(scene))
                {
                    return;
                }
            }

            World.GetExistingSystem<SetDataSubSceneSystem>().Enabled = true;
            World.GetExistingSystem<GetDataSubSceneSystem>().Enabled = true;
            Enabled = false;
        }
    }
}