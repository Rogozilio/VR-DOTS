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

    public static XRCameraInput XRCamera;
    public static XRLeftHand XRLeftHand;
    public static XRRightHand XRRightHand;

    private void Awake()
    {
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
                case "XRI HMD":
                    map["Position"].performed += (ctx) => {
                        XRCamera.position = ctx.ReadValue<Vector3>(); };
                    map["Rotation"].performed += (ctx) => {
                        XRCamera.rotation = (ctx.ReadValue<Quaternion>()); };
                    break;
                case "XRI LeftHand":
                    map["Position"].performed += (ctx) => {
                        XRLeftHand.input.position = ctx.ReadValue<Vector3>(); };
                    map["Rotation"].performed += (ctx) => {
                        XRLeftHand.input.rotation = (ctx.ReadValue<Quaternion>()); };
                    map["Tracking State"].performed += (ctx) => {
                        XRLeftHand.input.trackingState = ctx.ReadValue<int>(); };
                    map["Select"].performed += (ctx) => {
                        XRLeftHand.input.select = (ctx.ReadValue<float>()); };
                    map["Select Value"].performed += (ctx) => {
                        XRLeftHand.input.selectValue = ctx.ReadValue<float>(); };
                    map["Activate"].performed += (ctx) => {
                        XRLeftHand.input.activate = (ctx.ReadValue<float>()); };
                    map["Activate Value"].performed += (ctx) => {
                        XRLeftHand.input.activateValue = (ctx.ReadValue<float>()); };
                    map["UI Press"].performed += (ctx) => {
                        XRLeftHand.input.uiPress = (ctx.ReadValue<float>()); };
                    map["UI Press Value"].performed += (ctx) => {
                        XRLeftHand.input.uiPressValue = (ctx.ReadValue<float>()); };
                    //map["Haptic Device"].performed += (ctx) => {
                    //     XRLeftHand.input.hapticDevice = (ctx.ReadValue<float3>()); };
                    // map["Teleport Select"].performed += (ctx) => {
                    //     XRLeftHand.input.teleportSelect = (ctx.ReadValue<float>()); };
                    // map["Teleport Mode Activate"].performed += (ctx) => {
                    //     XRLeftHand.input.teleportModeActivate = (ctx.ReadValue<float2>()); };
                    // map["Teleport Mode Cancel"].performed += (ctx) => {
                    //     XRLeftHand.input.teleportModeCancel = (ctx.ReadValue<float>()); };
                    map["Turn"].performed += (ctx) => {
                        XRLeftHand.input.turn = (ctx.ReadValue<Vector2>()); };
                    map["Move"].performed += (ctx) => {
                        XRLeftHand.input.move = (ctx.ReadValue<Vector2>()); };
                    map["Rotate Anchor"].performed += (ctx) => {
                        XRLeftHand.input.rotateAnchor = (ctx.ReadValue<Vector2>()); };
                    map["Translate Anchor"].performed += (ctx) => {
                        XRLeftHand.input.translateAnchor = (ctx.ReadValue<Vector2>()); };
                    break;
                case "XRI RightHand":
                    map["Position"].performed += (ctx) => {
                        XRRightHand.input.position = ctx.ReadValue<Vector3>(); };
                    map["Rotation"].performed += (ctx) => {
                        XRRightHand.input.rotation = (ctx.ReadValue<Quaternion>()); };
                    map["Tracking State"].performed += (ctx) => {
                        XRRightHand.input.trackingState = ctx.ReadValue<int>(); };
                    map["Select"].performed += (ctx) => {
                        XRRightHand.input.select = (ctx.ReadValue<float>()); };
                    map["Select Value"].performed += (ctx) => {
                        XRRightHand.input.selectValue = ctx.ReadValue<float>(); };
                    map["Activate"].performed += (ctx) => {
                        XRRightHand.input.activate = (ctx.ReadValue<float>()); };
                    map["Activate Value"].performed += (ctx) => {
                        XRRightHand.input.activateValue = (ctx.ReadValue<float>()); };
                    map["UI Press"].performed += (ctx) => {
                        XRRightHand.input.uiPress = (ctx.ReadValue<float>()); };
                    map["UI Press Value"].performed += (ctx) => {
                        XRRightHand.input.uiPressValue = (ctx.ReadValue<float>()); };
                    //map["Haptic Device"].performed += (ctx) => {
                    //     XRRightHand.input.hapticDevice = (ctx.ReadValue<float3>()); };
                    map["Teleport Select"].performed += (ctx) => {
                        XRRightHand.input.teleportSelect = (ctx.ReadValue<float>()); };
                    map["Teleport Mode Activate"].performed += (ctx) => {
                        XRRightHand.input.teleportModeActivate = (ctx.ReadValue<float2>()); };
                    map["Teleport Mode Cancel"].performed += (ctx) => {
                        XRRightHand.input.teleportModeCancel = (ctx.ReadValue<float>()); };
                    map["Turn"].performed += (ctx) => {
                        XRRightHand.input.turn = (ctx.ReadValue<Vector2>()); };
                    map["Move"].performed += (ctx) => {
                        XRRightHand.input.move = (ctx.ReadValue<Vector2>()); };
                    map["Rotate Anchor"].performed += (ctx) => {
                        XRRightHand.input.rotateAnchor = (ctx.ReadValue<Vector2>()); };
                    map["Translate Anchor"].performed += (ctx) => {
                        XRRightHand.input.translateAnchor = (ctx.ReadValue<Vector2>()); };
                    break;
            }
        }
    }

    private void Update()
    {
        cameraToWorld = camera.position;
    }
}

public struct XRCameraInput
{
    public float3 position;
    public Quaternion rotation;
}

public struct XRLeftHand
{
    public Input input;
}

public struct XRRightHand
{
    public Input input;
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