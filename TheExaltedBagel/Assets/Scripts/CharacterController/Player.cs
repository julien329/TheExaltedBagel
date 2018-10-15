using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Collider2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float rotationSpeedX = 400f;
    [SerializeField] private float timeToIdle = 4f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float maxVelocityY = 17.5f;
    [SerializeField] private float timeToJumpApex = 0.35f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float accelerationTimeAirborne = 0.2f;
    [SerializeField] private float accelerationTimeGrounded = 0.1f;

    private float rotationXTarget = 180f;
    private float idleTimer = -1f;
    private float charHeight;
    private float gravity;
    private float jumpVelocity;
    private float velocityXSmoothing;
    private Vector3 velocity;
    private Controller2D controller;
    private Transform visualTransform;
    private Animator animator;

    private const float ROTATION_RIGHT = 90f;
    private const float ROTATION_IDLE = 180f;
    private const float ROTATION_LEFT = 270f;

    [NonSerialized] public float gravityDirection = 1f; 

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.controller = GetComponent<Controller2D>();
        this.visualTransform = this.transform.Find("Q-Mon1");
        this.animator = this.visualTransform.GetComponent<Animator>();
        this.charHeight = GetComponent<Collider2D>().offset.y * 2f;
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

        // Invert gravity if key is pressed
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            this.gravityDirection *= -1;
            float posOffset = (this.gravityDirection == 1) ? 0f : this.charHeight;
            this.visualTransform.localPosition = new Vector3(0f, posOffset, 0f);
            this.visualTransform.localScale = new Vector3(1f, gravityDirection, this.visualTransform.localScale.z);
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
    private void Animate(Vector2 input) {
        // Moving animations
        if (input.x != 0f)
        {
            this.idleTimer = this.timeToIdle;

            this.animator.SetBool("IsRunning", true);
            this.animator.SetFloat("RunningSpeed", Mathf.Abs(this.velocity.x) / this.moveSpeed);

            this.rotationXTarget = (Mathf.Sign(input.x) == 1) ? ROTATION_RIGHT : ROTATION_LEFT;
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
                    this.rotationXTarget = ROTATION_IDLE;
                }
            }
        }

        // Rotate the player according to the target rotation (left / right / idle)
        if ((this.rotationXTarget == ROTATION_RIGHT && this.visualTransform.localEulerAngles.y >= ROTATION_RIGHT) 
            || (this.rotationXTarget == ROTATION_LEFT && this.visualTransform.localEulerAngles.y <= ROTATION_LEFT)
            || (this.rotationXTarget == ROTATION_IDLE && this.visualTransform.localEulerAngles.y != ROTATION_IDLE))
        { 
            float direction = (this.rotationXTarget == ROTATION_RIGHT || (this.rotationXTarget == ROTATION_IDLE && this.visualTransform.localEulerAngles.y > ROTATION_IDLE)) ? -1f : 1f;
            this.visualTransform.Rotate(Vector3.up * direction * Time.deltaTime * rotationSpeedX);
            
            if (this.rotationXTarget == ROTATION_RIGHT && this.visualTransform.localEulerAngles.y < ROTATION_RIGHT)
            {
                this.visualTransform.localEulerAngles = new Vector3(this.visualTransform.localEulerAngles.x, ROTATION_RIGHT, this.visualTransform.localEulerAngles.z);
            }
            else if (this.rotationXTarget == ROTATION_LEFT && this.visualTransform.localEulerAngles.y > ROTATION_LEFT)
            {
                this.visualTransform.localEulerAngles = new Vector3(this.visualTransform.localEulerAngles.x, ROTATION_LEFT, this.visualTransform.localEulerAngles.z);
            }
            else if (this.rotationXTarget == ROTATION_IDLE && Mathf.Abs(this.visualTransform.localEulerAngles.y - ROTATION_IDLE) < Time.deltaTime * rotationSpeedX)
            {
                this.visualTransform.localEulerAngles = new Vector3(this.visualTransform.localEulerAngles.x, ROTATION_IDLE, this.visualTransform.localEulerAngles.z);
            }
        }
    }
}