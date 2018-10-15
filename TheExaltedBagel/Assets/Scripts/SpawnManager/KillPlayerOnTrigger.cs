using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayerOnTrigger : MonoBehaviour {

    private CheckpointManager gm;

    // Use this for initialization
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("CM").GetComponent<CheckpointManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gm.spawnPlayerToCheckpoint = true;
        }
    }
}
