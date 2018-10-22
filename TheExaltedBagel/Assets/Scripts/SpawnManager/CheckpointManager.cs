using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> listCheckpoints;

    private static CheckpointManager instance;

    public Vector3 firstSpawnPos;

    [NonSerialized] public float gravityDirectionLastCheckpoint = 1f;
    [NonSerialized] public int currentIndexLastCheckpoint = -1;
    [NonSerialized] public Vector3 lastCheckpointPos;

    private Player player;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this.gameObject);
        }

        this.lastCheckpointPos = this.firstSpawnPos;
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RespawnPlayerLastCheckpoint();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void RespawnPlayerLastCheckpoint()
    {
        this.player.RespawnPlayer(this.lastCheckpointPos, this.gravityDirectionLastCheckpoint);
    }
}
