using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{ 
    enum SpawnerType { NONE, CRYSTAL, ENEMY }
    enum Gravity { FLOOR, CEILING }

    [SerializeField] private GameObject spawningObject;
    [SerializeField] private SpawnerType spawnerType = SpawnerType.NONE;
    [SerializeField] private Gravity gravDirection = Gravity.FLOOR;

    private GameObject objectInstance;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        Destroy(GetComponent<MeshRenderer>());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnReset(bool isFirstSpawn)
    {
        if (this.spawningObject != null)
        {
            DestroySpawned(isFirstSpawn);

            this.objectInstance = Instantiate(this.spawningObject, this.transform);
            this.objectInstance.transform.localPosition = Vector3.zero;
            this.objectInstance.transform.localEulerAngles = Vector3.zero;

            if (this.spawnerType == SpawnerType.ENEMY)
            {
                MonsterAI ai = this.objectInstance.GetComponent<MonsterAI>();
                ai.gravityDirection = (this.gravDirection == Gravity.FLOOR) ? 1 : -1;
            }

            this.objectInstance.transform.localScale = Vector3.Scale(
                this.objectInstance.transform.localScale, 
                new Vector3(1f / this.transform.lossyScale.x, 1f / this.transform.lossyScale.y, 1f / this.transform.lossyScale.z));
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void DestroySpawned(bool isFirstSpawn)
    {
        if (this.objectInstance != null)
        {
            Destroy(this.objectInstance);
        }
        else if (!isFirstSpawn)
        {
            switch (this.spawnerType)
            {
                case SpawnerType.CRYSTAL:
                    LevelManager.instance.CrystalCount--;
                    break;
                case SpawnerType.ENEMY:
                    LevelManager.instance.KillCount--;
                    break;
                case SpawnerType.NONE:
                    print("Warning: Spawner type is not set!");
                    break;
            }
        }
    }
}
