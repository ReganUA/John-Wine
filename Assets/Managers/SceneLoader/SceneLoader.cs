using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("UI References")]
    public GameObject loadingScreen;
    public Slider loadingBar;

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsynchronously(sceneIndex));
    }

    private IEnumerator LoadSceneAsynchronously(int sceneIndex)
    {
        TransitionData.PreviousSceneName = SceneManager.GetActiveScene().buildIndex;

        if (loadingScreen != null) loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (operation.isDone == false)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (loadingBar != null)
                loadingBar.value = progress;

            yield return null;
        }
    }
}