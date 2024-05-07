using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadInGame : MonoBehaviour
{
    public Slider progressBar;

    private void Start()
    {
        LoadScene(1);
    }
    public void LoadScene(int sceneInt)
    {
        StartCoroutine(LoadAsyncScene(sceneInt));
    }

    IEnumerator LoadAsyncScene(int sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
    }
}
