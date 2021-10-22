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
    public bool HasCards { get { return _HasCards(); } }
    public List<GameObject> HandCards { get; private set; }
    public List<GameObject> Deck { get; private set; }
    private void Awake()
    {
        HandCards = new List<GameObject>();
        Deck = new List<GameObject>();
    }

    public void AddCardToHand(GameObject card)
    {
        HandCards.Add(card);
    }
    public void AddCardsToHand(List<GameObject> cards)
    {       
        cards.ForEach(x => HandCards.Add(x));
    }

    public void RemoveCardFromHand(GameObject card)
    {
        HandCards.Remove(card);
    }
    public void RemoveCardsFromHand(List<GameObject> cards)
    {
        cards.ForEach(x => HandCards.Remove(x));
    }

    public void AddCardToDeck(GameObject card)
    {
        Deck.Add(card);
    }
    public void AddCardsToDeck(List<GameObject> cards)
    {
        cards.ForEach(x => Deck.Add(x));

    }

    public void RemoveCardFromDeck(GameObject card)
    {
        Deck.Remove(card);
    }
    public void RemoveCardsFromDeck(List<GameObject> cards)
    {        
        cards.ForEach(x => Deck.Remove(x));
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
    private bool _HasCards()
    {
        if (Deck.Count > 0)
            return true;
        else if (HandCards.Count > 0)
            return true;
        else return false;
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
                //toThisPlayer.HandCards.Add(card);
                toThisPlayer.AddCardToHand(card);
                //fromThisPlayer.HandCards.Remove(card);
                fromThisPlayer.RemoveCardFromHand(card);
            }
            GameControl.instance.DrawCard(fromThisPlayer, 4);
        }
    }

    public void TakeCards(List<GameObject> cards)
    {
        //foreach (var card in cards)
        //{
        //    HandCards.Add(card);
        //}
        AddCardsToHand(cards);
    }
}
