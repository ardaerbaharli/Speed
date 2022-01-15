using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController instance;

        void Start()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadGameScreen()
        {
            instance.StartCoroutine(LoadAsyncScene("Game"));
        }

        public void LoadMainScreen()
        {
            instance.StartCoroutine(LoadAsyncScene("Menu"));
        }

        private IEnumerator LoadAsyncScene(string sceneName)
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
}