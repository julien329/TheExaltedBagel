using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    [SerializeField] private GameObject levelManager;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        if (LevelManager.instance == null)
        {
            Instantiate(levelManager);
        }
    }
}
