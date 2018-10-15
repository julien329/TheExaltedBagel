using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckpointManager : MonoBehaviour {

    [SerializeField] public List<GameObject> listCheckpoints;

    private static CheckpointManager instance;

    public Vector3 firstSpawnPos;

    [NonSerialized] public float gravityDirectionLastCheckpoint = 1f;
    [NonSerialized] public int currentIndexLastCheckpoint = -1;
    [NonSerialized] public Vector3 lastCheckpointPos;
    [NonSerialized] public bool spawnPlayerToCheckpoint = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
        lastCheckpointPos = firstSpawnPos;
    }
}
