using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedGate : MonoBehaviour {

    [SerializeField] private List<ButtonClick> buttons;
    [SerializeField] private float delay;
    private AudioSource openGateSound;
    private Ticking tick;
    private float timer;
    private bool countdown;
    private GameObject gate;
    private bool soundPlayed = true;

    private void Awake()
    {
        this.gate = this.transform.Find("Gate").gameObject;
        foreach (Transform t in transform)
        {
            if (t.name == "TimedButton")
            {
                buttons.Add(t.GetComponent<ButtonClick>());
            }
        }
        this.timer = this.delay;
        this.openGateSound = this.GetComponent<AudioSource>();
        this.tick = this.transform.Find("TimerTicking").GetComponent<Ticking>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.countdown)
        {
            this.timer -= Time.deltaTime;
        }
        if (this.timer < 0)
        {
            ResetGate();
        }

        bool openGate = true;
        foreach (ButtonClick b in this.buttons)
        {
            if (!b.GetClicked())
            {
                openGate = false;
                break;
            }
        }

        if (!this.countdown && !openGate)
        {
            foreach (ButtonClick b in this.buttons)
            {
                if (b.GetClicked())
                {
                    this.countdown = true;
                    this.tick.activate = true;
                    break;
                }
            }
        }

        if (openGate)
        {
            PlaySound();
            foreach (Transform t in this.gate.transform)
            {
                t.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            this.gate.GetComponent<BoxCollider>().enabled = false;
            BroadcastMessage("AllClicked");
            this.countdown = false;
            this.tick.activate = false;
            this.tick.ResetTick();
        }
    }

    public void ResetGate()
    {
        //Reset buttons first
        BroadcastMessage("ResetButton");

        //Reset gate after
        foreach (Transform t in this.gate.transform)
        {
            t.gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        this.gate.GetComponent<BoxCollider>().enabled = true;
        this.timer = this.delay;
        countdown = false;
        soundPlayed = false;
        this.tick.activate = false;
        this.tick.ResetTick();
    }

    void PlaySound()
    {
        if (!this.soundPlayed)
        {
            this.openGateSound.Play();
            this.soundPlayed = true;
        }
    }
}
