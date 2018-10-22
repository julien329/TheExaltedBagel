using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {
    
    [SerializeField] private GameObject MonsterInstance;

    // Use this for initialization
    void Start () {
        MonsterInstance = this.transform.parent.parent.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            Destroy(MonsterInstance);
        }
    }
}
