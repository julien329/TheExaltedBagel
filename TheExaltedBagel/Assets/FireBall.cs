﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {
    
    [SerializeField] private Transform target;
    [SerializeField] private GameObject explosionParticles;
    [SerializeField] private float speed;

    private Vector3 direction;

    private void Awake()
    {
        if (GameObject.Find("Player") == null)
            direction = new Vector3(62f, 10f, 0f);
        else
        {
            target = GameObject.Find("Player").GetComponent<Transform>();
            direction = (target.position - this.transform.position).normalized;
        }
    }

    void Update()
    {
        this.transform.position = this.transform.position + this.direction * this.speed * Time.deltaTime;

        if (this.transform.position.z < -5)
        {
            Explode();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        ParticleManager.instance.PlayParticleSystem(this.explosionParticles, this.transform.position);
        Destroy(this.gameObject);
    }
}
