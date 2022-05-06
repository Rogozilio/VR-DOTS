using System.Collections;
using System.Collections.Generic;
using DOTS.Components;
using DOTS.Enum;
using DOTS.Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class InputControllerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var hands = new NativeArray<Input>(2, Allocator.TempJob);
        hands[0] = GameController.Hands.input[0];
        hands[1] = GameController.Hands.input[1];
        
        JobHandle inputLeftHandJob = Entities.ForEach(
            (ref InputControllerComponent input) =>
            {
                if(input.handType == HandType.None)
                    return;
                
                var i = (int) input.handType - 1;
                input.position = hands[i].position;
                input.rotation = hands[i].rotation;
                input.trackingState = hands[i].trackingState;
                input.select = hands[i].select;
                input.selectValue = hands[i].selectValue;
                input.activate = hands[i].activate;
                input.activateValue = hands[i].activateValue;
                input.uiPress = hands[i].uiPress;
                input.uiPressValue = hands[i].uiPressValue;
                input.hapticDevice = hands[i].hapticDevice;
                input.teleportSelect = hands[i].teleportSelect;
                input.teleportModeActivate = hands[i].teleportModeActivate;
                input.teleportModeCancel = hands[i].teleportModeCancel;
                input.turn = hands[i].turn;
                input.move = hands[i].move;
                input.rotateAnchor = hands[i].rotateAnchor;
                input.translateAnchor = hands[i].translateAnchor;
            }).Schedule(Dependency);
        inputLeftHandJob.Complete();

        hands.Dispose();
    }
}