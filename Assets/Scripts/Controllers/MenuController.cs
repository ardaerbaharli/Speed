using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class MenuController : MonoBehaviour
    {
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

        public void ResetScores()
        {
            PlayerPrefs.SetInt("TopPlayerScore", 0);
            PlayerPrefs.SetInt("BottomPlayerScore", 0);

            bottomScore.text = PlayerPrefs.GetInt("BottomPlayerScore", 0).ToString();
            topScore.text = PlayerPrefs.GetInt("TopPlayerScore", 0).ToString();
        }
    }
}