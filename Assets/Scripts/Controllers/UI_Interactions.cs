using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Interactions : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject topPlayerName;
    [SerializeField] private GameObject bottomPlayerName;
    [SerializeField] private GameObject pauseMenu;

    private Player topPlayer;
    private Player bottomPlayer;

    private GameControl gameControl;

    private void Awake()
    {
        gameControl = gameObject.GetComponent<GameControl>();

        topPlayer = gameControl.topPlayer;
        bottomPlayer = gameControl.bottomPlayer;
    }


    public void SpeedButton()
    {
        if (!gameControl.isGameOver)
        {
            var clickedButton = EventSystem.current.currentSelectedGameObject;

            Player whoClicked = clickedButton.name.Substring("SpeedButton_".Length) == "Top" ? topPlayer : bottomPlayer;
            Player to = whoClicked == topPlayer ? bottomPlayer : topPlayer;

            if (gameControl.CheckIfSpeedIsValid())
            {
                if (!whoClicked.IsInCooldown)
                    gameControl.SpeedEvent(whoClicked, to);
            }
            else gameControl.SpeedCooldown(whoClicked);
        }
    }

    public void DrawMiddleButton()
    {
        if (!gameControl.isGameOver)
        {
            var clickedButton = EventSystem.current.currentSelectedGameObject;
            var player = clickedButton.name.Substring("DrawMiddleButton_".Length) == "Top" ? topPlayer : bottomPlayer;

            gameControl.twoMan.SetTrue(player);
            if (gameControl.twoMan.IsBothPlayersAccepted())
            {
                gameControl.twoMan.ResetKeys();
                gameControl.DealMiddleCards();
            }
        }
    }

    public void PauseButton()
    {
        var menu = Instantiate(pauseMenu, canvas.transform);
        menu.tag = "PauseMenu";
    }
}
