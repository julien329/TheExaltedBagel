using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {
    
    [SerializeField] private GameObject MonsterInstance;
    [SerializeField] private GameObject PoofPrefab;

    // Use this for initialization
    void Start () {
        MonsterInstance = this.transform.parent.parent.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            Instantiate(PoofPrefab, new Vector3(MonsterInstance.transform.position.x, MonsterInstance.transform.position.y + 0.5f, MonsterInstance.transform.position.z), Quaternion.identity);
            Destroy(MonsterInstance);
        }
    }
}
