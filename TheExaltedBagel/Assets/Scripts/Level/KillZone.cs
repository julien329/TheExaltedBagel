using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            LevelManager.instance.KillPlayer(false);
        }
    }
}
