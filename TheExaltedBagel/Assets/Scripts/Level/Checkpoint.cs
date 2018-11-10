using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool isStart;
    [SerializeField] private bool isEnding;
    [SerializeField] private float gravityDirection = 1f;
    [SerializeField] private GameObject spawnParticles;
    [SerializeField] private AudioClip saveSound;

    private Animator animator;
    private bool triggered = false;
    private float savedTimer;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.animator = GetComponent<Animator>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        if (this.isStart)
        {
            TriggerCheckpoint();
            LevelManager.instance.KillPlayer(true);
        }
        else
        {
            BroadcastMessage("SpawnReset", true);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (!this.triggered && collision.CompareTag("Player"))
        {
            TriggerCheckpoint();
            SoundManager.instance.PlaySound(this.saveSound);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void TriggerCheckpoint()
    {
        this.animator.SetTrigger("RaiseFlag");
        this.triggered = true;
        this.savedTimer = (this.isStart) ? LevelManager.instance.LevelTotalTime : LevelManager.instance.LevelTimer;

        LevelManager.instance.CurrentCheckpoint = this;

        if (this.isEnding)
        {
            LevelManager.instance.EndLevel();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void ResetSection(Player player, bool isFirstSpawn)
    {
        LevelManager.instance.LevelTimer = savedTimer;
        player.SpawnPlayer(this.transform.position, this.gravityDirection);
        BroadcastMessage("SpawnReset", isFirstSpawn);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnReset(bool isFirstSpawn)
    {
        if (!isFirstSpawn || this.isStart)
        {
            Vector3 eulerAngles = (this.gravityDirection == 1) ? new Vector3(270f, 0f, 0f) : new Vector3(90f, 0f, 0f);
            ParticleManager.instance.PlayParticleSystem(this.spawnParticles, this.transform.position, eulerAngles);
        }
    }
}
