using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float gravityDirection = 1f;
    [SerializeField] private float offsetSpawnPosY = -1f;

    private Renderer rend;
    private CheckpointManager checkpointManager;
    private bool triggered = false;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.rend = GetComponent<Renderer>();
        this.checkpointManager = GameObject.FindGameObjectWithTag("CM").GetComponent<CheckpointManager>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!this.triggered && collision.CompareTag("Player"))
        {
            //temp: Change color when triggered
            this.rend.material.shader = Shader.Find("_Color");
            this.rend.material.SetColor("_Color", Color.blue);

            // Change last checkpoint position if the checkpoint is further than the last recorded checkpoint
            int index = this.checkpointManager.listCheckpoints.IndexOf(this.gameObject);
            if(index > this.checkpointManager.currentIndexLastCheckpoint)
            {
                this.checkpointManager.lastCheckpointPos = this.transform.position;
                this.checkpointManager.lastCheckpointPos.y += this.offsetSpawnPosY;
                this.checkpointManager.currentIndexLastCheckpoint = index;
                this.checkpointManager.gravityDirectionLastCheckpoint = this.gravityDirection;
            }

            this.triggered = true;
        }
    }
}
