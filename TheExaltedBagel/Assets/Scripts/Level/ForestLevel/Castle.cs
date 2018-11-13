using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour {

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject pointLightPlayer;
    [SerializeField] private AudioClip  audioDoorClose;

    private bool isEnterCastle = true;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        // Enter in the castle , the close door is not suppose to be there
        this.door.SetActive(false);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Enter Castle
            if(isEnterCastle)
            {
                // Close the door
                this.door.SetActive(true);
                isEnterCastle = false;

                //Illumination
                directionalLight.SetActive(false);
                pointLightPlayer.SetActive(true);

                // Sound
                SoundManager.instance.PlaySound(audioDoorClose);
            }
        }
    }
}
