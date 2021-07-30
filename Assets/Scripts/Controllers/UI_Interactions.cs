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

        bottomPlayer = gameObject.AddComponent<Player>();
        bottomPlayer.playSide = PlaySide.Bottom;
        topPlayer = gameObject.AddComponent<Player>();
        topPlayer.playSide = PlaySide.Top;
    }

    public void SpeedButton()
    {
        var clickedButton = EventSystem.current.currentSelectedGameObject;

        Player whoClicked = clickedButton.name.Substring("SpeedButton_".Length) == "Top" ? topPlayer : bottomPlayer;
        Player to = whoClicked == topPlayer ? bottomPlayer : topPlayer;


        if (gameControl.CheckIfSpeedIsValid())
        {
            if (!whoClicked.IsInCooldown)
            {
                gameControl.SpeedEvent(whoClicked, to);
            }
        }
        else gameControl.SpeedCooldown(whoClicked);


        //if (!whoClicked.IsInCooldown)
        //{
        //    if (gameControl.CheckIfSpeedIsValid())
        //    {
        //        Player to = topPlayer;
        //        gameControl.SpeedEvent(whoClicked, to);
        //    }
        //    else
        //        gameControl.SpeedCooldown(whoClicked);
        //}
    }
    public void DrawMiddleButton()
    {

    }
}
