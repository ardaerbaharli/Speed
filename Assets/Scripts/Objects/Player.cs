using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlaySide playSide { get; set; }
    public bool IsInCooldown { get; set; }
    public float CooldownTime { get; set; }
    public bool DrawMiddle { get; set; }

    public List<GameObject> handCards { get; set; }
    public List<GameObject> playerDeck { get; set; }
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

    public void Cooldown(float time)
    {
        IsInCooldown = true;
        CooldownTime = time;
    }

    public void GiveCards(Player fromThisPlayer, Player toThisPlayer)
    {
        if (fromThisPlayer.handCards.Count > 0)
        {
            var cards = fromThisPlayer.handCards;
            StartCoroutine(AnimationController.SlideToHand(cards, toThisPlayer));

            foreach (var card in fromThisPlayer.handCards.ToList())
            {
                card.GetComponent<Card>().player = toThisPlayer;
                toThisPlayer.handCards.Add(card);
                fromThisPlayer.handCards.Remove(card);

            }
            GameControl.instance.DrawCard(fromThisPlayer, 4);
        }
    }

    public void TakeCards(List<GameObject> cards)
    {
        foreach (var card in cards)
        {
            handCards.Add(card);
        }
    }
}
