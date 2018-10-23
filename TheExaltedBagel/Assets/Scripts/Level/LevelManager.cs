using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private float deathScorePenalty = 1000f;

    private float score = 0f;
    private Player player;
    private Checkpoint currentCheckpoint;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public Checkpoint CurrentCheckpoint
    {
        set { this.currentCheckpoint = value; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public float Score
    {
        get { return this.score; }
        set { this.score = value; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }

        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            KillPlayer(false);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void KillPlayer(bool isFirstSpawn)
    {
        if (!isFirstSpawn)
        {
            this.score -= this.deathScorePenalty;
        }

        this.currentCheckpoint.ResetSection(this.player, isFirstSpawn);
    }
}
