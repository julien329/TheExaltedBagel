using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject creditsScreen;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClickPlay()
    {
        print("Play");
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClickControls()
    {
        this.lobbyScreen.SetActive(false);
        this.controlsScreen.SetActive(true);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClickCredits()
    {
        this.lobbyScreen.SetActive(false);
        this.creditsScreen.SetActive(true);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClickExit()
    {
        Application.Quit();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClickBack()
    {
        this.lobbyScreen.SetActive(true);
        this.controlsScreen.SetActive(false);
        this.creditsScreen.SetActive(false);
    }
}
