using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{ 
    [SerializeField] private GameObject spawningObject;
    [SerializeField] private float bonusScore;

    private GameObject objectInstance;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        Destroy(GetComponent<MeshRenderer>());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnObject(bool isFirstSpawn)
    {
        if (this.spawningObject != null)
        {
            if (this.objectInstance != null)
            {
                Destroy(this.objectInstance);
            }

            if (!isFirstSpawn)
            {
                LevelManager.instance.Score -= this.bonusScore;
            }

            this.objectInstance = Instantiate(this.spawningObject, this.transform);

            OnDestroyListener listener = this.objectInstance.AddComponent<OnDestroyListener>();
            listener.onDestroyDelegate = OnObjectDestroyed;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnObjectDestroyed()
    {
        LevelManager.instance.Score += this.bonusScore;
    }
}
