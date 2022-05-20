using System;
using System.Collections.Generic;
using System.Linq;
using DOTS.Components;
using DOTS.Enum;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Rendering;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using Hash128 = UnityEngine.Hash128;
using Random = System.Random;

public enum Amount
{
    Min,
    Max
}

public enum Copies
{
    None,
    Include,
    Only
}

public enum Get
{
    All,
    Random
}

public struct Filters
{
    public Amount amountElements;
    public Copies useCopies;
    public Get fromManyAnswers;
}

struct DataBlob
{
    private BlobAssetReference<BlobData> _data;

    public DataBlob(BlobAssetReference<BlobData> data)
    {
        _data = data;
    }

    public string Get(NativeList<byte> inputData, Filters filters)
    {
        var rooms = new Dictionary<byte, bool>();
        
        ref var value = ref _data.Value.data;
        for (var i = 0; i < value.Length; i++)
        {
            var interactives = new Dictionary<byte, byte>();
            ref var interactive = ref value[i].interactive;

            var numberCoincidences = 0;

            for (var j = 0; j < interactive.Length; j++)
            {
                foreach (var data in inputData)
                {
                    if (interactive[j] == data)
                    {
                        if (!interactives.ContainsKey(data))
                            interactives.Add(data, 1);
                        else
                            interactives[data] = (byte)(interactives[data] + 1);
                        break;
                    }
                }
            }

            for (var m = 0; m < interactives.Count;m++)
            {
                numberCoincidences += interactives[interactives.Keys.ElementAt(m)];
            }

            if (numberCoincidences == interactives.Count && interactives.Count != 0 && inputData.Length == interactives.Count)
            {
                rooms.Add((byte)i, false);
            }
            else if (numberCoincidences > interactives.Count && interactives.Count != 0 && inputData.Length == interactives.Count)
            {
                rooms.Add((byte)i, true);
            }
        }

        var count = (filters.amountElements == Amount.Min) ? 100 : 0;
        var listResult = new List<string>();

        foreach (var index in rooms.Keys)
        {
            ref var interactive = ref value[index].interactive;
            switch (filters.amountElements)
            {
                case Amount.Min:
                    if (count > interactive.Length)
                    {
                        count = interactive.Length;
                        listResult.Clear();
                        //listResult.Add(value[index].name.ToString());
                    }

                    break;
                case Amount.Max:
                    if (count < interactive.Length)
                    {
                        count = interactive.Length;
                        listResult.Clear();
                        //listResult.Add(value[index].name.ToString());
                    }

                    break;
            }

            if (count == interactive.Length)
            {
                if (filters.useCopies == Copies.Include)
                    listResult.Add(value[index].name.ToString());
                else if (filters.useCopies == Copies.Only && rooms[index])
                    listResult.Add(value[index].name.ToString());
                else if (filters.useCopies == Copies.None && !rooms[index])
                    listResult.Add(value[index].name.ToString());
            }
        }

        if (listResult.Count > 1)
        {
            if(filters.fromManyAnswers == global::Get.Random)
                return listResult[new Random().Next(listResult.Count)];
            if (filters.fromManyAnswers == global::Get.All)
            {
                var resultAll = "";
                foreach (var result in listResult)
                {
                    resultAll += result + "\n";
                }

                return resultAll;
            }
        }

        return listResult.Count == 1 ? listResult[0] : "null";
    }
}

namespace DOTS.Systems.SceneDataBlob
{
    //[UpdateAfter(typeof(SetDataSubSceneSystem))]
    public partial class GetDataSubSceneSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Enabled = false;
        }

        protected override void OnUpdate()
        {
            NativeArray<FixedString4096Bytes> answer =
                new NativeArray<FixedString4096Bytes>(1, Allocator.TempJob);

            var interactiveGroup = GetComponentDataFromEntity<InteractiveComponent>();
            var interactiveQuery = GetEntityQuery(ComponentType.ReadOnly<InteractiveComponent>())
                .ToEntityArray(Allocator.TempJob);

            var data =
                new NativeList<byte>(Allocator.TempJob);

            for (int i = 0; i < interactiveQuery.Length; i++)
            {
                var interactive = interactiveGroup[interactiveQuery[i]];
                if (interactive.inHand == HandType.Right)
                {
                    data.Add((byte)interactiveGroup[interactiveQuery[i]].id);
                }
            }

            var result = GetEntityQuery(ComponentType.ReadOnly<ReferenceData>())
                .ToComponentDataArray<ReferenceData>(Allocator.Temp);
            var dataBlob = new DataBlob(result[0].data);

            var filter = new Filters()
            {
                amountElements = Amount.Min,
                useCopies = Copies.Include,
                fromManyAnswers = Get.All
            };

            answer[0] = dataBlob.Get(data, filter);
            Entities.ForEach((UIText text) =>
            {
                text.text.text = answer[0].Value;
                answer.Dispose();
                result.Dispose();
                data.Dispose();
            }).WithoutBurst().Run();
            interactiveQuery.Dispose();
        }
    }
}