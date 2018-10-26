using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent (typeof(Collider))]
public class Player : MonoBehaviour
{
    [Header("MoveX")]
    [SerializeField] private float moveSpeedNormal = 7f;
    [SerializeField] private float moveSpeedWater = 3.5f;
    [SerializeField] private float moveSpeedIce = 10.5f;
    [SerializeField] private float accTimeAir = 0.2f;
    [SerializeField] private float accTimeGround = 0.1f;
    [SerializeField] private float accTimeIce = 0.4f;

    [Header("MoveY")]
    [SerializeField] private float maxVelocityY = 17.5f;
    [SerializeField] private float timeToJumpApex = 0.35f;
    [SerializeField] private float timeToJumpApexWater = 0.75f;
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float jumpHeightWater = 3.5f;
    //[SerializeField] private float bumpForce = 10f;

    [Header("Anim")]
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] private float timeToIdle = 4f;
    [SerializeField] private float minAnimSpeedRatio = 0.5f;
    [SerializeField] private GameObject splashParticles;
    [SerializeField] private GameObject deathParticles;

    [Header("Other")]
    [SerializeField] private uint gravityChargeMax = 3;

    private List<Collider> waterColliders = new List<Collider>();
    private uint gravityChargeCount = 3;
    private float moveSpeed;
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
    public uint GravityChargeCount
    {
        get { return this.gravityChargeCount; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public uint GravityChargeMax
    {
        get { return this.gravityChargeMax; }
    }

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

        ChangeEnvironment(false);

    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update ()
    {
        // Check horizontal and vertical movements
        MoveH();
        MoveV();

        // Rotate player according to movements and gravity
        RotationH();
        RotationV();

        // Send info to animator
        Animate();

        // Call move to check collisions and translate the player
        this.controller.Move(this.velocity * Time.deltaTime, gravityDirection);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            LevelManager.instance.KillPlayer(false);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnCollisionEnter(Collision collision)
    {
        // On entering water
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            // If not already in water
            if (this.waterColliders.Count == 0)
            {
                // Impact reduces current velocity according to difference in gravity
                float newGravity = -(2f * this.jumpHeightWater) / Mathf.Pow(this.timeToJumpApexWater, 2f);
                float impactForce = Mathf.Clamp(newGravity / this.gravity, 0f, 1f);
                this.velocity = new Vector3(this.velocity.x * impactForce, this.velocity.y * impactForce, 0f);

                // If we have a splash vfx
                if (this.splashParticles != null)
                {
                    Vector3 contactPos = new Vector3(this.transform.position.x, collision.collider.bounds.max.y, this.transform.position.z);
                    // If contact pos is within the water bounds
                    if (contactPos.x >= collision.collider.bounds.min.x && contactPos.x <= collision.collider.bounds.max.x)
                    {
                        // Add the spash object
                        GameObject particles = Instantiate(this.splashParticles);
                        particles.transform.position = contactPos;

                        // Play the vfx
                        ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
                        particleSystem.Play();
                    }
                }
            }

            // Memorize the water collider
            this.waterColliders.Add(collision.collider);
            // Change environment to water settings
            ChangeEnvironment(true);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnCollisionExit(Collision collision)
    {
        // On leaving water
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            // Remove saved collider
            this.waterColliders.Remove(collision.collider);
            // If we are no in any water
            if (this.waterColliders.Count == 0)
            {
                // Change environment to air settings
                ChangeEnvironment(false);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveH()
    {
        // Get current movespeed
        this.moveSpeed = GetMoveSpeedX();

        // Set target velocity according to user input
        float targetVelocityX = Input.GetAxis("Horizontal") * this.moveSpeed;
        // Smooth velocity (use acceleration). Change smoothing value if grounded or airborne
        this.velocity.x = Mathf.SmoothDamp(this.velocity.x, targetVelocityX, ref this.velocityXSmoothing, GetAccelerationTime());

        // If speed too small, set to null
        if (Mathf.Abs(this.velocity.x) < 0.1f)
        {
            this.velocity.x = 0f;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void MoveV() {
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
        if (Input.GetButtonDown("Gravity") && this.gravityChargeCount > 0)
        {
            this.gravityChargeCount--;
            this.gravityDirection *= -1;
            this.rotationVTarget = (this.gravityDirection == 1) ? ROTATION_DOWN : ROTATION_UP;
        }

        // If the jump key is pressed
        if (Input.GetButtonDown("Jump") && this.controller.collisions.below)
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
                this.rotYTransform.localEulerAngles = new Vector3(this.rotYTransform.localEulerAngles.x, this.rotationHTarget, this.rotYTransform.localEulerAngles.z);
            }
            else
            {
                this.rotYTransform.Rotate(this.rotYTransform.up, rotateAngle, Space.World);
            }
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
            float rotateAngle = direction * Time.deltaTime * rotationSpeed;

            float newRot = this.rotZTransform.localEulerAngles.z + rotateAngle;
            if (newRot < ROTATION_DOWN || newRot > ROTATION_UP)
            {
                float offset = (this.rotationVTarget == ROTATION_DOWN) ? 0f : this.boxCollider.size.y;
                this.rotZTransform.localPosition = new Vector3(0f, offset, 0f);
                this.rotZTransform.localEulerAngles = new Vector3(this.rotZTransform.localEulerAngles.x, this.rotZTransform.localEulerAngles.y, this.rotationVTarget);
            }
            else
            {
                this.rotZTransform.RotateAround(this.boxCollider.bounds.center, this.rotZTransform.forward, rotateAngle);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void Animate() 
    {
        // Moving animations
        float inputX = Input.GetAxis("Horizontal");
        if (inputX != 0f)
        {
            this.idleTimer = this.timeToIdle;

            this.animator.SetBool("IsRunning", true);
            this.animator.SetFloat("RunningSpeed", Mathf.Lerp(this.minAnimSpeedRatio, 1f, Mathf.Abs(this.velocity.x) / this.moveSpeed));

            this.rotationHTarget = (Mathf.Sign(inputX) == 1) ? ROTATION_RIGHT : ROTATION_LEFT;
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
    private void ChangeEnvironment(bool isWater)
    {
        float jump = (isWater) ? this.jumpHeightWater : this.jumpHeight;
        float time = (isWater) ? this.timeToJumpApexWater : this.timeToJumpApex;

        // Formula : deltaMovement = velocityInitial * time + (acceleration * time^2) / 2  -->  where acceleration = gravity and velocityInitial is null
        this.gravity = -(2f * jump) / Mathf.Pow(time, 2f);
        // Formula : velocityFinal = velocityInitial + acceleration * time  -->  where velocityFinal = jumpVelocity and velocityInitial is null
        this.jumpVelocity = Mathf.Abs(this.gravity) * time;

        this.moveSpeed = (isWater) ? this.moveSpeedWater : this.moveSpeedNormal;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private float GetAccelerationTime()
    {
        float accelerationTime;
        if (this.controller.collisions.below)
        {
            accelerationTime = (this.controller.collisions.isSlippery) ? this.accTimeIce : this.accTimeGround;
        }
        else
        {
            accelerationTime = this.accTimeAir;
        }

        return accelerationTime;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private float GetMoveSpeedX()
    {
        float moveSpeedX;
        if (this.waterColliders.Count > 0)
        {
            moveSpeedX = this.moveSpeedWater;
        }
        else
        {
            moveSpeedX = (this.controller.collisions.isSlippery) ? this.moveSpeedIce : this.moveSpeedNormal;
        }

        return moveSpeedX;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void KillPlayer()
    {
        if (this.deathParticles != null)
        {
            // Add the spash object
            GameObject particles = Instantiate(this.deathParticles);
            particles.transform.position = this.boxCollider.bounds.center;

            // Play the vfx
            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
            particleSystem.Play();
        }

        this.gameObject.SetActive(false);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnPlayer(Vector3 position, float gravityDirection)
    {
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }

        this.transform.position = position;
        this.gravityDirection = gravityDirection;

        this.rotationVTarget = (this.gravityDirection == 1) ? ROTATION_DOWN : ROTATION_UP;

        this.velocity = Vector3.zero;
        this.velocityXSmoothing = 0f;  
    }
}