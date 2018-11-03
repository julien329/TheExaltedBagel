﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateCoin : MonoBehaviour
{
    [SerializeField] private GameObject gate;
    [SerializeField] private GameObject[] coins;
    [SerializeField] private AudioClip openGateClip;
    [SerializeField] private AudioClip crystalSound;
    [SerializeField] private GameObject coinPickUpEffect;

    private uint collectedCoinsCount;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        foreach (GameObject coin in this.coins)
        {
            OnTriggerEnterListener listener = coin.AddComponent<OnTriggerEnterListener>();
            listener.onTriggerEnterDelegate = CoinCollected;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnReset(bool isFirstSpawn)
    {
        this.gate.SetActive(true);

        this.collectedCoinsCount = 0;
        foreach (GameObject coin in this.coins)
        {
            coin.SetActive(true);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void CoinCollected(GameObject coin)
    {
        this.collectedCoinsCount++;
        coin.SetActive(false);

        Instantiate(this.coinPickUpEffect, coin.GetComponent<BoxCollider>().bounds.center, Quaternion.identity);
        SoundManager.instance.PlaySound(this.crystalSound);

        if (this.collectedCoinsCount == this.coins.Length)
        {
            this.gate.SetActive(false);
            SoundManager.instance.PlaySound(openGateClip);
        }
    }
}
