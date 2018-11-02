using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AIMonster2 : MonoBehaviour
{
    public enum EnemyType { SlowMover, FastMover, Charger, FastFollow }
    public enum EnemyTheme { Normal, Winter, Fire }

    //For all enemies:
    [SerializeField] public EnemyType type = EnemyType.SlowMover;
    [SerializeField] public EnemyTheme theme = EnemyTheme.Normal;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float animSpeedModifier = 1f;
    [SerializeField] private float flipInterval = 2.0f;
    [SerializeField] private float gravity = -50f;
    [SerializeField] private float detectionRadius = 5f;
    private float maxVelocityY = 17.5f;
    private float accelerationTimeAirborne = 0.2f;
    private float accelerationTimeGrounded = 0.1f;

    private float gravityDirection = 1f;
    private float rotationHTarget = 180f;
    private float jumpVelocity;
    private float velocityXSmoothing;
    private float turnAroundTimer;
    private int direction = 1;
    private Vector3 velocity;
    private Controller2D controller;
    private Animator animator;
    private Transform rotYTransform;
    private Transform rotZTransform;
    private Transform playerTranform;

    private const float ROTATION_RIGHT = 90f;
    private const float ROTATION_IDLE = 180f;
    private const float ROTATION_LEFT = 270f;

    //For Charger:
    [SerializeField] private float chargerSurpriseDelay = 1f;
    [SerializeField] private float chargerMultiplier = 3f;
    private float surpriseTimer;
    private int chargeDirection = 1;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.controller = GetComponent<Controller2D>();

        this.rotYTransform = this.transform.Find("RotationY");
        this.rotZTransform = this.rotYTransform.Find("RotationZ");

        this.animator = this.rotZTransform.Find("QMon").GetComponent<Animator>();
        this.playerTranform = GameObject.Find("Player").GetComponent<Transform>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        this.turnAroundTimer = this.flipInterval;
        this.surpriseTimer = this.chargerSurpriseDelay;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (Time.deltaTime > Mathf.Epsilon)
        {
            // Get player input in raw states
            Vector2 input = Behaviour();

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

    Vector2 Behaviour()
    {
        int dir = playerTranform.position.x - this.transform.position.x > 0 ? 1 : -1;
        float distance = Mathf.Abs(playerTranform.position.x - this.transform.position.x);

        switch (this.type)
        {
            case EnemyType.SlowMover:
            case EnemyType.FastMover:

                return Wander();

            case EnemyType.FastFollow:


                if (distance <= detectionRadius)
                {
                    return new Vector2(dir * 3, 0);
                }
                else if (this.controller.collisions.left || this.controller.collisions.right)
                {
                    this.turnAroundTimer = this.flipInterval;

                    if (this.controller.collisions.left && this.direction == -1)
                    {
                        this.direction = 1;
                    }
                    if (this.controller.collisions.right && this.direction == 1)
                    {
                        this.direction = -1;
                    }

                    return Wander();
                }
                else if (distance > detectionRadius)
                {
                    return Wander();
                }
                else
                {
                    return Wander();
                }

            case EnemyType.Charger:
                
                if (this.controller.collisions.left || this.controller.collisions.right)
                {
                    this.transform.Find("Exclamation").GetComponent<TextMesh>().text = "";
                    this.surpriseTimer = this.chargerSurpriseDelay;
                    this.turnAroundTimer = this.flipInterval;

                    if (this.controller.collisions.left && this.direction == -1)
                    {
                        this.direction = 1;
                    }
                    if (this.controller.collisions.right && this.direction == 1)
                    {
                        this.direction = -1;
                    }

                    return Wander();
                }
                else if (distance <= detectionRadius && distance > 1 && this.surpriseTimer == 1f)
                {
                    this.chargeDirection = dir;
                    this.surpriseTimer -= Time.deltaTime;
                    this.transform.Find("Exclamation").GetComponent<TextMesh>().text = "!";
                    return new Vector2(this.chargeDirection, 0);
                }
                else if (this.surpriseTimer < this.chargerSurpriseDelay && this.surpriseTimer > 0)
                {
                    this.surpriseTimer -= Time.deltaTime;
                    if (surpriseTimer <= 0)
                    {
                        return new Vector2(this.chargeDirection * chargerMultiplier, 0);
                    }
                    return new Vector2(0, 0);
                }
                else if(surpriseTimer <= 0)
                {
                    return new Vector2(this.chargeDirection * chargerMultiplier, 0);
                }

                return Wander();

            default:
                return Wander();
        }
        
    }

    Vector2 Wander()
    {
        this.turnAroundTimer -= Time.deltaTime;
        if (this.turnAroundTimer < 0)
        {
            this.turnAroundTimer = this.flipInterval;
            this.direction *= -1;
        }
        return new Vector2(this.direction, 0);
    }
}