﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private GameObject otherPortal;
    [SerializeField] private GameObject teleportParticles;
    [SerializeField] private GameObject poofParticles;
    [SerializeField] private GameObject portalParticles;
    [SerializeField] private AudioClip portalSound;

    private bool isUpsideDown;
    private bool portalDisabled;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        if (this.portalParticles != null)
        {
            GameObject particles = Instantiate(this.portalParticles, this.transform);
            particles.transform.localPosition = new Vector3(0.5f, 1.1025f, 0.5f);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        this.isUpsideDown = this.transform.rotation.z > 0f;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (!this.portalDisabled)
            {
                this.PlayParticles(collider);
                SoundManager.instance.PlaySound(this.portalSound);

                float offsetY = (this.otherPortal.GetComponentInParent<Teleportation>().isUpsideDown) ? 
                    this.otherPortal.transform.localScale.y - ((BoxCollider)collider).size.y : -this.otherPortal.transform.localScale.y;
                LevelManager.instance.TeleportPlayer(new Vector2(this.otherPortal.transform.position.x, this.otherPortal.transform.position.y + offsetY));

                this.otherPortal.GetComponentInParent<Teleportation>().portalDisabled = true;
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            this.portalDisabled = false;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void PlayParticles(Collider collider)
    {
        Vector3 position, eulerAngles;

        position = this.otherPortal.transform.position;
        eulerAngles = (this.otherPortal.GetComponentInParent<Teleportation>().isUpsideDown) ? new Vector3(90f, 0f, 0f) : new Vector3(270f, 0f, 0f);
        ParticleManager.instance.PlayParticleSystem(this.teleportParticles, position, eulerAngles);

        position = this.otherPortal.transform.parent.GetComponentInParent<Teleportation>().otherPortal.transform.position;
        eulerAngles = (this.isUpsideDown) ? new Vector3(270f, 0f, 0f) : new Vector3(90f, 0f, 0f);
        ParticleManager.instance.PlayParticleSystem(this.poofParticles, position, eulerAngles);
    }
}
