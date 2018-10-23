using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            LevelManager.instance.KillPlayer(false);
        }
    }
}
