﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMonster2 : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float animSpeedModifier = 1f;
    private float maxVelocityY = 17.5f;
    private float accelerationTimeAirborne = 0.2f;
    private float accelerationTimeGrounded = 0.1f;
    [SerializeField] private float flipInterval = 1.0f;
    [SerializeField] private float gravity = -50f;

    private float gravityDirection = 1f;
    private float rotationHTarget = 180f;
    //private float jumpVelocity;
    private float velocityXSmoothing;
    private float turnAroundTimer;
    private int direction = 1;
    private Vector3 velocity;
    private Controller2D controller;
    private Animator animator;
    private Transform rotYTransform;
    private Transform rotZTransform;

    private const float ROTATION_RIGHT = 90f;
    private const float ROTATION_IDLE = 180f;
    private const float ROTATION_LEFT = 270f;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.controller = GetComponent<Controller2D>();

        this.rotYTransform = this.transform.Find("RotationY");
        this.rotZTransform = this.rotYTransform.Find("RotationZ");

        this.animator = this.rotZTransform.Find("QMon").GetComponent<Animator>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        this.turnAroundTimer = this.flipInterval;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        //Timer tick
        this.turnAroundTimer -= Time.deltaTime;

        //Change the monster's direction when timer runs out. Reset timer.
        if (this.turnAroundTimer < 0)
        {
            this.turnAroundTimer = this.flipInterval;
            this.direction *= -1;
        }

        // Get player input in raw states
        Vector2 input = new Vector2(this.direction, 0);

        // Check horizontal and vertical movements
        MoveH(input);
        MoveV(input);

        // Rotate player according to movements and gravity
        RotationH();

        // Send info to animator
        Animate(input);

        // Call move to check collisions and translate the player
        this.controller.Move(this.velocity * Time.deltaTime, gravityDirection);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveH(Vector2 input)
    {
        // Set target velocity according to user input
        float targetVelocityX = input.x * this.moveSpeed;
        // Smooth velocity (use acceleration). Change smoothing value if grounded or airborne
        this.velocity.x = Mathf.SmoothDamp(this.velocity.x, targetVelocityX, ref this.velocityXSmoothing,
            (this.controller.collisions.below) ? this.accelerationTimeGrounded : this.accelerationTimeAirborne);

        // If speed too small, set to null
        if (Mathf.Abs(this.velocity.x) < 0.1f)
        {
            this.velocity.x = 0f;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveV(Vector2 input)
    {
        // If there is a collision in Y axis, reset velocity
        if (this.controller.collisions.above || this.controller.collisions.below)
        {
            this.velocity.y = 0;
        }

        // Add gravity force downward to Y velocity
        this.velocity.y += this.gravity * this.gravityDirection * Time.deltaTime;

        if (Mathf.Abs(this.velocity.y) > this.maxVelocityY)
        {
            this.velocity.y = Mathf.Sign(this.velocity.y) * this.maxVelocityY;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void RotationH()
    {
        // Rotate the player according to the target rotation (left / right / idle)
        if ((this.rotationHTarget == ROTATION_RIGHT && this.rotYTransform.localEulerAngles.y > ROTATION_RIGHT)
            || (this.rotationHTarget == ROTATION_LEFT && this.rotYTransform.localEulerAngles.y < ROTATION_LEFT)
            || (this.rotationHTarget == ROTATION_IDLE && this.rotYTransform.localEulerAngles.y != ROTATION_IDLE))
        {
            float direction = (this.rotationHTarget == ROTATION_RIGHT || (this.rotationHTarget == ROTATION_IDLE && this.rotYTransform.localEulerAngles.y > ROTATION_IDLE)) ? -1f : 1f;
            float rotateAngle = direction * Time.deltaTime * rotationSpeed;

            float newRot = this.rotYTransform.localEulerAngles.y + rotateAngle;
            if (newRot < ROTATION_RIGHT || newRot > ROTATION_LEFT || (Mathf.Abs(newRot - ROTATION_IDLE) < Time.deltaTime * rotationSpeed && this.rotationHTarget == ROTATION_IDLE))
            {
                this.rotYTransform.localEulerAngles = new Vector3(this.rotYTransform.localEulerAngles.x, this.rotationHTarget, this.rotYTransform.localEulerAngles.z);
            }
            else
            {
                this.rotYTransform.Rotate(this.rotYTransform.up, rotateAngle, Space.World);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void Animate(Vector2 input)
    {
        // Moving animations
        if (input.x != 0f)
        {
            this.animator.SetBool("IsMoving", true);
            this.animator.SetFloat("MonsterSpeed", animSpeedModifier * Mathf.Abs(this.velocity.x) / this.moveSpeed);

            this.rotationHTarget = (Mathf.Sign(input.x) == 1) ? ROTATION_RIGHT : ROTATION_LEFT;
        }
        // Idle animations
        else
        {
            this.animator.SetBool("IsMoving", false);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void RespawnPlayer(Vector3 spawnPosition, float gravityDirection)
    {
        this.transform.position = spawnPosition;
        this.gravityDirection = gravityDirection;

        this.velocity = Vector3.zero;
        this.velocityXSmoothing = 0f;
    }
}
