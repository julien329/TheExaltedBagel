using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    static public LevelLoader instance;

    private Canvas canvas;
    private int levelIndex = 0;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        LevelLoader.instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.canvas = GetComponent<Canvas>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelASync());
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator LoadLevelASync()
    {
        this.canvas.enabled = true;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(++this.levelIndex);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        this.canvas.enabled = false;
    }
}
