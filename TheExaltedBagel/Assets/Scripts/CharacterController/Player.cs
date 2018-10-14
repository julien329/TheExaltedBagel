using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(Collider2D))]
public class Player : MonoBehaviour {

    [SerializeField] private float timeToIdle = 4f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float maxVelocityY = 17.5f;
    [SerializeField] private float timeToJumpApex = 0.35f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float accelerationTimeAirborne = 0.2f;
    [SerializeField] private float accelerationTimeGrounded = 0.1f;

    private float idleTimer = -1f;
    private float charHeight;
    private float facingDirection;
    private float gravity;
    private float jumpVelocity;
    private float velocityXSmoothing;
    private Vector3 velocity;
    private Controller2D controller;
    private Transform visualTransform;
    private Animator animator;

    [NonSerialized] public float gravityDirection = 1f; 

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake() {
        this.controller = GetComponent<Controller2D>();
        this.visualTransform = this.transform.Find("Q-Mon1");
        this.animator = this.visualTransform.GetComponent<Animator>();
        this.charHeight = GetComponent<Collider2D>().offset.y * 2f;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start () {
        this.controller = GetComponent<Controller2D>();

        // Formula : deltaMovement = velocityInitial * time + (acceleration * time^2) / 2  -->  where acceleration = gravity and velocityInitial is null
        this.gravity = -(2 * this.jumpHeight) / Mathf.Pow(this.timeToJumpApex, 2);
        // Formula : velocityFinal = velocityInitial + acceleration * time  -->  where velocityFinal = jumpVelocity and velocityInitial is null
        this.jumpVelocity = Mathf.Abs(this.gravity) * this.timeToJumpApex;
	}

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update () {
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
    private void MoveH(Vector2 input) {
        // Set target velocity according to user input
        float targetVelocityX = input.x * this.moveSpeed;
        // Smooth velocity (use acceleration). Change smoothing value if grounded or airborne
        this.velocity.x = Mathf.SmoothDamp(this.velocity.x, targetVelocityX, ref this.velocityXSmoothing, 
            (this.controller.collisions.below) ? this.accelerationTimeGrounded : this.accelerationTimeAirborne);

        // If speed too small, set to null
        if (Mathf.Abs(this.velocity.x) < 0.1f) {
            this.velocity.x = 0f;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveV(Vector2 input) {
        // If there is a collision in Y axis, reset velocity
        if (this.controller.collisions.above || this.controller.collisions.below) {
            this.velocity.y = 0;
        }

        // Invert gravity if key is pressed
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            this.gravityDirection *= -1;
            float posOffset = (this.gravityDirection == 1) ? 0f : this.charHeight;
            this.visualTransform.localPosition = new Vector3(0f, posOffset, 0f);
            this.visualTransform.localScale = new Vector3(1f, gravityDirection, this.visualTransform.localScale.z);
        }

        // If the jump key is pressed
        if (Input.GetKeyDown(KeyCode.Space) && this.controller.collisions.below) {
            this.velocity.y = this.jumpVelocity * this.gravityDirection;
        }

        // Add gravity force downward to Y velocity
        this.velocity.y += this.gravity * this.gravityDirection * Time.deltaTime;

        if (Mathf.Abs(this.velocity.y) > this.maxVelocityY) {
            this.velocity.y = Mathf.Sign(this.velocity.y) * this.maxVelocityY;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void Animate(Vector2 input) {
        if (input.x != 0f) {
            idleTimer = timeToIdle;

            // Running anim
            this.animator.SetBool("IsRunning", true);
            this.animator.SetFloat("RunningSpeed", Mathf.Abs(this.velocity.x) / this.moveSpeed);

            // Set proper facing direction according to x velocity
            float velocityXDirection = Mathf.Sign(input.x);
            this.facingDirection = velocityXDirection;

            if (velocityXDirection == 1) {
                this.animator.SetTrigger("FlipRight");
            }
            else {
                this.animator.SetTrigger("FlipLeft");
            }
        }
        else {
            // Idle anim
            this.animator.SetBool("IsRunning", false);

            if (idleTimer > 0f) {
                idleTimer -= Time.deltaTime;
                if (idleTimer <= 0f) {
                    idleTimer = -1f;
                    this.animator.ResetTrigger("FlipRight");
                    this.animator.ResetTrigger("FlipLeft");
                    this.animator.SetTrigger("Idle");
                }
            }
        }
    }
}