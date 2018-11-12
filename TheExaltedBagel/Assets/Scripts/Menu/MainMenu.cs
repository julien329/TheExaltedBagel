using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private AudioClip bagelClip;
    [SerializeField] private AudioClip acceptSound;

    private bool gameStarted;
    private AudioSource audioSource;
    private Animator animator;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.animator = GetComponent<Animator>();
        this.audioSource = GetComponent<AudioSource>();

        this.lobbyScreen.SetActive(true);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClickPlay()
    {
        this.lobbyScreen.SetActive(false);
        this.animator.SetTrigger("ShowScroll");
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

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnScrollVisible()
    {
        StartCoroutine(DismissScroll());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void LoadLevel()
    {
        this.audioSource.Stop();
        LevelLoader.instance.LoadNextLevel();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void PlayBagelSound()
    {
        this.audioSource.clip = this.bagelClip;
        this.audioSource.loop = false;
        this.audioSource.Play();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator DismissScroll()
    {
        this.audioSource.Stop();

        while (true)
        {
            if (Input.GetButtonDown("Jump") && !this.gameStarted)
            {
                this.gameStarted = true;

                this.animator.SetTrigger("ShowBagel");
                this.audioSource.clip = this.acceptSound;
                this.audioSource.loop = false;
                this.audioSource.Play();
            }

            yield return null;
        }
    }
} 
