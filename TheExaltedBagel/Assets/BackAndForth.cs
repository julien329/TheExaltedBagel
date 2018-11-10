using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForth : MonoBehaviour {

    [SerializeField] private float movement;
    [SerializeField] private float displacement;
    private Vector3 startPosition;

	// Use this for initialization
	void Start () {
        this.startPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = new Vector3(this.transform.position.x + this.movement, this.transform.position.y, this.transform.position.z);

        if (Mathf.Abs(this.startPosition.x - this.transform.position.x) >= displacement)
        {
            this.startPosition = this.transform.position;
            movement *= -1;
        }
	}
}
