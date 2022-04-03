using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Speed.Controllers
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private GameObject scorePanel;

        [SerializeField] private Text bottomScore;
        [SerializeField] private Text topScore;

        void Start()
        {
            bottomScore.text = PlayerPrefs.GetInt("BottomPlayerScore", 0).ToString();
            topScore.text = PlayerPrefs.GetInt("TopPlayerScore", 0).ToString();
        }


        public void StartGame()
        {
            SceneController.instance.LoadGameScreen();
        }

        public void ShowScores()
        {
            scorePanel.SetActive(true);
        }


        public void ResetScores()
        {
            PlayerPrefs.SetInt("TopPlayerScore", 0);
            PlayerPrefs.SetInt("BottomPlayerScore", 0);

            bottomScore.text = PlayerPrefs.GetInt("BottomPlayerScore", 0).ToString();
            topScore.text = PlayerPrefs.GetInt("TopPlayerScore", 0).ToString();
            StartCoroutine(HideScorePanel());
        }

        private IEnumerator HideScorePanel()
        {
            yield return new WaitForSeconds(0.3f);
            scorePanel.SetActive(false);
        }
    }
}