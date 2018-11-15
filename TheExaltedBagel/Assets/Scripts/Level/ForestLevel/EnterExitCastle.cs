using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExitCastle : MonoBehaviour {

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject pointLightPlayer;
    [SerializeField] private AudioClip audioDoorClose;
    [SerializeField] private Material snowSkyBox;
    [SerializeField] private bool isEnterCastle = true;

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
            if (this.isEnterCastle)
            {
                if (!this.isAlreadyEnterInCastle)
                {
                    this.door.SetActive(true);
                    this.isAlreadyEnterInCastle = true;
                    SoundManager.instance.PlaySound(this.audioDoorClose);
                }
            }
            else
            {
                RenderSettings.skybox = snowSkyBox;
                // TO CHANGE TO BRIGHHT MOTHAFUCKAA
                float ambiantColor = 138f / 255f;
                RenderSettings.ambientLight = new Color(ambiantColor, ambiantColor, ambiantColor);
                this.directionalLight.GetComponent<Light>().color = Color.white;
            }

            //Illumination
            this.pointLightPlayer.SetActive(this.isEnterCastle);
            this.directionalLight.SetActive(!this.isEnterCastle);
        }
    }
}
