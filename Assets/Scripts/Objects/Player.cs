using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlaySide PlaySide { get; set; }
    public bool IsInCooldown { get; set; }
    public float CooldownTime { get; set; }
    public bool DrawMiddle { get; set; }
    public int Score { get; set; }
    public List<GameObject> HandCards { get; set; }
    public List<GameObject> PlayerDeck { get; set; }
    private void Awake()
    {
        HandCards = new List<GameObject>();
        PlayerDeck = new List<GameObject>();
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
        if (fromThisPlayer.HandCards.Count > 0)
        {
            var cards = fromThisPlayer.HandCards;
            StartCoroutine(AnimationController.SlideToHand(cards, toThisPlayer));

            foreach (var card in fromThisPlayer.HandCards.ToList())
            {
                card.GetComponent<Card>().Player = toThisPlayer;
                toThisPlayer.HandCards.Add(card);
                fromThisPlayer.HandCards.Remove(card);

            }
            GameControl.instance.DrawCard(fromThisPlayer, 4);
        }
    }

    public void TakeCards(List<GameObject> cards)
    {
        foreach (var card in cards)
        {
            HandCards.Add(card);
        }
    }
}
