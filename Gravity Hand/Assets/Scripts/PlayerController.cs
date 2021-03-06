﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    bool isGrounded;

    private Vector3 velocity;

    void Update()
    {
        isGrounded = controller.isGrounded;
        
        //Reset players velocity when grounded
        if(isGrounded && velocity.y <0)
        {
            velocity.y = 0f;
        }

        //Movement enabled on Input * speed value
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        //Calculates appropriate jump height
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        //Applies gravity to the players fall speed over time
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
