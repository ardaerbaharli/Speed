using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private static SceneController instance;
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
 
    public static void LoadGameScreen()
    {
        instance.StartCoroutine(LoadAsyncScene("Game"));
    }

    public static void LoadMainScreen()
    {
        instance.StartCoroutine(LoadAsyncScene("Main"));
    }

    public static IEnumerator LoadAsyncScene(string sceneName)
    {
        System.GC.Collect();
        Resources.UnloadUnusedAssets();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
