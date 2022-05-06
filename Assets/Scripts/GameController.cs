using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public static GameController gameController;
    public Transform camera;
    public  static float3 cameraToWorld;
    public InputActionAsset inputActionAsset;
    
    public static Hands Hands;

    private void Awake()
    {
        Hands.input = new Input[2];
        
        if (gameController && gameController != this)
        {
            Destroy(this);
        }

        gameController = this;

        cameraToWorld = camera.position;
        foreach (InputActionMap map in inputActionAsset.actionMaps)
        {
            map.Enable();
            switch (map.name)
            {
                case "XRI LeftHand":
                    map["Position"].performed += (ctx) => {
                        Hands.input[0].position = ctx.ReadValue<Vector3>() + new Vector3(
                            cameraToWorld.x, 0, cameraToWorld.z); };
                    map["Rotation"].performed += (ctx) => {
                        Hands.input[0].rotation = (ctx.ReadValue<Quaternion>()); };
                    map["Tracking State"].performed += (ctx) => {
                        Hands.input[0].trackingState = ctx.ReadValue<int>(); };
                    map["Select"].performed += (ctx) => {
                        Hands.input[0].select = (ctx.ReadValue<float>()); };
                    map["Select Value"].performed += (ctx) => {
                        Hands.input[0].selectValue = ctx.ReadValue<float>(); };
                    map["Activate"].performed += (ctx) => {
                        Hands.input[0].activate = (ctx.ReadValue<float>()); };
                    map["Activate Value"].performed += (ctx) => {
                        Hands.input[0].activateValue = (ctx.ReadValue<float>()); };
                    map["UI Press"].performed += (ctx) => {
                        Hands.input[0].uiPress = (ctx.ReadValue<float>()); };
                    map["UI Press Value"].performed += (ctx) => {
                        Hands.input[0].uiPressValue = (ctx.ReadValue<float>()); };
                    //map["Haptic Device"].performed += (ctx) => {
                    //     XRLeftHand.input.hapticDevice = (ctx.ReadValue<float3>()); };
                    // map["Teleport Select"].performed += (ctx) => {
                    //     XRLeftHand.input.teleportSelect = (ctx.ReadValue<float>()); };
                    // map["Teleport Mode Activate"].performed += (ctx) => {
                    //     XRLeftHand.input.teleportModeActivate = (ctx.ReadValue<float2>()); };
                    // map["Teleport Mode Cancel"].performed += (ctx) => {
                    //     XRLeftHand.input.teleportModeCancel = (ctx.ReadValue<float>()); };
                    map["Turn"].performed += (ctx) => {
                        Hands.input[0].turn = (ctx.ReadValue<Vector2>()); };
                    map["Move"].performed += (ctx) => {
                        Hands.input[0].move = (ctx.ReadValue<Vector2>()); };
                    map["Rotate Anchor"].performed += (ctx) => {
                        Hands.input[0].rotateAnchor = (ctx.ReadValue<Vector2>()); };
                    map["Translate Anchor"].performed += (ctx) => {
                        Hands.input[0].translateAnchor = (ctx.ReadValue<Vector2>()); };
                    break;
                case "XRI RightHand":
                    map["Position"].performed += (ctx) => {
                        Hands.input[1].position = ctx.ReadValue<Vector3>() + new Vector3(
                            cameraToWorld.x, 0, cameraToWorld.z); };
                    map["Rotation"].performed += (ctx) => {
                        Hands.input[1].rotation = (ctx.ReadValue<Quaternion>()); };
                    map["Tracking State"].performed += (ctx) => {
                        Hands.input[1].trackingState = ctx.ReadValue<int>(); };
                    map["Select"].performed += (ctx) => {
                        Hands.input[1].select = (ctx.ReadValue<float>()); };
                    map["Select Value"].performed += (ctx) => {
                        Hands.input[1].selectValue = ctx.ReadValue<float>(); };
                    map["Activate"].performed += (ctx) => {
                        Hands.input[1].activate = (ctx.ReadValue<float>()); };
                    map["Activate Value"].performed += (ctx) => {
                        Hands.input[1].activateValue = (ctx.ReadValue<float>()); };
                    map["UI Press"].performed += (ctx) => {
                        Hands.input[1].uiPress = (ctx.ReadValue<float>()); };
                    map["UI Press Value"].performed += (ctx) => {
                        Hands.input[1].uiPressValue = (ctx.ReadValue<float>()); };
                    //map["Haptic Device"].performed += (ctx) => {
                    //     XRRightHand.input.hapticDevice = (ctx.ReadValue<float3>()); };
                    map["Teleport Select"].performed += (ctx) => {
                        Hands.input[1].teleportSelect = (ctx.ReadValue<float>()); };
                    map["Teleport Mode Activate"].performed += (ctx) => {
                        Hands.input[1].teleportModeActivate = (ctx.ReadValue<float2>()); };
                    map["Teleport Mode Cancel"].performed += (ctx) => {
                        Hands.input[1].teleportModeCancel = (ctx.ReadValue<float>()); };
                    map["Turn"].performed += (ctx) => {
                        Hands.input[1].turn = (ctx.ReadValue<Vector2>()); };
                    map["Move"].performed += (ctx) => {
                        Hands.input[1].move = (ctx.ReadValue<Vector2>()); };
                    map["Rotate Anchor"].performed += (ctx) => {
                        Hands.input[1].rotateAnchor = (ctx.ReadValue<Vector2>()); };
                    map["Translate Anchor"].performed += (ctx) => {
                        Hands.input[1].translateAnchor = (ctx.ReadValue<Vector2>()); };
                    break;
            }
        }
    }

    private void Update()
    {
        cameraToWorld = camera.position;
    }
}

public struct Hands
{
    public Input[] input;
}

public struct Input
{
    public float3 position;
    public Quaternion rotation;
    public int trackingState;
    public float select;
    public float selectValue;
    public float activate;
    public float activateValue;
    public float uiPress;
    public float uiPressValue;
    public float3 hapticDevice;
    public float teleportSelect;
    public float2 teleportModeActivate;
    public float teleportModeCancel;
    public float2 turn;
    public float2 move;
    public float2 rotateAnchor;
    public float2 translateAnchor;
}