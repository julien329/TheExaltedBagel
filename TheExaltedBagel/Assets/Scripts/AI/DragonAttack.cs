using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttack : MonoBehaviour {

    [SerializeField] private GameObject dragon;
    private Animator animator;
    private DragonManager dm;

    // Use this for initialization
    void Awake () {
        this.animator = this.dragon.GetComponent<Animator>();
        this.dm = this.dragon.GetComponent<DragonManager>();
	}

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            this.animator.SetTrigger("Fly Idle");
            dm.ToggleFlying();
            this.gameObject.SetActive(false);
        }
    }
}
