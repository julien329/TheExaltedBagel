using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAndForth : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float displacement = 8f;

    private Vector3 startPosition;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start ()
    {
        this.startPosition = this.transform.position;
	}

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update ()
    {
        this.transform.Translate(new Vector3(this.speed * Time.deltaTime, 0f, 0f));

        if (Mathf.Abs(this.startPosition.x - this.transform.position.x) >= displacement)
        {
            this.startPosition += new Vector3(Mathf.Sign(this.speed) * this.displacement, 0f, 0f);
            this.speed *= -1;
        }
    }
}
