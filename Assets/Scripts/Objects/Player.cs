using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlaySide playSide { get; set; }
    public bool IsInCooldown { get; set; }
    public float CooldownTime { get; set; }
    public bool DrawMiddle { get; set; }

    public List<GameObject> handCards;
    public List<GameObject> playerDeck;
    private void Awake()
    {
        handCards = new List<GameObject>();
        playerDeck = new List<GameObject>();
    }
    private void Update()
    {
        if (IsInCooldown)
        {
            if (CooldownTime > 0)
                CooldownTime -= Time.deltaTime;
            else
                IsInCooldown = false;
        }
    }

    /// <summary>
    /// Cooldown the speed button of this player.
    /// </summary>
    /// <param name="time">For how long</param>
    public void Cooldown(float time)
    {
        IsInCooldown = true;
        CooldownTime = time;
    }

    /// <summary>
    /// Give the cards in the hand to this player as the result of the speed
    /// </summary>
    /// <param name="toThisPlayer">Player who is getting the cards.</param>
    /// <param name="fromThisPlayer">Player who is giving the cards.</param>
    public void GiveCards(Player fromThisPlayer, Player toThisPlayer)
    {
        if (fromThisPlayer.handCards.Count > 0)
        {
            var cards = fromThisPlayer.handCards;
            StartCoroutine(AnimationController.SlideToHand(cards, toThisPlayer));

            foreach (var card in fromThisPlayer.handCards.ToList())
            {
                fromThisPlayer.handCards.Remove(card);
            }
            GameControl.instance.DrawCard(fromThisPlayer, 4);
            toThisPlayer.TakeCards(fromThisPlayer.handCards);
        }
    }

    /// <summary>
    /// Add given cards to the hand
    /// </summary>
    /// <param name="cards">List of Card objects</param>
    public void TakeCards(List<GameObject> cards)
    {
        foreach (var card in cards)
        {
            handCards.Add(card);
        }
    }
}
