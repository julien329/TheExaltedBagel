using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGate : MonoBehaviour {

    private int totalCoinCount = 0;
    private GameObject gate;
    private AudioSource openGateSound;
    private bool gateOpened = false;

    private void Awake()
    {
        this.gate = this.transform.Find("Gate").gameObject;
        this.openGateSound = this.GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start ()
    {
        foreach (Transform t in transform)
        {
            if (t.name == "Coin")
            {
                totalCoinCount++;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.gateOpened)
            return;

        bool openGate = true;
        foreach (Transform t in this.transform)
        {
            if (t.name == "Coin")
            {
                if (t.GetComponent<BoxCollider>().enabled)
                {
                    openGate = false;
                    break;
                }
            }
        }

        if (openGate)
        {
            this.openGateSound.Play();
            foreach (Transform t in this.gate.transform)
            {
                t.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            this.gate.GetComponent<BoxCollider>().enabled = false;
            gateOpened = true;
        }
    }

    public void SpawnReset(bool isFirstSpawn)
    {
        //Reset coins first
        BroadcastMessage("ResetCoin");

        //Reset gate after
        foreach (Transform t in this.gate.transform)
        {
            t.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        gate.GetComponent<BoxCollider>().enabled = true;
        gateOpened = false;
    }
}
