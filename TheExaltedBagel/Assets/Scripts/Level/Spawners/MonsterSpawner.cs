using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : Spawner
{ 
    enum Gravity { FLOOR, CEILING }

    [SerializeField] private Gravity gravDirection = Gravity.FLOOR;
    [SerializeField] private float travelDistance = 3f;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public override void SpawnReset(bool isFirstSpawn)
    {
        base.SpawnReset(isFirstSpawn);

        if (this.objectInstance != null)
        {
            MonsterAI ai = this.objectInstance.GetComponent<MonsterAI>();
            ai.TravelDistance = this.travelDistance;
            if (this.gravDirection == Gravity.CEILING)
            {
                ai.ReverseGravity();
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    protected override void DecrementCounter()
    {
        LevelManager.instance.KillCount--;
    }
}
