using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterCastle : MonoBehaviour {

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject pointLightPlayer;
    [SerializeField] private AudioClip  audioDoorClose;

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
            // Close the door
            this.door.SetActive(true);

            //Illumination
            directionalLight.SetActive(false);
            pointLightPlayer.SetActive(true);

            // Sound
            SoundManager.instance.PlaySound(audioDoorClose);
        }
    }
}
