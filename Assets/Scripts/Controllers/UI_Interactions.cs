using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Interactions : MonoBehaviour
{
    [SerializeField] GameObject topPlayerName;
    [SerializeField] GameObject bottomPlayerName;
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
