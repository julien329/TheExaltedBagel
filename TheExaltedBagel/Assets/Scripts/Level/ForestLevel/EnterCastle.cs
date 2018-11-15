using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCastle : MonoBehaviour {

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject pointLightPlayer;
    [SerializeField] private AudioClip  audioDoorClose;
    [SerializeField] private GameObject dynamicSpike;

    private bool isAlreadyEnterCastle = false;
    private Vector3 spikeStartPosition; 

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        // Enter in the castle , the close door is not suppose to be there
        this.door.SetActive(false);

        spikeStartPosition = dynamicSpike.transform.position;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            dynamicSpike.transform.position = spikeStartPosition;
            dynamicSpike.SetActive(false);

            if(!this.isAlreadyEnterCastle)
            {
                // Close the door
                this.door.SetActive(true);

                //Illumination
                directionalLight.SetActive(false);
                pointLightPlayer.SetActive(true);

                // Sound
                SoundManager.instance.PlaySound(audioDoorClose);
                this.isAlreadyEnterCastle = true;
            }
        }

    }
}
