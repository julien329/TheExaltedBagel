using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDestroyListener : MonoBehaviour {

    public delegate void OnDestroyDelegate();
    public OnDestroyDelegate onDestroyDelegate;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnDestroy()
    {
        onDestroyDelegate();
        onDestroyDelegate = null;
    }
}
