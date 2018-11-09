using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalSpawner : Spawner
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    protected override void DecrementCounter()
    {
        LevelManager.instance.CrystalCount--;
    }
}
