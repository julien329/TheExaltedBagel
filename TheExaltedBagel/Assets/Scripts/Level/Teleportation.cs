using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    [SerializeField] private GameObject otherPortal;
    [SerializeField] private GameObject teleportParticles;


    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!LevelManager.instance.IsPlayerAlreadyTeleport)
            {
                bool isNextPortalUpsideDown = false;
                float offsetY = 0.0f;
                if (otherPortal.transform.rotation.z > 0f)
                {
                    isNextPortalUpsideDown = true;
                    offsetY = -1.13f;
                }
                LevelManager.instance.TeleportPlayer(new Vector2(otherPortal.transform.position.x, otherPortal.transform.position.y + offsetY));
                this.playAnimation(isNextPortalUpsideDown);
                LevelManager.instance.IsPlayerAlreadyTeleport = true;
            }
            else
            {
                // To not make a infinite teleport loop
                LevelManager.instance.IsPlayerAlreadyTeleport = false;
            }
        }
    }

    public void playAnimation(bool isNextPortalUpsideDown)
    {
        // If we have a teleport vfx
        if (this.teleportParticles != null)
        {
            // Add the spash object
            GameObject particles = Instantiate(this.teleportParticles);
            particles.transform.position = otherPortal.transform.position;
            // Reverse the animation if the other portal is upside down
            if (isNextPortalUpsideDown)
            {
                particles.transform.Rotate(180f, 0f, 0f);
            }

            // Play the vfx
            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
            particleSystem.Play();
        }
    }
}
