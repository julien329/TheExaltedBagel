using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnterListener : MonoBehaviour {

    public delegate void OnTriggerEnterDelegate(GameObject gameObject);
    public OnTriggerEnterDelegate onTriggerEnterDelegate;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.tag == "Player")
        {
            onTriggerEnterDelegate(this.gameObject);
        }
    }
}
