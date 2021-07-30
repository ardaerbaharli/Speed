using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public GameObject leftGroundCardHolder;
    public GameObject rightGroundCardHolder;

    public GameObject topPlayerDrawCards;
    public GameObject bottomPlayerDrawCards;

    public GameObject topHand;
    public GameObject bottomHand;

    public GameObject bottomPlayerDeck;
    public GameObject topPlayerDeck;

    public GameObject cardPrefab;
    public static GameControl instance;

    private Player bottom, top;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        var players = gameObject.GetComponents<Player>();

        if (players[0].playSide == PlaySide.Bottom)
        {
            bottom = players[0];
            top = players[1];
        }
        else
        {
            top = players[0];
            bottom = players[1];
        }

        DeckController.instance.CreateADeck();

        DealMiddle();
        DealPlayerDecks();
        DealPlayerHands();
    }

    private void DealPlayerHands()
    {
        DrawCard(bottom, 4);
        DrawCard(top, 4);
    }

    private void DealPlayerDecks()
    {
        int remainingCardCount = DeckController.instance.deck.Count;
        for (int i = 0; i < remainingCardCount; i++)
        {
            var drawedCard = DeckController.instance.DrawRandomCard();

            var card = Instantiate(cardPrefab);
            card.AddComponent<Card>();
            card.name = drawedCard.Name;
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.name);

            card.GetComponent<Card>().ID = drawedCard.ID;
            card.GetComponent<Card>().Name = drawedCard.Name;
            card.GetComponent<Card>().Suit = drawedCard.Suit;
            card.GetComponent<Card>().Value = drawedCard.Value;

            if (i < remainingCardCount / 2)
            {
                top.playerDeck.Add(card);
                card.transform.SetParent(topPlayerDeck.transform);
                card.transform.position = topPlayerDeck.transform.position;
            }
            else
            {
                bottom.playerDeck.Add(card);
                card.transform.SetParent(bottomPlayerDeck.transform);
                card.transform.position = bottomPlayerDeck.transform.position;
            }
        }
    }

    private void DealMiddle()
    {
        int sideCardCount = GameSettings.deckCount * 4 * 2;

        for (int i = 0; i < sideCardCount; i++)
        {
            var drawedCard = DeckController.instance.DrawRandomCard();

            var card = Instantiate(cardPrefab);
            card.AddComponent<Card>();
            card.name = drawedCard.Name;
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.name);

            card.GetComponent<Card>().ID = drawedCard.ID;
            card.GetComponent<Card>().Name = drawedCard.Name;
            card.GetComponent<Card>().Suit = drawedCard.Suit;
            card.GetComponent<Card>().Value = drawedCard.Value;

            if (i < 4)
            {
                card.transform.SetParent(topPlayerDrawCards.transform);
                card.transform.position = topPlayerDrawCards.transform.position;
            }
            else
            {
                card.transform.SetParent(bottomPlayerDrawCards.transform);
                card.transform.position = bottomPlayerDrawCards.transform.position;
            }
        }


        for (int i = 0; i < 2; i++)
        {
            var drawedCard = DeckController.instance.DrawRandomCard();

            var card = Instantiate(cardPrefab);
            card.AddComponent<Card>();
            card.name = drawedCard.Name;
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.name);

            card.GetComponent<Card>().ID = drawedCard.ID;
            card.GetComponent<Card>().Name = drawedCard.Name;
            card.GetComponent<Card>().Suit = drawedCard.Suit;
            card.GetComponent<Card>().Value = drawedCard.Value;

            if (i == 0)
                card.transform.SetParent(leftGroundCardHolder.transform);
            else
                card.transform.SetParent(rightGroundCardHolder.transform);
        }
    }

    public void DrawCard(Player player, int count)
    {
        if (player.handCards.Count < 4 && player.playerDeck.Count != 0)
        {
            if (player.playerDeck.Count < count)
                count = player.playerDeck.Count;

            var cards = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var card = player.playerDeck.Last();
                player.playerDeck.Remove(player.playerDeck.Last());
                player.handCards.Add(card);
                cards.Add(card);
            }
            StartCoroutine(AnimationController.SlideTo(cards, player));
        }
    }

    /// <summary>
    /// Checks if the both middle cards have same values.
    /// </summary>
    /// <returns></returns>
    public bool CheckIfSpeedIsValid()
    {
        var leftCard = leftGroundCardHolder.transform.GetChild(leftGroundCardHolder.transform.childCount - 1).transform;
        int leftCardValue = leftCard.GetComponent<Card>().Value;

        var rightCard = rightGroundCardHolder.transform.GetChild(rightGroundCardHolder.transform.childCount - 1).transform;
        int rightCardValue = rightCard.GetComponent<Card>().Value;

        if (leftCardValue == rightCardValue)
            return true;
        return false;
    }

    /// <summary>
    /// Cooldown the player to prevent spamming.
    /// </summary>
    /// <param name="player">The player invoked the "Speed" action</param>
    public void SpeedCooldown(Player player)
    {
        player.Cooldown(GameSettings.cooldownTime);
    }

    /// <summary>
    /// Actual event of "Speed".
    /// </summary>
    /// <param name="fromThisPlayer">The player invoked the "Speed" action</param>
    /// <param name="toThisPlayer">The player who will take the cards.</param>
    public void SpeedEvent(Player fromThisPlayer, Player toThisPlayer)
    {
        fromThisPlayer.GiveCards(fromThisPlayer, toThisPlayer);
    }
}
