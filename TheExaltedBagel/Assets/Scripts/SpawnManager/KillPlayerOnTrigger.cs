using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnTrigger : MonoBehaviour
{
    private CheckpointManager checkpointManager;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        this.checkpointManager = GameObject.FindGameObjectWithTag("CM").GetComponent<CheckpointManager>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            this.checkpointManager.RespawnPlayerLastCheckpoint();
        }
    }
}
