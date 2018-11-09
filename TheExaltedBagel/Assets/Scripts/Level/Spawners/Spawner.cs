using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawningObject;

    protected GameObject objectInstance;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    protected void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public virtual void SpawnReset(bool isFirstSpawn)
    {
        if (this.spawningObject != null)
        {
            DestroySpawned(isFirstSpawn);

            this.objectInstance = Instantiate(this.spawningObject, this.transform);
            this.objectInstance.transform.localPosition = Vector3.zero;
            this.objectInstance.transform.localEulerAngles = Vector3.zero;
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
            this.objectInstance = null;
        }
        else if (!isFirstSpawn)
        {
            DecrementCounter();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    protected virtual void DecrementCounter()
    {
    }
}
