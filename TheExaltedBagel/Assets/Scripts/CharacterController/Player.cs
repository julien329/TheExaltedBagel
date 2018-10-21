using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Collider))]
public class Player : MonoBehaviour
{
    [NonSerialized] public uint gravityChargeCount = 3;
    public uint gravityChargeMax = 3;

    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] private float timeToIdle = 4f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float maxVelocityY = 17.5f;
    [SerializeField] private float timeToJumpApex = 0.35f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float accelerationTimeAirborne = 0.2f;
    [SerializeField] private float accelerationTimeGrounded = 0.1f;

    private float gravityDirection = 1f; 
    private float rotationHTarget = 180f;
    private float rotationVTarget = 0f;
    private float idleTimer = -1f;
    private float gravity;
    private float jumpVelocity;
    private float velocityXSmoothing;
    private Vector3 velocity;
    private Controller2D controller;
    private BoxCollider boxCollider;
    private Animator animator;
    private Transform rotYTransform;
    private Transform rotZTransform;

    private const float ROTATION_RIGHT = 90f;
    private const float ROTATION_IDLE = 180f;
    private const float ROTATION_LEFT = 270f;
    private const float ROTATION_UP = 180f;
    private const float ROTATION_DOWN = 0f;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.controller = GetComponent<Controller2D>();
        this.boxCollider = GetComponent<BoxCollider>();

        this.rotYTransform = this.transform.Find("RotationY");
        this.rotZTransform = this.rotYTransform.Find("RotationZ");

        this.animator = this.rotZTransform.Find("Q-Mon1").GetComponent<Animator>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start ()
    {
        this.controller = GetComponent<Controller2D>();

        // Formula : deltaMovement = velocityInitial * time + (acceleration * time^2) / 2  -->  where acceleration = gravity and velocityInitial is null
        this.gravity = -(2 * this.jumpHeight) / Mathf.Pow(this.timeToJumpApex, 2);
        // Formula : velocityFinal = velocityInitial + acceleration * time  -->  where velocityFinal = jumpVelocity and velocityInitial is null
        this.jumpVelocity = Mathf.Abs(this.gravity) * this.timeToJumpApex;
	}

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update ()
    {
        // Get player input in raw states
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Check horizontal and vertical movements
        MoveH(input);
        MoveV(input);

        // Rotate player according to movements and gravity
        RotationH();
        RotationV();

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
    private void MoveV(Vector2 input) {
        // If there is a collision in Y axis, reset velocity
        if (this.controller.collisions.above || this.controller.collisions.below)
        {
            this.velocity.y = 0;
        }

        if (this.controller.collisions.below)
        {
            this.gravityChargeCount = this.gravityChargeMax;
        }

        // Invert gravity if key is pressed or when the player, we need to spawn the player upside down
        if (Input.GetKeyDown(KeyCode.LeftControl) && this.gravityChargeCount > 0)
        {
            this.gravityChargeCount--;
            this.gravityDirection *= -1;
            this.rotationVTarget = (this.gravityDirection == 1) ? ROTATION_DOWN : ROTATION_UP;
        }

        // If the jump key is pressed
        if (Input.GetKeyDown(KeyCode.Space) && this.controller.collisions.below)
        {
            this.velocity.y = this.jumpVelocity * this.gravityDirection;
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
                rotateAngle = direction * Mathf.Abs(this.rotYTransform.localEulerAngles.y - this.rotationHTarget);
            }

            this.rotYTransform.Rotate(rotYTransform.up, rotateAngle, Space.World);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void RotationV()
    {
        // Rotate the player according to the gravity (up / down / idle)
        if ((this.rotationVTarget == ROTATION_DOWN && this.rotZTransform.localEulerAngles.z > ROTATION_DOWN)
            || (this.rotationVTarget == ROTATION_UP && this.rotZTransform.localEulerAngles.z < ROTATION_UP))
        {
            float direction = (this.rotationVTarget == ROTATION_DOWN) ? -1f : 1f;
            Vector3 centerPos = this.boxCollider.bounds.center;
            float rotateAngle = direction * Time.deltaTime * rotationSpeed;

            float newRot = this.rotZTransform.localEulerAngles.z + rotateAngle;
            if (newRot < ROTATION_DOWN || newRot > ROTATION_UP)
            {
                rotateAngle = direction * Mathf.Abs(this.rotZTransform.localEulerAngles.z - this.rotationVTarget);
            }

            this.rotZTransform.RotateAround(centerPos, rotZTransform.forward, rotateAngle);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void Animate(Vector2 input) 
    {
        // Moving animations
        if (input.x != 0f)
        {
            this.idleTimer = this.timeToIdle;

            this.animator.SetBool("IsRunning", true);
            this.animator.SetFloat("RunningSpeed", Mathf.Abs(this.velocity.x) / this.moveSpeed);

            this.rotationHTarget = (Mathf.Sign(input.x) == 1) ? ROTATION_RIGHT : ROTATION_LEFT;
        }
        // Idle animations
        else
        {
            this.animator.SetBool("IsRunning", false);

            // Countdown before idle
            if (this.idleTimer > 0f)
            {
                this.idleTimer -= Time.deltaTime;
                if (this.idleTimer <= 0f)
                {
                    this.idleTimer = -1f;
                    this.rotationHTarget = ROTATION_IDLE;
                }
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void RespawnPlayer(Vector3 spawnPosition, float gravityDirection)
    {
        this.transform.position = spawnPosition;
        this.gravityDirection = gravityDirection;

        this.rotationVTarget = (this.gravityDirection == 1) ? ROTATION_DOWN : ROTATION_UP;

        this.velocity = Vector3.zero;
        this.velocityXSmoothing = 0f;  
    }
}