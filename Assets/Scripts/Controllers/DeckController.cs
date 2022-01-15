using System.Collections.Generic;
using Objects;
using UnityEngine;

namespace Controllers
{
    public class DeckController : MonoBehaviour
    {
        public List<Card> deck;


        public void CreateADeck()
        {
            var cardObject = new GameObject();
            deck = new List<Card>();

            for (int i = 0; i < 52; i++)
            {
                var card = cardObject.AddComponent<Card>();
                card.id = i;

                int cardValueIndex = card.id % 13;
                switch (cardValueIndex)
                {
                    case 0:
                        card.value = 13;
                        break;
                    default:
                        card.value = cardValueIndex;
                        break;
                }

                int cardSuitIndex = (int) (card.id / 13f);
                switch (cardSuitIndex)
                {
                    case 0:
                        card.suit = "H";
                        break;
                    case 1:
                        card.suit = "C";
                        break;
                    case 2:
                        card.suit = "S";
                        break;

                    case 3:
                        card.suit = "D";
                        break;
                    default:
                        card.suit = "";
                        break;
                }

                card.cardName = $"{card.suit}{card.value}";

                deck.Add(card);
            }
            Destroy(cardObject);
        }

        public Card DrawRandomCard()
        {
            if (deck.Count == 0)
                return null;
            
            int cardID = Random.Range(0, deck.Count);
            var card = deck[cardID];
            deck.Remove(card);
            return card;
        }
    }
}