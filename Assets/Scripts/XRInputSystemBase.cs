using System.Collections;
using System.Collections.Generic;
using DOTS.Components;
using DOTS.Tags;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class XRInputSystemBase : SystemBase
{
    protected override void OnUpdate()
    {
        float3 position = GameController.XRLeftHand.input.position + new float3(
            GameController.cameraToWorld.x, 0, GameController.cameraToWorld.z);
        quaternion rotation = GameController.XRLeftHand.input.rotation;
        int trackingState = GameController.XRLeftHand.input.trackingState;
        float select = GameController.XRLeftHand.input.select;
        float selectValue = GameController.XRLeftHand.input.selectValue;
        float activate = GameController.XRLeftHand.input.activate;
        float activateValue = GameController.XRLeftHand.input.activateValue;
        float uiPress = GameController.XRLeftHand.input.uiPress;
        float uiPressValue = GameController.XRLeftHand.input.uiPressValue;
        float3 hapticDevice = GameController.XRLeftHand.input.hapticDevice;
        float teleportSelect = GameController.XRLeftHand.input.teleportSelect;
        float2 teleportModeActivate = GameController.XRLeftHand.input.teleportModeActivate;
        float teleportModeCancel = GameController.XRLeftHand.input.teleportModeCancel;
        float2 turn = GameController.XRLeftHand.input.turn;
        float2 move = GameController.XRLeftHand.input.move;
        float2 rotateAnchor = GameController.XRLeftHand.input.rotateAnchor;
        float2 translateAnchor = GameController.XRLeftHand.input.translateAnchor;

        JobHandle inputLeftHandJob = Entities.WithAll<TagLeftHand>().ForEach(
            (ref XRHandInputControllerComponent input) =>
            {
                input.position = position;
                input.rotation = rotation;
                input.trackingState = trackingState;
                input.select = select;
                input.selectValue = selectValue;
                input.activate = activate;
                input.activateValue = activateValue;
                input.uiPress = uiPress;
                input.uiPressValue = uiPressValue;
                input.hapticDevice = hapticDevice;
                input.teleportSelect = teleportSelect;
                input.teleportModeActivate = teleportModeActivate;
                input.teleportModeCancel = teleportModeCancel;
                input.turn = turn;
                input.move = move;
                input.rotateAnchor = rotateAnchor;
                input.translateAnchor = translateAnchor;
            }).Schedule(Dependency);
        inputLeftHandJob.Complete();

        position = GameController.XRRightHand.input.position + new float3(
            GameController.cameraToWorld.x, 0, GameController.cameraToWorld.z);
        rotation = GameController.XRRightHand.input.rotation;
        trackingState = GameController.XRRightHand.input.trackingState;
        select = GameController.XRRightHand.input.select;
        selectValue = GameController.XRRightHand.input.selectValue;
        activate = GameController.XRRightHand.input.activate;
        activateValue = GameController.XRRightHand.input.activateValue;
        uiPress = GameController.XRRightHand.input.uiPress;
        uiPressValue = GameController.XRRightHand.input.uiPressValue;
        hapticDevice = GameController.XRRightHand.input.hapticDevice;
        teleportSelect = GameController.XRRightHand.input.teleportSelect;
        teleportModeActivate = GameController.XRRightHand.input.teleportModeActivate;
        teleportModeCancel = GameController.XRRightHand.input.teleportModeCancel;
        turn = GameController.XRRightHand.input.turn;
        move = GameController.XRRightHand.input.move;
        rotateAnchor = GameController.XRRightHand.input.rotateAnchor;
        translateAnchor = GameController.XRRightHand.input.translateAnchor;

        JobHandle inputRightHandJob = Entities.WithAll<TagRightHand>().ForEach(
            (ref XRHandInputControllerComponent input) =>
            {
                input.position = position;
                input.rotation = rotation;
                input.trackingState = trackingState;
                input.select = select;
                input.selectValue = selectValue;
                input.activate = activate;
                input.activateValue = activateValue;
                input.uiPress = uiPress;
                input.uiPressValue = uiPressValue;
                input.hapticDevice = hapticDevice;
                input.teleportSelect = teleportSelect;
                input.teleportModeActivate = teleportModeActivate;
                input.teleportModeCancel = teleportModeCancel;
                input.turn = turn;
                input.move = move;
                input.rotateAnchor = rotateAnchor;
                input.translateAnchor = translateAnchor;
            }).Schedule(Dependency);
        inputRightHandJob.Complete();
    }
}