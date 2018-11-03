using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private GameObject otherPortal;
    [SerializeField] private GameObject teleportParticles;
    [SerializeField] private GameObject portalParticles;

    private bool portalDisabled;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        if (this.portalParticles != null)
        {
            GameObject particles = Instantiate(this.portalParticles, this.transform);
            particles.transform.localPosition = new Vector3(0.5f, 1.1025f, 0.5f);

            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
            particleSystem.Play();
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (!this.portalDisabled)
            {
                bool isExitUpsideDown = otherPortal.transform.rotation.z > 0f;
                this.PlayParticles(isExitUpsideDown);

                float offsetY = (isExitUpsideDown) ? this.otherPortal.transform.localScale.y - ((BoxCollider)collider).size.y : -this.otherPortal.transform.localScale.y;
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
    public void PlayParticles(bool isExitUpsideDown)
    {
        // If we have a teleport vfx
        if (this.teleportParticles != null)
        {
            // Add the spash object
            GameObject particles = Instantiate(this.teleportParticles);
            particles.transform.localPosition = otherPortal.transform.position;
            particles.transform.localEulerAngles = (isExitUpsideDown) ? new Vector3(90f, 0f, 0f) : new Vector3(270f, 0f, 0f);

            // Play the vfx
            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
            particleSystem.Play();
        }
    }
}
