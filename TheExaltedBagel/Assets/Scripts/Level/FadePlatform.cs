using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePlatform : MonoBehaviour
{
    [SerializeField] private GameObject crumblingParticle;
    [SerializeField] private GameObject platform;
    [SerializeField] private AudioClip crumbleSound;
    [SerializeField] private float fadeDelay = 1f;
    [SerializeField] private float respawnDelay = 2f;

    private bool isActive;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        BoxCollider boxCollider = this.platform.GetComponent<BoxCollider>();

        BoxCollider trigger = this.gameObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.center = this.platform.transform.localPosition + boxCollider.center;
        trigger.size = boxCollider.size + new Vector3(0f, 0.1f, 0f);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isActive)
        {
            StartCoroutine(DisablePlatform());
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator DisablePlatform()
    {
        this.isActive = true;

        SoundManager.instance.PlaySound(this.crumbleSound);

        GameObject particleObject = Instantiate(this.crumblingParticle, this.transform);
        particleObject.transform.position = this.platform.GetComponent<BoxCollider>().bounds.center;
        particleObject.transform.localEulerAngles = this.crumblingParticle.transform.localEulerAngles;

        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
        particleSystem.Play();

        yield return new WaitForSeconds(fadeDelay);

        Destroy(particleObject);

        this.platform.SetActive(false);

        yield return new WaitForSeconds(respawnDelay);

        this.platform.SetActive(true);

        this.isActive = false;
    }
}
