using System.Collections;
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

    private bool isGameOver;

    private void Awake()
    {
        instance = this;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        bottomPlayer = gameObject.AddComponent<Player>();
        bottomPlayer.PlaySide = PlaySide.Bottom;

        topPlayer = gameObject.AddComponent<Player>();
        topPlayer.PlaySide = PlaySide.Top;

        twoMan = gameObject.AddComponent<TwoPlayerKey>();
        twoMan.SetPlayers(bottomPlayer, topPlayer);
    }
    private void Update()
    {
        if (!isGameOver)
        {
            if (!bottomPlayer.HasCards)
                Gameover(bottomPlayer);
            else if (!topPlayer.HasCards)
                Gameover(topPlayer);
        }
    }

    private void Start()
    {
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

            var card = CreateCard(drawedCard);

            card.AddComponent<Button>();
            card.GetComponent<Button>().onClick.AddListener(delegate { card.GetComponent<Card>().CardClick(); });

            if (i < remainingCardCount / 2)
            {
                topPlayer.AddCardToDeck(card);
                card.transform.SetParent(topPlayerDeck.transform);
                card.transform.position = topPlayerDeck.transform.position;
            }
            else
            {
                bottomPlayer.AddCardToDeck(card);
                card.transform.SetParent(bottomPlayerDeck.transform);
                card.transform.position = bottomPlayerDeck.transform.position;
            }
        }
    }

    private Vector3 CardScaler()
    {
        float currentHeight = Screen.height;
        float rate = currentHeight / GameSettings.referenceScreenHeight;
        return GameSettings.referenceCardScale * rate;
    }

    public void DealMiddleCards()
    {
        int topPlayerDrawCardsCount = topPlayerDrawCards.transform.childCount;
        int bottomPlayerDrawCardsCount = bottomPlayerDrawCards.transform.childCount;
        if (topPlayerDrawCardsCount > 0 && bottomPlayerDrawCardsCount > 0)
        {
            var leftGroundCard = topPlayerDrawCards.transform.GetChild(topPlayerDrawCardsCount - 1).gameObject;
            StartCoroutine(AnimationController.SlideToMiddle(leftGroundCard, leftGroundCardHolder.transform));

            var rightGroundCard = bottomPlayerDrawCards.transform.GetChild(bottomPlayerDrawCardsCount - 1).gameObject;
            StartCoroutine(AnimationController.SlideToMiddle(rightGroundCard, rightGroundCardHolder.transform));
        }
        else
        {
            StartCoroutine(SideCardsEmpty());
        }
    }

    private IEnumerator SideCardsEmpty()
    {
        int leftGroundCount = leftGroundCardHolder.transform.childCount;
        int rightGroundCount = rightGroundCardHolder.transform.childCount;

        for (int i = leftGroundCount - 1; i >= 0; i--)
        {
            var cardObj = leftGroundCardHolder.transform.GetChild(i).gameObject;
            StartCoroutine(AnimationController.SlideToMiddle(cardObj, topPlayerDrawCards.transform));
        }

        for (int i = rightGroundCount - 1; i >= 0; i--)
        {
            var cardObj = rightGroundCardHolder.transform.GetChild(i).gameObject;
            StartCoroutine(AnimationController.SlideToMiddle(cardObj, bottomPlayerDrawCards.transform));
        }

        do
        {
            yield return new WaitForEndOfFrame();
        } while (bottomPlayerDrawCards.transform.GetChild(0).GetComponent<Card>().IsSliding || topPlayerDrawCards.transform.GetChild(0).GetComponent<Card>().IsSliding);

        DealMiddleCards();
    }

    private void DealSideCards()
    {
        int sideCardCount = GameSettings.deckCount * 4 * 2 + 2;

        for (int i = 0; i < sideCardCount; i++)
        {
            var drawedCard = DeckController.instance.DrawRandomCard();
            var card = CreateCard(drawedCard);

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

    private GameObject CreateCard(Card drawedCard)
    {
        var card = Instantiate(cardPrefab);
        card.GetComponent<RectTransform>().localScale = CardScaler();
        card.AddComponent<Card>();
        card.name = drawedCard.CardName;
        card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Cards/{card.name}");
        card.GetComponent<Card>().ID = drawedCard.ID;
        card.GetComponent<Card>().CardName = drawedCard.CardName;
        card.GetComponent<Card>().Suit = drawedCard.Suit;
        card.GetComponent<Card>().Value = drawedCard.Value;
        return card;
    }

    public void DrawCard(Player player, int count)
    {
        if (!player.HasCards)
        {
            Gameover(player);
        }
        else if (player.HandCards.Count < 4 && player.Deck.Count != 0)
        {
            if (player.Deck.Count < count)
                count = player.Deck.Count;

            var cards = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                var card = player.Deck.Last();
                player.RemoveCardFromDeck(player.Deck.Last());
                player.AddCardToHand(card);
                card.GetComponent<Card>().Player = player;
                cards.Add(card);
            }

            StartCoroutine(AnimationController.SlideToHand(cards, player));
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

    public void SpeedCooldown(Player player)
    {
        player.Cooldown(GameSettings.cooldownTime);
    }

    public void SpeedEvent(Player fromThisPlayer, Player toThisPlayer)
    {
        UpdateLastSpeed();

        fromThisPlayer.GiveCards(fromThisPlayer, toThisPlayer);
    }

    private void UpdateLastSpeed()
    {
        lastSpeed.Clear();

        if (rightGroundCardHolder.transform.childCount > 0 && leftGroundCardHolder.transform.childCount > 0)
        {
            lastSpeed.Add(leftGroundCardHolder.transform.GetChild(leftGroundCardHolder.transform.childCount - 1).transform.GetComponent<Card>());
            lastSpeed.Add(rightGroundCardHolder.transform.GetChild(rightGroundCardHolder.transform.childCount - 1).transform.GetComponent<Card>());
        }
    }
    private void Gameover(Player winner)
    {
        isGameOver = true;
        winner.Score++;
        string key = $"{winner.PlaySide}PlayerScore";
        PlayerPrefs.SetInt(key, winner.Score);

    }

    public GameObject GetTargetCard(GameObject cardObject)
    {
        var card = cardObject.GetComponent<Card>();

        var leftCard = leftGroundCardHolder.transform.GetChild(leftGroundCardHolder.transform.childCount - 1).GetComponent<Card>();
        var rightCard = rightGroundCardHolder.transform.GetChild(rightGroundCardHolder.transform.childCount - 1).GetComponent<Card>();
        GameObject target = null;

        if (IsCardMatching(card, leftCard))
            target = leftGroundCardHolder;
        else if (IsCardMatching(card, rightCard))
            target = rightGroundCardHolder;

        if (target != null)
        {
            twoMan.ResetKeys();
        }

        return target;
    }

    public bool IsCardMatching(Card card, Card holder)
    {
        bool isAisAce = card.Value == 1;
        bool isAisK = card.Value == 13;

        bool isBisAce = holder.Value == 1;
        bool isBisK = holder.Value == 13;

        if (isAisAce && isBisK)
            return true;

        if (isAisK && isBisAce)
            return true;

        if (card.Value == holder.Value - 1)
            return true;

        if (card.Value == holder.Value + 1)
            return true;

        return false;
    }

}
