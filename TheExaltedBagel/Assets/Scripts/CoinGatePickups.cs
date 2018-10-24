using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGatePickups : MonoBehaviour {

    [SerializeField] private GameObject PickUpEffect;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            Instantiate(PickUpEffect, new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z), Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}
