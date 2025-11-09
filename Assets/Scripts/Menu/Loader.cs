using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loader : MonoBehaviour
{
    [field: SerializeField] public string SceneToLoad0 { get; set; }
    [field: SerializeField] public string SceneToLoad1 { get; set; }

    private Coroutine _loadingCoroutine;
    private AsyncOperation _asyncLoad;

    public void LoadScene0()
    {
        if (string.IsNullOrEmpty(SceneToLoad0))
        {
            Debug.LogError("SceneToLoad0 is not set!");
            return;
        }
        
        if (_loadingCoroutine != null)
        {
            Debug.LogWarning("Scene loading already in progress!");
            return;
        }

        _loadingCoroutine = StartCoroutine(LoadYourAsyncScene(SceneToLoad0));
    }
    
    public void LoadScene1()
    {
        if (string.IsNullOrEmpty(SceneToLoad0))
        {
            Debug.LogError("SceneToLoad0 is not set!");
            return;
        }
        
        if (_loadingCoroutine != null)
        {
            Debug.LogWarning("Scene loading already in progress!");
            return;
        }

        _loadingCoroutine = StartCoroutine(LoadYourAsyncScene(SceneToLoad1));
    }

    private IEnumerator LoadYourAsyncScene(string sceneName)
    {
        _asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        if (_asyncLoad == null)
        {
            Debug.LogError($"Failed to load scene: {sceneName}");
            _loadingCoroutine = null;
            yield break;
        }
       
        _asyncLoad.allowSceneActivation = true;

        while (!_asyncLoad.isDone)
        {
            yield return null;
        }

        _asyncLoad = null;
        _loadingCoroutine = null;
    }

    public void CancelLoading()
    {
        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = null;
            _asyncLoad = null;
            Debug.Log("Scene loading cancelled.");
        }
    }
}