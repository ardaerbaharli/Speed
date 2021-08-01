using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Interactions : MonoBehaviour
{
    private GameControl gameControl;
    private Player bottomPlayer;
    private Player topPlayer;

    private void Awake()
    {
        gameControl = gameObject.GetComponent<GameControl>();

        bottomPlayer = gameControl.bottomPlayer;
        topPlayer = gameControl.topPlayer;
    }

    public void SpeedButton()
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

    public void DrawMiddleButton()
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
