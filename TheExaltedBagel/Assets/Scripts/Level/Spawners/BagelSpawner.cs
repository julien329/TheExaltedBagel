using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagelSpawner : Spawner
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    public override void SpawnReset(bool isFirstSpawn)
    {
        base.SpawnReset(isFirstSpawn);

        if (this.objectInstance != null)
        {
            this.objectInstance.transform.localEulerAngles = new Vector3(-35f, 0f, 0f);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    protected override void DecrementCounter()
    {
        LevelManager.instance.RemoveBagel();
    }
}
