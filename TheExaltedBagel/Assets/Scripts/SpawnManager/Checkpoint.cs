using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    [SerializeField] private float gravityDirection = 1f;
    [SerializeField] private float offsetSpawnPosY = -1f;

    private CheckpointManager gm;
    private bool triggered = false;


    // Use this for initialization
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("CM").GetComponent<CheckpointManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!triggered && collision.CompareTag("Player"))
        {
            //temp: Change color when triggered
            Renderer rend = GetComponent<Renderer>();
            rend.material.shader = Shader.Find("_Color");
            rend.material.SetColor("_Color", Color.blue);

            // Change last checkpoint position if the checkpoint is further than the last recorded checkpoint
            int index = gm.listCheckpoints.IndexOf(this.gameObject);
            if(index > gm.currentIndexLastCheckpoint)
            {
                gm.lastCheckpointPos = transform.position;
                gm.lastCheckpointPos.y += offsetSpawnPosY;
                gm.currentIndexLastCheckpoint = index;
                gm.gravityDirectionLastCheckpoint = gravityDirection;
            }
            triggered = true;
        }
    }
}
