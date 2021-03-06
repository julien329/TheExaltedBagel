﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterExitCastle : MonoBehaviour {

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject directionalLight;
    [SerializeField] private GameObject pointLightPlayer;
    [SerializeField] private AudioClip audioDoorClose;
    [SerializeField] private Material skyBox;
    [SerializeField] private bool isEnterCastle = true;

    private bool isAlreadyEnterInCastle = false;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        // Enter in the castle , the close door is not suppose to be there
        if (this.isEnterCastle && this.door != null)
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
                    this.isAlreadyEnterInCastle = true;

                    if (this.door != null)
                    {
                        this.door.SetActive(true);
                    }
                }
                float directLightColor = 190f / 255f;
                RenderSettings.ambientLight = new Color(0.0f, 0.0f, 0.0f);
                this.directionalLight.GetComponent<Light>().color = new Color(directLightColor, directLightColor, directLightColor);
            }
            else
            {
                if (this.door != null)
                {
                    this.door.SetActive(false);
                }
                // TO CHANGE TO BRIGHHT MOTHAFUCKAA
                float ambiantColor = 30f / 255f;
                RenderSettings.ambientLight = new Color(ambiantColor, ambiantColor, ambiantColor);
                this.directionalLight.GetComponent<Light>().color = Color.white;
            }

            //Illumination
            this.pointLightPlayer.SetActive(this.isEnterCastle);
            this.directionalLight.SetActive(!this.isEnterCastle);

            if (this.skyBox != null)
            {
                RenderSettings.skybox = skyBox;
            }

            if(this.audioDoorClose != null)
            {
                SoundManager.instance.PlaySound(this.audioDoorClose);
            }
        }
    }
}
