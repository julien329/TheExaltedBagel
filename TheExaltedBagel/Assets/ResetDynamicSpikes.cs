using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDynamicSpikes : MonoBehaviour
{

    [SerializeField] private GameObject[] dynamicSpikes;

    private Vector3[] spikesStartPosition;

    // Use this for initialization
    void Start()
    {

        this.spikesStartPosition = new Vector3[this.dynamicSpikes.Length];
        for (uint i = 0; i < this.dynamicSpikes.Length; ++i)
        {
            this.spikesStartPosition[i] = this.dynamicSpikes[i].transform.position;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (uint i = 0; i < dynamicSpikes.Length; ++i)
            {
                this.dynamicSpikes[i].transform.position = this.spikesStartPosition[i];
                this.dynamicSpikes[i].SetActive(false);
            }
            LevelManager.instance.IsDynamicSpikesReset = true;
        }
    }
}
