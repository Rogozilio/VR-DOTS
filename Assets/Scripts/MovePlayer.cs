using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public float speed = 1f;
    private Transform _camera;
    private float horizontalAxis;
    private float verticalAxis;

    private void Start()
    {
        _camera = Camera.main.transform;
        foreach (var map in inputActionAsset.actionMaps)
        {
            if (map.name == "XRI LeftHand")
            {
                map["Move"].performed += updateAxis;
                map["Move"].canceled += zeroAxis;
            }
        }
    }

    private void zeroAxis(InputAction.CallbackContext obj)
    {
        horizontalAxis = 0;
        verticalAxis = 0; 
    }
    private void updateAxis(InputAction.CallbackContext obj)
    {
        horizontalAxis = GameController.XRLeftHand.input.move.x;//UnityEngine.Input.GetAxis("Horizontal");
        verticalAxis = GameController.XRLeftHand.input.move.y; //UnityEngine.Input.GetAxis("Vertical");
    }

    private void Update()
    {
        var forward = _camera.forward;
        var right = _camera.right;
        
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
 
        var desiredMoveDirection = forward * verticalAxis + right * horizontalAxis;

        transform.Translate(desiredMoveDirection * (speed * Time.deltaTime));
    }
}