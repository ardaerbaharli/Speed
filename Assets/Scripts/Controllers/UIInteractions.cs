using Speed.Objects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Speed.Controllers
{
    public class UIInteractions : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private Text winnerText;
        [SerializeField] private Text scoreTextBottom;
        [SerializeField] private Text scoreTextTop;


        private Player topPlayer;
        private Player bottomPlayer;
        private GameControl gameControl;

        private void Start()
        {
            gameControl = gameObject.GetComponent<GameControl>();

            topPlayer = gameControl.topPlayer;
            bottomPlayer = gameControl.bottomPlayer;
        }


        public void SpeedButton()
        {
            if (gameControl.isGameOver) return;
            gameControl.twoPlayerKey.ResetKeys();

            var clickedButton = EventSystem.current.currentSelectedGameObject;

            Player whoClicked = clickedButton.name.Substring("SpeedButton_".Length) == "Top" ? topPlayer : bottomPlayer;
            Player to = whoClicked == topPlayer ? bottomPlayer : topPlayer;

            if (gameControl.CheckIfSpeedIsValid())
            {
                if (!whoClicked.isInCooldown)
                    gameControl.SpeedEvent(whoClicked, to);
            }
            else gameControl.SpeedCooldown(whoClicked);
        }

        public void DrawMiddleButton()
        {
            if (gameControl.isGameOver) return;
            var clickedButton = EventSystem.current.currentSelectedGameObject;
            var player = clickedButton.name.Substring("DrawMiddleButton_".Length) == "Top" ? topPlayer : bottomPlayer;

            gameControl.twoPlayerKey.SetTrue(player);

            if (!gameControl.twoPlayerKey.IsBothPlayersAccepted()) return;
            gameControl.twoPlayerKey.ResetKeys();
            gameControl.DealMiddleCards();
        }

        public void PauseButton()
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }

        public void ResumeButton()
        {
            Time.timeScale = 1;
            pauseMenu.GetComponent<PopupController>().Deactivate();
        }

        public void RestartButton()
        {
            SceneController.instance.LoadGameScreen();
            Time.timeScale = 1;
        }

        public void MenuButton()
        {
            SceneController.instance.LoadMainScreen();
        }

        public void UpdateScores()
        {
            scoreTextBottom.text = PlayerPrefs.GetInt("BottomPlayerScore", 0).ToString();
            scoreTextTop.text = PlayerPrefs.GetInt("TopPlayerScore", 0).ToString();
        }

        public void UpdateWinnerText(PlaySide winnerPlaySide)
        {
            winnerText.text = winnerPlaySide == PlaySide.Bottom ? "Bottom Player" : "Top Player";
        }
    }
}