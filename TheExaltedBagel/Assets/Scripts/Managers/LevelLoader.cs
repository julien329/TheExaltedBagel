using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    static public LevelLoader instance;

    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float outroDuration = 5f;

    private Canvas canvas;
    private TextMeshProUGUI loadingText;
    private TextMeshProUGUI endingText;
    private Image background;
    private int levelIndex = 0;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        this.canvas = GetComponent<Canvas>();
        this.loadingText = GameObject.Find("Loading").GetComponent<TextMeshProUGUI>();
        this.endingText = GameObject.Find("TheEnd").GetComponent<TextMeshProUGUI>();
        this.background = GetComponentInChildren<Image>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Start()
    {
        this.canvas.enabled = true;
        this.loadingText.enabled = false;
        this.endingText.enabled = false;
        this.background.color = new Color(0f, 0f, 0f, 0f);
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelASync());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void PlayEndingOutro()
    {
        StartCoroutine(EndingOutro());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator LoadLevelASync()
    {
        this.loadingText.enabled = true;
        this.background.color = new Color(0f, 0f, 0f, 1f);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(++this.levelIndex);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        this.loadingText.enabled = false;

        float alpha = 1f;
        while (alpha > 0)
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime * this.fadeSpeed);
            this.background.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator EndingOutro()
    {
        yield return new WaitForSeconds(this.outroDuration);

        SoundManager.instance.FadeOutAmbiant(this.fadeSpeed);

        float alpha = 0f;
        while (alpha < 1)
        {
            alpha = Mathf.Clamp01(alpha + Time.deltaTime * this.fadeSpeed);
            this.background.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }

        this.endingText.enabled = true;

        while (!Input.GetButtonDown("Jump"))
        {
            yield return null;
        }

        this.levelIndex = 0;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(this.levelIndex);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        this.endingText.enabled = false;

        alpha = 1f;
        while (alpha > 0)
        {
            alpha = Mathf.Clamp01(alpha - Time.deltaTime * this.fadeSpeed);
            this.background.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }
}