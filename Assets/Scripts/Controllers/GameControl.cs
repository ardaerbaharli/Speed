using System;
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

    public TwoPlayerKey twoMan;

    public Player bottomPlayer, topPlayer;

    private List<Card> lastSpeed = new List<Card>();

    private void Awake()
    {
        instance = this;

        bottomPlayer = gameObject.AddComponent<Player>();
        bottomPlayer.playSide = PlaySide.Bottom;
        topPlayer = gameObject.AddComponent<Player>();
        topPlayer.playSide = PlaySide.Top;

        twoMan = gameObject.AddComponent<TwoPlayerKey>();
        twoMan.SetPlayers(bottomPlayer, topPlayer);
    }

    private void Start()
    {
        DeckController.instance.CreateADeck();

        DealSideCards();
        DealPlayerDecks();
        DealPlayerHands();
        DealMiddleCards();
    }

    private void DealPlayerHands()
    {
        DrawCard(bottomPlayer, 4);
        DrawCard(topPlayer, 4);
    }

    private void DealPlayerDecks()
    {
        int remainingCardCount = DeckController.instance.deck.Count;
        for (int i = 0; i < remainingCardCount; i++)
        {
            var drawedCard = DeckController.instance.DrawRandomCard();

            var card = Instantiate(cardPrefab);
            card.AddComponent<Card>();
            card.name = drawedCard.CardName;
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.name);

            card.GetComponent<Card>().ID = drawedCard.ID;
            card.GetComponent<Card>().CardName = drawedCard.CardName;
            card.GetComponent<Card>().Suit = drawedCard.Suit;
            card.GetComponent<Card>().Value = drawedCard.Value;

            if (i < remainingCardCount / 2)
            {
                topPlayer.playerDeck.Add(card);
                card.transform.SetParent(topPlayerDeck.transform);
                card.transform.position = topPlayerDeck.transform.position;
            }
            else
            {
                bottomPlayer.playerDeck.Add(card);
                card.transform.SetParent(bottomPlayerDeck.transform);
                card.transform.position = bottomPlayerDeck.transform.position;
            }
        }
    }

    public void DealMiddleCards()
    {
        var leftGroundCard = topPlayerDrawCards.transform.GetChild(topPlayerDrawCards.transform.childCount - 1).gameObject;
        StartCoroutine(AnimationController.SlideTo(leftGroundCard, leftGroundCardHolder));

        var rightGroundCard = bottomPlayerDrawCards.transform.GetChild(topPlayerDrawCards.transform.childCount - 1).gameObject;
        StartCoroutine(AnimationController.SlideTo(rightGroundCard, rightGroundCardHolder));
    }

    private void DealSideCards()
    {
        int sideCardCount = GameSettings.deckCount * 4 * 2 + 2;

        for (int i = 0; i < sideCardCount; i++)
        {
            var drawedCard = DeckController.instance.DrawRandomCard();

            var card = Instantiate(cardPrefab);
            card.AddComponent<Card>();
            card.name = drawedCard.CardName;
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>(card.name);

            card.GetComponent<Card>().ID = drawedCard.ID;
            card.GetComponent<Card>().CardName = drawedCard.CardName;
            card.GetComponent<Card>().Suit = drawedCard.Suit;
            card.GetComponent<Card>().Value = drawedCard.Value;

            if (i < 5)
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


    public bool CheckIfSpeedIsValid()
    {
        var leftCard_go = leftGroundCardHolder.transform.GetChild(leftGroundCardHolder.transform.childCount - 1).transform;
        var leftCard = leftCard_go.GetComponent<Card>();
        int leftCardValue = leftCard.Value;

        var rightCard_go = rightGroundCardHolder.transform.GetChild(rightGroundCardHolder.transform.childCount - 1).transform;
        var rightCard = rightCard_go.GetComponent<Card>();
        int rightCardValue = rightCard.Value;

        if (IsThisNewSpeed(leftCard, rightCard))
        {
            if (leftCardValue == rightCardValue)
                return true;
        }
        return false;
    }

    private bool IsThisNewSpeed(Card leftCard, Card rightCard)
    {
        if (lastSpeed.Count == 2)
        {
            var oldSpeedLeftCard = lastSpeed[0];
            var oldSpeedRightCard = lastSpeed[1];

            if (Card.IsEqual(leftCard, oldSpeedLeftCard) && Card.IsEqual(rightCard, oldSpeedRightCard))
                return false;
        }
        return true;
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
        UpdateLastSpeed();

        fromThisPlayer.GiveCards(fromThisPlayer, toThisPlayer);
    }

    private void UpdateLastSpeed()
    {
        lastSpeed.Clear();

        lastSpeed.Add(leftGroundCardHolder.transform.GetChild(rightGroundCardHolder.transform.childCount - 1).transform.GetComponent<Card>());
        lastSpeed.Add(rightGroundCardHolder.transform.GetChild(rightGroundCardHolder.transform.childCount - 1).transform.GetComponent<Card>());
    }
}
