using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour {

    [SerializeField] private GameObject hitTrigger;
    [SerializeField] private GameObject deathParticles;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fireball;
    [SerializeField] private GameObject firstGate;
    [SerializeField] private GameObject lastGate;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private Collider[] bodyParts;
    [SerializeField] private AudioClip dyingSound;
    [SerializeField] private AudioClip bitingSound;
    [SerializeField] private float rotationSpeed = 1000f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float flyAwaySpeed = 10f;
    [SerializeField] private float biteDelay = 10f;
    [SerializeField] private float fireballDelay = 10f;
    [SerializeField] private int maxHealth = 4;
    [SerializeField] private int health;

    private Transform rotYTransform;
    private Animator animator;
    [SerializeField] private Vector3 target;
    private float rotationHTarget = 180f;
    private bool isFlying = false;
    private bool canAttack = true;
    private int stage;

    private const float ROTATION_RIGHT = 0f;
    private const float ROTATION_LEFT = 180f;
    private const float ROTATION_BACK1 = 90f;
    private const float ROTATION_BACK2 = 270f;

    private void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        OnTriggerEnterListener listener = hitTrigger.AddComponent<OnTriggerEnterListener>();
        listener.onTriggerEnterDelegate = OnHit;
        //this.target = player.transform.position;
        this.rotYTransform = this.transform.Find("RotationY");

        bodyParts = this.gameObject.GetComponentsInChildren<Collider>();
    }

    // Use this for initialization
    void Start ()
    {
        this.health = this.maxHealth;
        this.rotationHTarget = ROTATION_LEFT;
        this.stage = 1;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.stage == 1 && this.isFlying)
        {
            Stage1Behaviour();
            if (!this.player.activeInHierarchy)
                this.health = maxHealth;
        }
        else if (this.stage == 2)
        {
            ToggleColliders(false);
            FlyAway();
        }
        else if (this.stage == 3)
        {
            if (this.player.transform.position.x > 50f)
                this.stage = 4;
        }
        else if (this.stage == 4)
        {
            Stage4Behaviour();
        }
        else if (this.stage == 5)
        {
            Stage5Behaviour();
            if (!this.player.activeInHierarchy)
                this.health = 2;
        }

    }

    public void OnHit(GameObject head)
    {
        if (this.isFlying)
            this.animator.SetTrigger("Fly Take Damage");

        this.health--;

        //Temporary immunity to avoid double hits
        ToggleColliders(false);
        StartCoroutine(ImmunityDelay(2f));

        if (this.health == 2)
        {
            this.firstGate.SetActive(false);
            this.stage = 2;
        }
        else if (this.health == 0)
        {
            this.lastGate.SetActive(false);
        }
    }

    private void Die()
    {
        ParticleManager.instance.PlayParticleSystem(deathParticles, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z));
        Destroy(this.gameObject);
    }

    public void ToggleFlying()
    {
        this.isFlying = !this.isFlying;
    }

    private void RotationH()
    {
        if ((this.rotationHTarget == ROTATION_RIGHT && this.rotYTransform.localEulerAngles.y > ROTATION_RIGHT)
            || (this.rotationHTarget == ROTATION_LEFT && this.rotYTransform.localEulerAngles.y < ROTATION_LEFT))
        {
            float direction = (this.rotationHTarget == ROTATION_RIGHT) ? -1f : 1f;
            float rotateAngle = direction * Time.deltaTime * rotationSpeed;

            float newRot = this.rotYTransform.localEulerAngles.y + rotateAngle;
            if (newRot < ROTATION_RIGHT || newRot > ROTATION_LEFT)
            {
                this.rotYTransform.localEulerAngles = new Vector3(this.rotYTransform.localEulerAngles.x, this.rotationHTarget, this.rotYTransform.localEulerAngles.z);
            }
            else
            {
                this.rotYTransform.Rotate(this.rotYTransform.up, rotateAngle, Space.World);
            }
        }
    }

    private void Stage1Behaviour()
    {
        if (this.player.transform.position.x < 2.5)
            this.target = new Vector3(15f, 6f, 0f);
        else
            this.target = player.transform.position;

        if (new Vector3(this.target.x - this.transform.position.x, this.target.y - this.transform.position.y, this.target.z - this.transform.position.z).magnitude < 1f)
            return;

        if (Mathf.Abs(this.transform.position.y - this.target.y) > 1)
        {
            if (this.transform.position.y - this.target.y < -1 && this.transform.position.y < 13)
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + this.speed * Time.deltaTime, this.transform.position.z);
            else if (this.transform.position.y - this.target.y > -1)
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - this.speed * Time.deltaTime, this.transform.position.z);
        }

        if (Mathf.Abs(this.transform.position.x - this.target.x) > 1)
        {
            if (this.transform.position.x - this.target.x < 0)
            {
                this.transform.position = new Vector3(this.transform.position.x + this.speed * Time.deltaTime, this.transform.position.y, this.transform.position.z);
                this.rotationHTarget = ROTATION_RIGHT;
            }
            else if (this.transform.position.x - this.target.x > 0 && this.transform.position.x > 8)
            {
                this.transform.position = new Vector3(this.transform.position.x - this.speed * Time.deltaTime, this.transform.position.y, this.transform.position.z);
                this.rotationHTarget = ROTATION_LEFT;
            }
        }

        if (new Vector3(this.player.transform.position.x - this.transform.position.x, this.player.transform.position.y - this.transform.position.y, this.player.transform.position.z - this.transform.position.z).magnitude < 5 && this.canAttack)
        {
            this.animator.SetTrigger("Fly Bite Attack");
            SoundManager.instance.PlaySound(this.bitingSound, 1);
            this.canAttack = false;
            StartCoroutine(AttackDelay(this.biteDelay));
        }

        RotationH();
    }

    private void FlyAway()
    {
        this.transform.position = new Vector3(this.transform.position.x + this.flyAwaySpeed * Time.deltaTime, this.transform.position.y + this.flyAwaySpeed * Time.deltaTime, this.transform.position.z + this.flyAwaySpeed * Time.deltaTime);
        this.rotationHTarget = ROTATION_RIGHT;
        RotationH();

        if (this.transform.position.x > 40)
        {
            this.stage = 3;
            this.transform.position = new Vector3(68f, 6f, 18f);
            this.rotYTransform.localEulerAngles = new Vector3(30f, 90f, 0f);
        }
    }

    private void Stage4Behaviour()
    {
        if (this.player.transform.position.x > 103f)
        {
            this.transform.position = new Vector3(139f, 4f, 0f);
            this.rotYTransform.localEulerAngles = new Vector3(0f, 180f, 0f);
            ToggleColliders(true);
            this.stage = 5;
        }
        else if (this.player.transform.position.x > 50f && this.player.transform.position.x < 75f && this.canAttack)
        {
            this.animator.SetTrigger("Fly Projectile Attack");
            StartCoroutine(FireBallAnimDelay(0.65f));
            this.canAttack = false;
            StartCoroutine(AttackDelay(this.fireballDelay));
        }
    }

    private void Stage5Behaviour()
    {
        this.target = this.player.transform.position;

        // Check if dragon is dead
        if (this.health <= 0)
        {
            if (this.transform.position.y > 0)
            {
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 10f * Time.deltaTime, this.transform.position.z);
            }
            if (this.health == 0)
            {
                this.animator.SetTrigger("Fly Die");
                StartCoroutine(WaitForTimer());
                this.health--;
            }
            return;
        }

        //Don't move if the player is too close to avoid jitter
        if (new Vector3(this.target.x - this.transform.position.x, this.target.y - this.transform.position.y, this.target.z - this.transform.position.z).magnitude < 1f)
            return;

        if (Mathf.Abs(this.transform.position.x - this.target.x) > 1)
        {
            if (this.transform.position.x - this.target.x < 0)
            {
                this.rotationHTarget = ROTATION_RIGHT;
            }
            else if (this.transform.position.x - this.target.x > 0)
            {
                this.rotationHTarget = ROTATION_LEFT;
            }
        }

        if (new Vector3(this.player.transform.position.x - this.transform.position.x, this.player.transform.position.y - this.transform.position.y, this.player.transform.position.z - this.transform.position.z).magnitude < 5 && this.canAttack)
        {
            this.animator.SetTrigger("Fly Bite Attack");
            SoundManager.instance.PlaySound(this.bitingSound, 1);
            this.canAttack = false;
            StartCoroutine(AttackDelay(this.biteDelay * 0.5f));
        }

        RotationH();
    }

    //private void Stage5Behaviour()
    //{
    //    // Check if dragon is dead
    //    if (this.health <= 0)
    //    {
    //        if (this.transform.position.y > 0)
    //        {
    //            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 10f * Time.deltaTime, this.transform.position.z);
    //        }
    //        if (this.health == 0)
    //        {
    //            this.animator.SetTrigger("Fly Die");
    //            StartCoroutine(WaitForTimer());
    //            this.health--;
    //        }
    //        return;
    //    }

    //    //Change target if player ran away
    //    if (this.player.transform.position.x < 125f)
    //        this.target = new Vector3(139f, 6f, 0f);
    //    else
    //        this.target = player.transform.position;

    //    //Don't move if the player is too close to avoid jitter
    //    if (new Vector3(this.target.x - this.transform.position.x, this.target.y - this.transform.position.y, this.target.z - this.transform.position.z).magnitude < 1f)
    //        return;


    //    if (Mathf.Abs(this.transform.position.y - this.target.y) > 1)
    //    {
    //        if (this.transform.position.y - this.target.y < -1 && this.transform.position.y < 13)
    //            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + this.speed * Time.deltaTime * 1.5f, this.transform.position.z);
    //        else if (this.transform.position.y - this.target.y > -1)
    //            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - this.speed * Time.deltaTime * 1.5f, this.transform.position.z);
    //    }

    //    if (Mathf.Abs(this.transform.position.x - this.target.x) > 1)
    //    {
    //        if (this.transform.position.x - this.target.x < 0)
    //        {
    //            this.transform.position = new Vector3(this.transform.position.x + this.speed * Time.deltaTime * 1.5f, this.transform.position.y, this.transform.position.z);
    //            this.rotationHTarget = ROTATION_RIGHT;
    //        }
    //        else if (this.transform.position.x - this.target.x > 0)
    //        {
    //            this.transform.position = new Vector3(this.transform.position.x - this.speed * Time.deltaTime * 1.5f, this.transform.position.y, this.transform.position.z);
    //            this.rotationHTarget = ROTATION_LEFT;
    //        }
    //    }

    //    if (new Vector3(this.player.transform.position.x - this.transform.position.x, this.player.transform.position.y - this.transform.position.y, this.player.transform.position.z - this.transform.position.z).magnitude < 5 && this.canAttack)
    //    {
    //        this.animator.SetTrigger("Fly Bite Attack");
    //        SoundManager.instance.PlaySound(this.bitingSound, 1);
    //        this.canAttack = false;
    //        StartCoroutine(AttackDelay(this.biteDelay * 0.5f));
    //    }

    //    RotationH();
    //}

    private void ToggleColliders(bool enable)
    {
        foreach (Collider collider in this.bodyParts)
            collider.enabled = enable;
    }

    private IEnumerator WaitForTimer()
    {
        yield return new WaitForSeconds(0.4f);
        SoundManager.instance.PlaySound(dyingSound, 1f);
        yield return new WaitForSeconds(3f);
        Die();
    }

    private IEnumerator AttackDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.canAttack = true;
    }

    private IEnumerator ImmunityDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleColliders(true);
    }

    private IEnumerator FireBallAnimDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(this.fireball, this.firingPoint.position, this.firingPoint.localRotation);
    }
}
