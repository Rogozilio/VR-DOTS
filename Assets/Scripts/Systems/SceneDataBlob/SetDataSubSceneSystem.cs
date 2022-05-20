using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;


public struct Data
{
    public FixedString128Bytes name;
    public BlobArray<byte> interactive;
}

public struct BlobData
{
    public BlobArray<Data> data;
}

ref struct Blob
{
    private BlobBuilderArray<Data> _data;
    private BlobBuilder _blobBuilder;
    private int _count;

    public Blob(int length)
    {
        _blobBuilder = new BlobBuilder(Allocator.Temp);
        ref var root = ref _blobBuilder.ConstructRoot<BlobData>();
        _data = _blobBuilder.Allocate(ref root.data, length);
        _count = 0;
    }

    public void Add(string value, byte[] values)
    {
        _data[_count].name = value;
        var interactiveName =
            _blobBuilder.Allocate(ref _data[_count].interactive, values.Length);
        for (var i = 0; i < values.Length; i++)
        {
            interactiveName[i] = values[i];
        }

        _count++;
    }

    public BlobAssetReference<BlobData> CreateBlobAssetReference()
    {
        var result = _blobBuilder.CreateBlobAssetReference<BlobData>(Allocator.Persistent);
        _blobBuilder.Dispose();
        return result;
    }
}

namespace DOTS.Systems.SceneDataBlob
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SetDataSubSceneSystem : SystemBase
    {
        private EndInitializationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

        protected override void OnCreate()
        {
            Enabled = false;
        }

        protected override void OnStartRunning()
        {
            _endSimulationEntityCommandBufferSystem =
                World.GetOrCreateSystem<EndInitializationEntityCommandBufferSystem>();
            var cbs = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
            var sceneQuery = GetEntityQuery(ComponentType.ReadOnly<SceneReference>())
                .ToComponentDataArray<SceneReference>(Allocator.Temp);
            var interactiveQuery =
                GetEntityQuery(typeof(InteractiveComponent), typeof(SceneSection));
            var nameQuery =
                GetEntityQuery(typeof(NameRoom), typeof(SceneSection));
            var blob = new Blob(sceneQuery.Length);

            foreach (var scene in sceneQuery)
            {
                interactiveQuery.SetSharedComponentFilter(new SceneSection
                    { SceneGUID = scene.SceneGUID, Section = 0 });
                nameQuery.SetSharedComponentFilter(new SceneSection
                    { SceneGUID = scene.SceneGUID, Section = 0 });
                var interactiveEntity = interactiveQuery.ToEntityArray(Allocator.Temp);
                var names = new byte[interactiveQuery.CalculateEntityCount()];
                for (var i = 0; i < interactiveEntity.Length; i++)
                {
                    var interactiveGroup = GetComponentDataFromEntity<InteractiveComponent>(true);
                    names[i] = (byte)interactiveGroup[interactiveEntity[i]].id;
                }

                var name = nameQuery.ToComponentDataArray<NameRoom>()[0].Name;
                blob.Add(name, names);
                interactiveQuery.ResetFilter();
                interactiveEntity.Dispose();
            }

            var entity = cbs.CreateEntity();
            cbs.AddComponent(entity,
                new ReferenceData() { data = blob.CreateBlobAssetReference() });

            var sceneSystem = World.GetOrCreateSystem<SceneSystem>();
            foreach (var scene in sceneQuery)
            {
                sceneSystem.UnloadScene(scene.SceneGUID);
            }
            sceneQuery.Dispose();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            // var result = GetEntityQuery(ComponentType.ReadOnly<ReferenceData>())
            //     .ToComponentDataArray<ReferenceData>(Allocator.Temp);
            // for (int i = 0; i < result[0].data.Value.data.Length; i++)
            // {
            //     Debug.Log(result[0].data.Value.data[i].name);
            //     Debug.Log(result[0].data.Value.data[i].interactive.Length);
            //     for (int j = 0; j < result[0].data.Value.data[i].interactive.Length; j++)
            //     {
            //         Debug.Log(result[0].data.Value.data[i].interactive[j]);
            //     }
            // }
            //
            // result.Dispose();
        }
    }
}