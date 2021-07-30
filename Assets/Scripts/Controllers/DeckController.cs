using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour
{
    public List<Card> deck;

    public static DeckController instance;
    private void Awake()
    {
        instance = this;
        deck = CreateADeck();
    }
    public List<Card> CreateADeck()
    {
        var deck = new List<Card>();
        for (int i = 0; i < 52; i++)
        {
            var card = gameObject.AddComponent<Card>();
            card.ID = i;

            int cardValueIndex = card.ID % 13;
            switch (cardValueIndex)
            {
                case 0:
                    card.Value = 13;
                    break;
                default:
                    card.Value = cardValueIndex;
                    break;
            }
            int cardSuitIndex = (int)(card.ID / 13f);
            switch (cardSuitIndex)
            {
                case 0:
                    card.Suit = "H";
                    break;
                case 1:
                    card.Suit = "C";
                    break;
                case 2:
                    card.Suit = "S";
                    break;

                case 3:
                    card.Suit = "D";
                    break;
                default:
                    card.Suit = "";
                    break;
            }

            card.CardName = $"{card.Suit}{card.Value}";

            deck.Add(card);
        }
        return deck;
    }
    public Card DrawRandomCard()
    {
        if (deck.Count > 0)
        {
            int cardID = Random.Range(0, deck.Count);
            var card = deck[cardID];
            deck.Remove(card);
            return card;
        }
        else return null;
    }
}

