using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticking : MonoBehaviour {

    [SerializeField] private float initialDuration = 10f;
    [SerializeField] private float duration;
    [SerializeField] private float initialDelay = 1f;
    [SerializeField] private float delay;
    [SerializeField] private float delayModifier = 0.95f;
    [SerializeField] public bool activate = false;
    private float delayCounter;
    private AudioSource tick;
    private AudioSource failure;

    // Use this for initialization
    void Awake () {
        this.tick = this.GetComponents<AudioSource>()[0];
        this.failure = this.GetComponents<AudioSource>()[1];
    }

    // Use this for initialization
    private void Start()
    {
        this.delay = this.initialDelay;
        this.duration = this.initialDuration;
    }

    // Update is called once per frame
    void Update () {
        if (!activate)
            return;

        if (this.duration <= 0)
        {
            ResetTick();
            return;
        }

        this.duration -= Time.deltaTime;
        this.delayCounter += Time.deltaTime;

        if (this.delayCounter > this.delay)
        {
            this.tick.Play();
            this.delay *= this.delayModifier;
            this.delayCounter = 0;
        }
	}

    public void ResetTick()
    {
        if (this.duration <= 0)
            this.failure.Play();

        this.activate = false;
        this.duration = this.initialDuration;
        this.delay = this.initialDelay;
        this.delayCounter = 0;
    }
}
