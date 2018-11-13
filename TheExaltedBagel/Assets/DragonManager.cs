using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour {
    
    private Animator animator;

    private void Awake()
    {
        this.animator = this.GetComponent<Animator>();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            this.animator.SetTrigger("Bite Attack");
            this.animator.SetTrigger("Fly Bite Attack");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.animator.SetTrigger("Fly Idle");
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.animator.SetTrigger("Fly Take Damage");

        }
    }
}
