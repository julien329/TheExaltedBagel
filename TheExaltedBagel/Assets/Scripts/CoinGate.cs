using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGate : MonoBehaviour {

    private int totalCoinCount = 0;

	// Use this for initialization
	void Start () {
        foreach (Transform t in transform)
        {
            if (t.name == "Coin")
            {
                totalCoinCount++;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        bool destroy = true;
        foreach (Transform t in this.transform)
        {
            if (t.name == "Coin")
            {
                destroy = false;
                break;
            }
        }

        if (destroy)
        {
            Destroy(this.gameObject);
        }
    }
}
