using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

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
    [SerializeField] private float timeToJumpApex = 0.35f;
    [SerializeField] private float timeToJumpApexWater = 0.75f;
    [SerializeField] private float jumpHeight = 2.5f;
    [SerializeField] private float jumpHeightWater = 3.5f;
    [SerializeField] private float bumpForce = 10f;

    [Header("Anim")]
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] private float timeToIdle = 4f;
    [SerializeField] private float minAnimSpeedRatio = 0.5f;
    [SerializeField] private GameObject splashParticles;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private GameObject scoreCrytalParticles;
    [SerializeField] private GameObject bagelParticles;
    [SerializeField] private GameObject killParticles;

    [Header("UI")]
    [SerializeField] private Image oxygenBar;
    [SerializeField] private Transform oxygenBackground;
    [SerializeField] private Canvas oxygenCanvas;
    [SerializeField] private float oxygenBarYOffset = 1.75f;

    [Header("Sound")]
    [SerializeField] private AudioClip bubblesSound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip crystalSound;
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private AudioClip gravitySound;
    [SerializeField] private AudioClip killSound;
    [SerializeField] private AudioClip eatSound;

    [Header("Other")]
    [SerializeField][Range(0, GRAVITY_CHARGES_MAX)] private uint gravityChargeMax = GRAVITY_CHARGES_MAX;
    [SerializeField] private float oxygenDuration = 10.0f;

    private uint gravityChargeCount = 3;
    private float moveSpeed;
    private float gravityDirection = 1f; 
    private float rotationHTarget = 180f;
    private float rotationVTarget = 0f;
    private float idleTimer = -1f;
    private float gravity;
    private float jumpVelocity;
    private float velocityXSmoothing;
    private float timeLeftOxygen;
    private Camera mainCamera;
    private Vector3 velocity;
    private Controller2D controller;
    private BoxCollider boxCollider;
    private Animator animator;
    private Transform rotYTransform;
    private Transform rotZTransform;
    private List<Collider> waterColliders = new List<Collider>();
    private List<Collider> bubbleColliders = new List<Collider>();

    private const float ROTATION_RIGHT = 90f;
    private const float ROTATION_IDLE = 180f;
    private const float ROTATION_LEFT = 270f;
    private const float ROTATION_UP = 180f;
    private const float ROTATION_DOWN = 0f;
    private const uint GRAVITY_CHARGES_MAX = 3;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public uint GravityChargeCount
    {
        get { return this.gravityChargeCount; }
        set
        {
            this.gravityChargeCount = Math.Min(value, this.gravityChargeMax);
            LevelManager.instance.UpdateGravityChargeUI();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public uint GravityChargeMax
    {
        get { return this.gravityChargeMax; }
        set
        {
            uint newValue = Math.Min(value, Player.GRAVITY_CHARGES_MAX);
            this.gravityChargeCount = newValue;
            this.gravityChargeMax = newValue;
            LevelManager.instance.InitGravityChargeUI();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.controller = GetComponent<Controller2D>();
        this.boxCollider = GetComponent<BoxCollider>();

        this.rotYTransform = this.transform.Find("RotationY");
        this.rotZTransform = this.rotYTransform.Find("RotationZ");       
        this.animator = this.rotZTransform.Find("Q-Mon1").GetComponent<Animator>();
        this.mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start ()
    {
        this.controller.Initialize();

        ChangeEnvironment(false);
        LevelManager.instance.InitGravityChargeUI();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update ()
    {
        if (Time.deltaTime > Mathf.Epsilon)
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
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void LateUpdate()
    {
        WaterTimerToDeath();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter(Collider collider)
    {
        // WaterLayer
        if (collider.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            OnEnterWater(collider);
        }
        // DeathLayer
        else if (collider.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            LevelManager.instance.KillPlayer(false);
        }
        // KillMonster
        else if (collider.transform.tag == "KillTrigger")
        {
            OnKillMonster(collider);
        }
        // DragonHit
        else if (collider.transform.tag == "DragonHit")
        {
            OnHitDragon(collider);
        }
        // KilledByMonster
        else if (collider.transform.tag == "Enemy")
        {
            LevelManager.instance.KillPlayer();
        }
        // TouchBubble
        else if (collider.transform.tag == "Bubble")
        {
            this.bubbleColliders.Add(collider);
            SoundManager.instance.PlaySound(this.bubblesSound, 0.75f);
        }
        // TouchCrystal
        else if (collider.transform.tag == "Crystal")
        {
            OnTakeCrystal(collider);
        }
        // TouchBagel
        else if (collider.transform.tag == "Bagel")
        {
            OnTakeBagel(collider);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerExit(Collider collider)
    {
        // LeaveBubble
        if (collider.transform.tag == "Bubble")
        {
            this.bubbleColliders.Remove(collider);
        }
        // LeaveWater
        else if (collider.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            OnExitWater(collider);
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
        if (this.controller.Collisions.above || this.controller.Collisions.below)
        {
            this.velocity.y = 0;
        }

        if (this.controller.Collisions.below)
        {
            this.GravityChargeCount = this.GravityChargeMax;
        }

        // Invert gravity if key is pressed or when the player, we need to spawn the player upside down
        if (Input.GetButtonDown("Gravity") && this.GravityChargeCount > 0)
        {
            this.GravityChargeCount--;
            this.gravityDirection *= -1;
            this.rotationVTarget = (this.gravityDirection == 1) ? ROTATION_DOWN : ROTATION_UP;
            SoundManager.instance.PlaySound(this.gravitySound);
        }

        // If the jump key is pressed
        if (Input.GetButtonDown("Jump") && this.controller.Collisions.below)
        {
            this.velocity.y = this.jumpVelocity * this.gravityDirection;
        }

        // Add gravity force downward to Y velocity
        this.velocity.y += this.gravity * this.gravityDirection * Time.deltaTime;

        if (Mathf.Abs(this.velocity.y) > this.jumpVelocity)
        {
            this.velocity.y = Mathf.Sign(this.velocity.y) * this.jumpVelocity;
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
        if (this.controller.Collisions.below)
        {
            accelerationTime = (this.controller.Collisions.isSlippery) ? this.accTimeIce : this.accTimeGround;
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
            moveSpeedX = (this.controller.Collisions.isSlippery) ? this.moveSpeedIce : this.moveSpeedNormal;
        }

        return moveSpeedX;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnEnterWater(Collider collider)
    {
        // If not already in water
        if (this.waterColliders.Count == 0)
        {
            // Impact reduces current velocity according to difference in gravity
            float newGravity = -(2f * this.jumpHeightWater) / Mathf.Pow(this.timeToJumpApexWater, 2f);
            float impactForce = Mathf.Clamp(newGravity / this.gravity, 0f, 1f);
            this.velocity = new Vector3(this.velocity.x * impactForce, this.velocity.y * impactForce, 0f);

            // If we have a splash vfx
            Vector3 contactPos = new Vector3(this.transform.position.x, collider.bounds.max.y, 0f);
            // If contact pos is within the water bounds
            if (contactPos.x >= collider.bounds.min.x && contactPos.x <= collider.bounds.max.x)
            {
                // Add the spash object
                ParticleManager.instance.PlayParticleSystem(this.splashParticles, contactPos);
            }

            SoundManager.instance.PlaySound(this.splashSound, 0.25f);

            // Change environment to water settings
            ChangeEnvironment(true);

            // Show UI Oxygen Bar and Reset Timer
            this.timeLeftOxygen = this.oxygenDuration;
            this.oxygenCanvas.enabled = true;
        }

        // Memorize the water collider
        this.waterColliders.Add(collider);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnExitWater(Collider collider)
    {
        // Remove saved collider
        this.waterColliders.Remove(collider);
        // If we are no in any water
        if (this.waterColliders.Count == 0)
        {
            // Change environment to air settings
            ChangeEnvironment(false);

            // Hide UI Oxygen Bar
            this.oxygenCanvas.enabled = false;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnEnterTeleport(Vector2 newPosition)
    {
        this.transform.position = new Vector3(newPosition.x, newPosition.y, 0f);
        this.velocity = Vector3.zero;
        this.velocityXSmoothing = 0f;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnHitDragon(Collider collider)
    {
        this.velocity.y = this.bumpForce * this.gravityDirection;
        this.GravityChargeCount++;
        
        SoundManager.instance.PlaySound(this.killSound, 0.25f);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnKillMonster(Collider collider)
    {
        this.velocity.y = this.bumpForce * collider.GetComponentInParent<MonsterAI>().GravityDirection;
        this.GravityChargeCount++;

        LevelManager.instance.KillCount++;
        SoundManager.instance.PlaySound(this.killSound, 0.25f);

        GameObject monster = collider.transform.parent.gameObject;
        Vector3 position = new Vector3(monster.transform.position.x, monster.transform.position.y + 0.5f, monster.transform.position.z);
        ParticleManager.instance.PlayParticleSystem(this.killParticles, position);

        monster.SetActive(false);
        Destroy(monster);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnTakeCrystal(Collider collider)
    {
        LevelManager.instance.CrystalCount++;

        ParticleManager.instance.PlayParticleSystem(this.scoreCrytalParticles, collider.bounds.center);
        SoundManager.instance.PlaySound(this.crystalSound, 0.5f);

        Destroy(collider.gameObject);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnTakeBagel(Collider collider)
    {
        this.GravityChargeMax++;

        ParticleManager.instance.PlayParticleSystem(this.bagelParticles, collider.bounds.center);
        SoundManager.instance.PlaySound(this.eatSound);

        Destroy(collider.gameObject);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void KillPlayer()
    {
        ParticleManager.instance.PlayParticleSystem(this.deathParticles, this.boxCollider.bounds.center);
        SoundManager.instance.PlaySound(this.deathSound);

        this.gameObject.SetActive(false);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnPlayer(Vector3 position, float gravityDirection)
    {
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }

        ChangeEnvironment(false);

        this.waterColliders.Clear();
        this.bubbleColliders.Clear();
        this.oxygenCanvas.enabled = false;

        this.GravityChargeCount = this.GravityChargeMax;
        this.gravityDirection = gravityDirection;
        this.transform.position = (gravityDirection == 1) ? position : position - new Vector3(0f, this.boxCollider.size.y, 0f);
        this.rotationVTarget = (gravityDirection == 1) ? ROTATION_DOWN : ROTATION_UP;
        this.rotZTransform.localEulerAngles = (gravityDirection == 1) ? new Vector3(0f, 0f, 0f) : new Vector3(0f, 0f, 180f);
        this.rotZTransform.localPosition = (gravityDirection == 1) ? new Vector3(0f, 0f, 0f) : new Vector3(0f, this.boxCollider.size.y, 0f);
        this.velocity = Vector3.zero;
        this.velocityXSmoothing = 0f;  
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void WaterTimerToDeath()
    {
        // Continue Timer if player in water
        if (this.waterColliders.Count > 0)
        {
            // Update Timer
            this.timeLeftOxygen = (this.bubbleColliders.Count > 0) ? this.oxygenDuration : this.timeLeftOxygen - Time.deltaTime;

            // Update UI
            this.oxygenBar.fillAmount = this.timeLeftOxygen / this.oxygenDuration;
            this.oxygenBackground.position = this.mainCamera.WorldToScreenPoint(this.transform.position + new Vector3(0f, this.oxygenBarYOffset, 0f));

            if (this.timeLeftOxygen < 0f)
            {
                // Kill player !
                LevelManager.instance.KillPlayer(false);
            }
        }
    }
}