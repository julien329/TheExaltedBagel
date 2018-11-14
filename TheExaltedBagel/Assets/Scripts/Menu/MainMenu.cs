using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private AudioClip bagelClip;
    [SerializeField] private AudioClip acceptSound;
    [SerializeField] private float controllerPollRate = 1f;
    [SerializeField] private Button[] mainButtons;
    [SerializeField] private Button[] backButtons;

    private bool gameStarted;
    private uint gamepadCount = 0;
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
    void Start()
    {
        StartCoroutine(PollGamepads());
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
        AutoSelectButton();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void OnClickCredits()
    {
        this.lobbyScreen.SetActive(false);
        this.creditsScreen.SetActive(true);
        AutoSelectButton();
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
        AutoSelectButton();
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
    public void AutoSelectButton()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (this.gamepadCount > 0)
        {
            if (this.lobbyScreen.activeSelf)
            {
                this.mainButtons[0].Select();
            }
            else if (this.controlsScreen.activeSelf)
            {
                this.backButtons[0].Select();
            }
            else if (this.creditsScreen.activeSelf)
            {
                this.backButtons[1].Select();
            }
        }
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

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator PollGamepads()
    {
        while (true)
        {
            uint nbConnectedGamepad = 0;
            string[] names = Input.GetJoystickNames();
            foreach (string name in names)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    nbConnectedGamepad++;
                }
            }

            if (nbConnectedGamepad != this.gamepadCount)
            {
                this.gamepadCount = nbConnectedGamepad;

                for (int i = 0; i < this.mainButtons.Length; ++i)
                {
                    Navigation navigation = new Navigation();
                    navigation.mode = (this.gamepadCount > 0) ? Navigation.Mode.Vertical : Navigation.Mode.None;
                    this.mainButtons[i].navigation = navigation;
                }

                for (int i = 0; i < this.backButtons.Length; ++i)
                {
                    Navigation navigation = new Navigation();
                    navigation.mode = (this.gamepadCount > 0) ? Navigation.Mode.Vertical : Navigation.Mode.None;
                    this.backButtons[i].navigation = navigation;
                }

                AutoSelectButton();
            }

            yield return new WaitForSeconds(this.controllerPollRate);
        }
    }
} 
