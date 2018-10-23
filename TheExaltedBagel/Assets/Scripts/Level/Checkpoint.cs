using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float gravityDirection = 1f;
    [SerializeField] private uint index = 0;

    private Renderer rend;
    private bool triggered = false;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.rend = GetComponent<Renderer>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        if (this.index == 0)
        {
            TriggerCheckpoint();
            LevelManager.instance.KillPlayer(true);
        }
        else
        {
            BroadcastMessage("SpawnObject", true);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnValidate()
    {
        if (this.gameObject.activeInHierarchy)
        {
            this.gameObject.name = "Checkpoint " + this.index;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (!this.triggered && collision.CompareTag("Player"))
        {
            TriggerCheckpoint();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void TriggerCheckpoint()
    {
        this.rend.material.color = Color.red;

        this.triggered = true;
        LevelManager.instance.CurrentCheckpoint = this;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void ResetSection(Player player, bool isFirstSpawn)
    {
        player.SpawnPlayer(this.transform.position, this.gravityDirection);
        BroadcastMessage("SpawnObject", isFirstSpawn);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnObject(bool isFirstSpawn)
    {
        // Broadcast Receiver
    }
}
