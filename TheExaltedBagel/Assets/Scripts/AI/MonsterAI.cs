using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MonsterAI : MonoBehaviour
{
    public enum EnemyType { SlowMover, FastMover, Charger, FastFollow, Jumper }
    public enum EnemyTheme { Normal, Winter, Fire }

    [Header("Common Settings")]
    [SerializeField] public EnemyType type = EnemyType.SlowMover;
    [SerializeField] public EnemyTheme theme = EnemyTheme.Normal;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float rotationSpeed = 500f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float animSpeedModifier = 1f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float travelDisctance = 5f;
    [SerializeField] private LayerMask layerMask;
    private Vector3 startPosition;

    private float gravity = -50f;
    private float gravityDirection = 1f;
    private float maxVelocityY = 17.5f;
    private float accelerationTimeAirborne = 0.2f;
    private float accelerationTimeGrounded = 0.1f;

    private float rotationHTarget = 180f;
    private float velocityXSmoothing;
    private int direction = 1;
    private Vector3 velocity;
    private Controller2D controller;
    private Animator animator;
    private Transform killTriggerObject;
    private Transform rotYTransform;
    private Transform rotZTransform;
    private Transform playerTranform;

    private const float ROTATION_RIGHT = 90f;
    private const float ROTATION_IDLE = 180f;
    private const float ROTATION_LEFT = 270f;

    [Header("Charger Settings")]
    [SerializeField] private float chargerSurpriseDelay = 1f;
    [SerializeField] private float chargerMultiplier = 3f;
    private float surpriseTimer;
    private int chargeDirection = 1;

    [Header("Charger Settings")]
    [SerializeField] private float timeToBounceApex = 0.35f;
    [SerializeField] private float bounceHeight = 2.5f;
    private float bounceVelocity;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public float TravelDistance
    {
        set
        {
            this.travelDisctance = value;
            this.startPosition = new Vector3(this.transform.position.x - (this.travelDisctance / 2f), this.transform.position.y, this.transform.position.z);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.controller = GetComponent<Controller2D>();

        this.killTriggerObject = this.transform.Find("KillCollider");
        this.rotYTransform = this.transform.Find("RotationY");
        this.rotZTransform = this.rotYTransform.Find("RotationZ");
        this.animator = this.rotZTransform.Find("QMon").GetComponent<Animator>();

        this.playerTranform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        this.surpriseTimer = this.chargerSurpriseDelay;
        this.startPosition = new Vector3(this.transform.position.x - (this.travelDisctance / 2f), this.transform.position.y, this.transform.position.z);     
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (Time.deltaTime > Mathf.Epsilon)
        {
            Vector2 input = Behaviour();

            MoveH(input);
            MoveV(input, this.type == EnemyType.Jumper);

            RotationH();
            Animate(input);

            this.controller.Move(this.velocity * Time.deltaTime, gravityDirection);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            this.gameObject.SetActive(false);
            ParticleManager.instance.PlayParticleSystem(this.deathParticles, this.transform.position);
            SoundManager.instance.PlaySound(this.deathSound, 0.5f);
            LevelManager.instance.KillCount++;
            Destroy(this.gameObject);
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
    private void MoveV(Vector2 input, bool isJumper)
    {
        // If there is a collision in Y axis, reset velocity
        if (this.controller.collisions.above || this.controller.collisions.below)
        {
            this.velocity.y = 0;
        }


        if (input.y != 0 && isJumper)
        {
            this.gravity = -(2f * this.bounceHeight) / Mathf.Pow(this.timeToBounceApex, 2f);
            this.bounceVelocity = Mathf.Abs(this.gravity) * 0.35f;
            this.velocity.y = this.bounceVelocity * this.gravityDirection;
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
    Vector2 Behaviour()
    {
        int dir = this.playerTranform.position.x - this.transform.position.x > 0 ? 1 : -1;
        float distance = DistanceBetween(this.transform.position, this.playerTranform.position);

        switch (this.type)
        {
            //Only difference between the movers is the default moveSpeed.
            case EnemyType.SlowMover:
            case EnemyType.FastMover:

                return Wander();

            case EnemyType.FastFollow:

                return Follow(distance, dir);

            case EnemyType.Charger:

                return Charge(distance, dir);

            case EnemyType.Jumper:

                return Jumper();

            default:

                return Wander();
        }    
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    Vector2 Wander()
    {
        if (this.controller.collisions.left && this.direction == -1)
        {
            this.startPosition = this.transform.position;
            this.direction = 1;
        }
        else if (this.controller.collisions.right && this.direction == 1)
        {
            this.startPosition = this.transform.position;
            this.direction = -1;
        }
        else if (Mathf.Abs(this.startPosition.x - this.transform.position.x) > this.travelDisctance)
        {
            this.startPosition = this.transform.position;
            this.direction *= -1;
        }
        return new Vector2(this.direction, 0);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    Vector2 Follow(float distance, int direction)
    {
        if (distance <= this.detectionRadius)
        {
            RaycastHit hit;
            if (Physics.Linecast(this.gameObject.GetComponent<BoxCollider>().bounds.center, this.playerTranform.gameObject.GetComponent<BoxCollider>().bounds.center, out hit, this.layerMask))
            {
                return Wander();
            }
            return new Vector2(direction * 3, 0);
        }
        return Wander();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    Vector2 Charge(float distance, int direction)
    {
        if (this.controller.collisions.left || this.controller.collisions.right)
        {
            this.transform.Find("Exclamation").GetComponent<TextMesh>().text = "";
            this.surpriseTimer = this.chargerSurpriseDelay;
            return Wander();
        }
        else if (distance <= this.detectionRadius && distance > 1 && this.surpriseTimer == this.chargerSurpriseDelay)
        {
            RaycastHit hit;
            if (Physics.Linecast(this.gameObject.GetComponent<BoxCollider>().bounds.center, this.playerTranform.gameObject.GetComponent<BoxCollider>().bounds.center, out hit, this.layerMask))
            {
                return Wander();
            }

            this.chargeDirection = direction;
            this.surpriseTimer -= Time.deltaTime;
            this.transform.Find("Exclamation").GetComponent<TextMesh>().text = "!";
            return new Vector2(this.chargeDirection, 0);
        }
        else if (this.surpriseTimer < this.chargerSurpriseDelay && this.surpriseTimer > 0)
        {
            this.surpriseTimer -= Time.deltaTime;
            if (this.surpriseTimer <= 0)
            {
                return new Vector2(this.chargeDirection * chargerMultiplier, 0);
            }
            return new Vector2(0, 0);
        }
        else if (this.surpriseTimer <= 0)
        {
            return new Vector2(this.chargeDirection * chargerMultiplier, 0);
        }

        return Wander();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    Vector2 Jumper()
    {
        //Horizontal direction
        if (this.controller.collisions.left && this.direction == -1)
        {
            this.startPosition = this.transform.position;
            this.direction = 1;
        }
        else if (this.controller.collisions.right && this.direction == 1)
        {
            this.startPosition = this.transform.position;
            this.direction = -1;
        }
        else if (Mathf.Abs(this.startPosition.x - this.transform.position.x) > this.travelDisctance)
        {
            this.startPosition = this.transform.position;
            this.direction *= -1;
        }

        //Vertical direction
        if (this.controller.collisions.below)
        {
            return new Vector2(this.direction, this.gravityDirection);
        }

        return new Vector2(this.direction, 0);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    float DistanceBetween(Vector2 p1, Vector2 p2)
    {
        return Mathf.Sqrt(Mathf.Pow(Mathf.Abs(p2.x - p1.x), 2) + Mathf.Pow(Mathf.Abs(p2.y - p1.y), 2));
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void ReverseGravity()
    {
        this.gravityDirection *= -1;
        this.transform.localPosition -= new Vector3(0f, this.GetComponent<BoxCollider>().size.y, 0f);

        this.rotZTransform.localEulerAngles = new Vector3(0f, 0f, 180f);
        this.rotZTransform.transform.localPosition = new Vector3(0f, this.GetComponent<BoxCollider>().size.y, 0f);

        this.killTriggerObject.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
        this.killTriggerObject.transform.localPosition = new Vector3(0f, this.GetComponent<BoxCollider>().size.y, 0f);
    }
}