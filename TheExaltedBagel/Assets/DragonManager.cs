using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour {

    [SerializeField] private GameObject hitTrigger;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int health;
    [SerializeField] private AudioClip dyingSound;
    [SerializeField] private GameObject deathParticles;

    private bool canBeHit = true;
    private Animator animator;
    private bool isFlying = false;

    private void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        OnTriggerEnterListener listener = hitTrigger.AddComponent<OnTriggerEnterListener>();
        listener.onTriggerEnterDelegate = OnHit;
    }

    // Use this for initialization
    void Start ()
    {
        health = maxHealth;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            this.animator.SetTrigger("Bite Attack");
            this.animator.SetTrigger("Fly Bite Attack");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.animator.SetTrigger("Fly Idle");
            isFlying = !isFlying;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.animator.SetTrigger("Fly Take Damage");

        }
    }

    public void OnHit(GameObject head)
    {
        if (!this.canBeHit)
            return;

        if (this.isFlying)
            this.animator.SetTrigger("Fly Take Damage");
        else
            this.animator.SetTrigger("Take Damage");

        health--;

        if (health == 0)
        {
            if (this.isFlying)
            {
                this.animator.SetTrigger("Fly Die");
            }
            else
            {
                this.animator.SetTrigger("Die");
            }

            StartCoroutine(WaitForTimer());
            
            health = maxHealth;
        }
    }

    private IEnumerator WaitForTimer()
    {
        yield return new WaitForSeconds(0.4f);
        SoundManager.instance.PlaySound(dyingSound, 0.5f);
        yield return new WaitForSeconds(3f);
        Die();
    }

    private void Die()
    {
        ParticleManager.instance.PlayParticleSystem(deathParticles, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z));
        Destroy(this.gameObject);
    }
}
