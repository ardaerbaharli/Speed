using System;
using System.Collections;
using System.Collections.Generic;
using Speed.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Speed.Controllers
{
    public enum PlaySide
    {
        Top,
        Bottom
    };

    public class GameControl : MonoBehaviour
    {
        [Header("Main Objects")] [SerializeField]
        private GameObject gamePanel;

        [SerializeField] private GameObject gameOverPanel;

        [Header("Prefabs")] [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private DeckController deckControllerPrefab;
        [SerializeField] private TwoPlayerKey twoPlayerKeyPrefab;

        [Header("Scene objects")] [SerializeField]
        private GameObject leftGroundCardHolder;

        [SerializeField] private GameObject rightGroundCardHolder;

        [SerializeField] private GameObject leftDrawDeck;
        [SerializeField] private GameObject rightDrawDeck;

        [SerializeField] private Transform topHand;
        [SerializeField] private Transform bottomHand;

        [SerializeField] private GameObject topPlayerDeck;
        [SerializeField] private GameObject bottomPlayerDeck;

        [SerializeField] private GameObject numberOfCardsTextBottom;
        [SerializeField] private GameObject numberOfCardsTextTop;

        [NonSerialized] public Player bottomPlayer, topPlayer;
        [NonSerialized] public TwoPlayerKey twoPlayerKey;
        public bool isGameOver;

        private GameObject bottomPlayerObject, topPlayerObject;
        private DeckController deckController;
        private KeyValuePair<Card, Card> lastSpeed;
        private UIInteractions uiInteractions;

        private readonly Vector3 referenceCardScale = new Vector3(0.6499999f, 1.17f, 1f);
        private const float CooldownTime = 2f;
        private const int NumberOfDecks = 1;
        private const int NumberOfSides = 2;
        private const int NumberOfCardsOnSides = 4;
        private const int NumberOfCardsInTheMiddle = 2;
        private const int ReferenceScreenHeight = 2160;


        private void Awake()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;

            uiInteractions = GetComponent<UIInteractions>();

            bottomPlayerObject = Instantiate(playerPrefab);
            bottomPlayerObject.name = "Bottom Player";
            bottomPlayer = bottomPlayerObject.GetComponent<Player>();
            bottomPlayer.playSide = PlaySide.Bottom;
            bottomPlayer.SetHand(bottomHand);
            bottomPlayer.SetGameControl(this);
            bottomPlayer.SetNumberOfCardsText(numberOfCardsTextBottom);

            topPlayerObject = Instantiate(playerPrefab);
            topPlayerObject.name = "Top Player";
            topPlayer = topPlayerObject.GetComponent<Player>();
            topPlayer.playSide = PlaySide.Top;
            topPlayer.SetHand(topHand);
            topPlayer.SetGameControl(this);
            topPlayer.SetNumberOfCardsText(numberOfCardsTextTop);

            deckController = Instantiate(deckControllerPrefab);
            deckController.CreateADeck();


            twoPlayerKey = Instantiate(twoPlayerKeyPrefab);
            twoPlayerKey.SetPlayers(bottomPlayer, topPlayer);

            lastSpeed = new KeyValuePair<Card, Card>();

            DealSideCards();
            DealPlayerDecks();
            DealPlayerHands();
            DealMiddleCards();
        }

        private void Update()
        {
            if (isGameOver) return;

            if (!bottomPlayer.HasCards)
                StartCoroutine(GameOver(bottomPlayer));
            else if (!topPlayer.HasCards)
                StartCoroutine(GameOver(topPlayer));
        }

        private void DealSideCards()
        {
            var sideCardCount = NumberOfDecks * NumberOfSides * NumberOfCardsOnSides +
                                NumberOfCardsInTheMiddle;

            for (int i = 0; i < sideCardCount; i++)
            {
                var cardConfig = deckController.DrawRandomCard();
                var card = CreateCardObject(cardConfig, false);

                if (i < 5)
                {
                    card.transform.SetParent(leftDrawDeck.transform);
                    card.transform.position = leftDrawDeck.transform.position;
                }
                else
                {
                    card.transform.SetParent(rightDrawDeck.transform);
                    card.transform.position = rightDrawDeck.transform.position;
                }
            }
        }

        private void DealPlayerDecks()
        {
            int remainingCardCount = deckController.deck.Count;
            for (int i = 0; i < remainingCardCount; i++)
            {
                var cardConfig = deckController.DrawRandomCard();
                var card = CreateCardObject(cardConfig);

                if (i < remainingCardCount / 2)
                {
                    topPlayer.AddCardToDeck(card);
                    card.transform.SetParent(topPlayerDeck.transform);
                    card.transform.position = topPlayerDeck.transform.position;
                    card.GetComponent<Card>().AdjustRotation();
                }
                else
                {
                    bottomPlayer.AddCardToDeck(card);
                    card.transform.SetParent(bottomPlayerDeck.transform);
                    card.transform.position = bottomPlayerDeck.transform.position;
                }
            }
        }

        private void DealPlayerHands()
        {
            bottomPlayer.DrawCard(4);
            topPlayer.DrawCard(4);
        }

        private Vector3 GetScale()
        {
            float currentHeight = Screen.height;
            float ratio = currentHeight / ReferenceScreenHeight;
            return referenceCardScale * ratio;
        }

        public void DealMiddleCards()
        {
            var topPlayerDrawDeckTransform = leftDrawDeck.transform;
            var bottomPlayerDrawDeckTransform = rightDrawDeck.transform;

            int topPlayerDrawCardsCount = topPlayerDrawDeckTransform.childCount;
            int bottomPlayerDrawCardsCount = bottomPlayerDrawDeckTransform.childCount;
            if (topPlayerDrawCardsCount > 0 && bottomPlayerDrawCardsCount > 0)
            {
                var leftGroundCard = topPlayerDrawDeckTransform.GetChild(topPlayerDrawCardsCount - 1).gameObject;
                leftGroundCard.GetComponent<Card>().SlideToMiddle(leftGroundCardHolder.transform);

                var rightGroundCard =
                    bottomPlayerDrawDeckTransform.GetChild(bottomPlayerDrawCardsCount - 1).gameObject;
                rightGroundCard.GetComponent<Card>().SlideToMiddle(rightGroundCardHolder.transform);
            }
            else
            {
                StartCoroutine(SideCardsEmpty());
            }
        }

        private IEnumerator SideCardsEmpty()
        {
            var leftGroundCardHolderTransform = leftGroundCardHolder.transform;
            var rightGroundCardHolderTransform = rightGroundCardHolder.transform;

            int leftGroundCount = leftGroundCardHolderTransform.childCount;
            int rightGroundCount = rightGroundCardHolderTransform.childCount;

            for (int i = leftGroundCount - 1; i >= 0; i--)
            {
                var card = leftGroundCardHolderTransform.GetChild(i).gameObject;
                card.GetComponent<Card>().SlideToMiddle(leftDrawDeck.transform);
            }

            for (int i = rightGroundCount - 1; i >= 0; i--)
            {
                var card = rightGroundCardHolderTransform.GetChild(i).gameObject;
                card.GetComponent<Card>().SlideToMiddle(rightDrawDeck.transform);
            }

            do
            {
                yield return new WaitForEndOfFrame();
            } while (rightDrawDeck.transform.GetChild(0).GetComponent<Card>().isSliding ||
                     leftDrawDeck.transform.GetChild(0).GetComponent<Card>().isSliding);

            DealMiddleCards();
        }


        private GameObject CreateCardObject(Card cardConfig, bool withButton = true)
        {
            var card = Instantiate(cardPrefab);
            card.GetComponent<RectTransform>().localScale = GetScale();
            card.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Cards/{cardConfig.cardName}");

            var cardComponent = card.AddComponent<Card>();
            card.name = cardConfig.cardName;
            cardComponent.id = cardConfig.id;
            cardComponent.cardName = cardConfig.cardName;
            cardComponent.suit = cardConfig.suit;
            cardComponent.value = cardConfig.value;
            cardComponent.SetGameControl(this);

            if (!withButton) return card;

            var button = card.AddComponent<Button>();
            button.onClick.AddListener(delegate { cardComponent.CardClick(); });

            return card;
        }


        public bool CheckIfSpeedIsValid()
        {
            var leftCardObject = leftGroundCardHolder.transform.GetChild(leftGroundCardHolder.transform.childCount - 1)
                .transform;
            var leftCard = leftCardObject.GetComponent<Card>();
            int leftCardValue = leftCard.value;

            var rightCardObject = rightGroundCardHolder.transform
                .GetChild(rightGroundCardHolder.transform.childCount - 1)
                .transform;
            var rightCard = rightCardObject.GetComponent<Card>();
            int rightCardValue = rightCard.value;

            if (!IsSpeedUnique(leftCard, rightCard)) return false;
            return leftCardValue == rightCardValue;
        }

        private bool IsSpeedUnique(Card leftCard, Card rightCard)
        {
            if (lastSpeed.Equals(new KeyValuePair<Card, Card>())) return true;

            var oldSpeedLeftCard = lastSpeed.Key;
            var oldSpeedRightCard = lastSpeed.Value;

            if (leftCard.Equals(oldSpeedLeftCard) && rightCard.Equals(oldSpeedRightCard))
                return false;

            return true;
        }

        public void SpeedCooldown(Player player)
        {
            player.Cooldown(CooldownTime);
        }

        public void SpeedEvent(Player fromThisPlayer, Player toThisPlayer)
        {
            UpdateLastSpeed();
            fromThisPlayer.GiveCards(toThisPlayer);
        }

        private void UpdateLastSpeed()
        {
            var rightGroundCardHolderTransform = rightGroundCardHolder.transform;
            var leftGroundCardHolderTransform = leftGroundCardHolder.transform;
            var leftGroundChildCount = leftGroundCardHolderTransform.childCount;
            var rightGroundChildCount = rightGroundCardHolderTransform.childCount;

            if (rightGroundChildCount <= 0 || leftGroundChildCount <= 0)
                return;

            var leftCard = leftGroundCardHolderTransform.GetChild(leftGroundChildCount - 1)
                .transform.GetComponent<Card>();
            var rightCard = rightGroundCardHolderTransform.GetChild(rightGroundChildCount - 1)
                .transform.GetComponent<Card>();

            lastSpeed = new KeyValuePair<Card, Card>(leftCard, rightCard);
        }

        public IEnumerator GameOver(Player winner)
        {
            isGameOver = true;
            IncreaseScore(winner);

            yield return new WaitForSeconds(1);

            uiInteractions.UpdateWinnerText(winner.playSide);
            uiInteractions.UpdateScores();
            gameOverPanel.SetActive(true);
            gamePanel.SetActive(false);
        }

        private static void IncreaseScore(Player winner)
        {
            string key = $"{winner.playSide}PlayerScore";
            int score = PlayerPrefs.GetInt(key);
            score++;
            PlayerPrefs.SetInt(key, score);
        }

        public GameObject GetTargetMiddleHolder(GameObject cardObject)
        {
            var card = cardObject.GetComponent<Card>();

            var leftCard = leftGroundCardHolder.transform.GetChild(leftGroundCardHolder.transform.childCount - 1)
                .GetComponent<Card>();
            var rightCard = rightGroundCardHolder.transform.GetChild(rightGroundCardHolder.transform.childCount - 1)
                .GetComponent<Card>();

            GameObject target = null;

            if (IsCardMatching(card, leftCard))
                target = leftGroundCardHolder;
            else if (IsCardMatching(card, rightCard))
                target = rightGroundCardHolder;

            if (target != null)
            {
                twoPlayerKey.ResetKeys();
            }

            return target;
        }

        private bool IsCardMatching(Card children, Card parent)
        {
            bool isChildrenAce = children.value == 1;
            bool isChildrenK = children.value == 13;

            bool isParentAce = parent.value == 1;
            bool isParentK = parent.value == 13;

            if (isChildrenAce && isParentK)
                return true;

            if (isChildrenK && isParentAce)
                return true;

            if (children.value == parent.value - 1)
                return true;

            if (children.value == parent.value + 1)
                return true;

            return false;
        }
    }
}