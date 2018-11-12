using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyScreen;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject creditsScreen;
    [SerializeField] private float continueTime = 0.5f;

    private Animator animator;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        this.animator = GetComponent<Animator>();
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
        LevelLoader.instance.LoadNextLevel();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator DismissScroll()
    {
        yield return new WaitForSeconds(this.continueTime);

        while (true)
        {
            if (Input.GetButtonDown("Jump"))
            {
                this.animator.SetTrigger("ShowBagel");
            }
            yield return null;
        }
    }
} 
