using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {

    static public LevelLoader instance;

    [SerializeField] private float fadeSpeed = 1f;

    private Animator animator;
    private Canvas canvas;
    private TextMeshProUGUI loadingText;
    private Image background;
    private int levelIndex = 0;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        LevelLoader.instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.canvas = GetComponent<Canvas>();
        this.animator = GetComponent<Animator>();
        this.loadingText = GetComponentInChildren<TextMeshProUGUI>();
        this.background = GetComponentInChildren<Image>();

        this.canvas.enabled = true;
        this.loadingText.enabled = false;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelASync());
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
}
