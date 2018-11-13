using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateCrystal : MonoBehaviour
{
    [SerializeField] private GameObject gate;
    [SerializeField] private GameObject[] crystals;
    [SerializeField] private AudioClip openGateClip;
    [SerializeField] private AudioClip crystalSound;
    [SerializeField] private GameObject crystalPickUpEffect;

    private uint collectedCrystalsCount;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        foreach (GameObject crystal in this.crystals)
        {
            OnTriggerEnterListener listener = crystal.AddComponent<OnTriggerEnterListener>();
            listener.onTriggerEnterDelegate = CrystalCollected;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void SpawnReset(bool isFirstSpawn)
    {
        this.gate.SetActive(true);

        this.collectedCrystalsCount = 0;
        foreach (GameObject crystal in this.crystals)
        {
            crystal.SetActive(true);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void CrystalCollected(GameObject crystal)
    {
        this.collectedCrystalsCount++;
        crystal.SetActive(false);

        ParticleManager.instance.PlayParticleSystem(this.crystalPickUpEffect, crystal.GetComponent<MeshRenderer>().bounds.center);
        SoundManager.instance.PlaySound(this.crystalSound, 0.5f);

        if (this.collectedCrystalsCount == this.crystals.Length)
        {
            this.gate.SetActive(false);
            SoundManager.instance.PlaySound(openGateClip, 0.75f);
        }
    }
}
