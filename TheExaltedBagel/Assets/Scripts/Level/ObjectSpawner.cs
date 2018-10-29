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
    public void SpawnReset(bool isFirstSpawn)
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
            this.objectInstance.transform.localPosition = Vector3.zero;
            this.objectInstance.transform.localEulerAngles = Vector3.zero;

            this.objectInstance.transform.localScale = Vector3.Scale(
                this.objectInstance.transform.localScale, 
                new Vector3(1f / this.transform.lossyScale.x, 1f / this.transform.lossyScale.y, 1f / this.transform.lossyScale.z));

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
