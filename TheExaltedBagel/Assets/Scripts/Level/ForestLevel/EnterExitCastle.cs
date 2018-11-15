using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExitCastle : MonoBehaviour {

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject pointLightPlayer;
    [SerializeField] private AudioClip  audioDoorClose;
    [SerializeField] private Material snowSkyBox;
    [SerializeField] private bool  isEnterCastle = true;

    private bool isAlreadyEnterInCastle = false;
    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        // Enter in the castle , the close door is not suppose to be there
        if (this.isEnterCastle)
        {
            this.door.SetActive(!this.isEnterCastle);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Door
            if(this.isEnterCastle)
            {
                if(!isAlreadyEnterInCastle)
                {
                    this.door.SetActive(this.isEnterCastle);
                    SoundManager.instance.PlaySound(this.audioDoorClose);
                    this.directionalLight.SetActive(false);
                    this.isAlreadyEnterInCastle = true;
                }
            }
            else
            {
                RenderSettings.skybox = snowSkyBox;
                // TO CHANGE TO BRIGHHT MOTHAFUCKAA
                RenderSettings.ambientLight = new Color(10f, 10f, 10f, 0f);
            }

            //Illumination
            this.pointLightPlayer.SetActive(this.isEnterCastle);
        }
    }
}
