using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Speed.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Speed.Objects
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private int remainingCardCount;
        [SerializeField] private List<GameObject> handCards;
        [SerializeField] private List<GameObject> deck;

        public PlaySide playSide;
        public bool isInCooldown;
        public float cooldownTime;
        public bool pressedDrawButton;

        private const float DealSlideTime = 0.1f;
        private GameObject numberOfCardsText;
        private float leftBorderX, rightBorderX;
        private Vector3 cardArea;
        private Vector2 anchor;
        private GameControl gameControl;
        private Transform hand;

        public bool HasCards => _HasCards();
        public void SetHand(Transform h) => hand = h;
        public void SetGameControl(GameControl gc) => gameControl = gc;
        public void SetNumberOfCardsText(GameObject numberOfCardsText) => this.numberOfCardsText = numberOfCardsText;

        private void Awake()
        {
            numberOfCardsText = GameObject.Find("RemainingCardCount" + playSide);

            handCards = new List<GameObject>();
            deck = new List<GameObject>();
            leftBorderX = 0f;
            rightBorderX = Screen.width;
            cardArea = Vector3.zero;
            anchor = new Vector2(0.5f, 0.5f);
        }

        private void AddCardToHand(GameObject card)
        {
            handCards.Add(card);
        }

        public void RemoveCardFromHand(GameObject card)
        {
            handCards.Remove(card);
        }

        public void AddCardToDeck(GameObject card)
        {
            card.GetComponent<Card>().player = this;
            deck.Add(card);
        }

        private void RemoveCardFromDeck(GameObject card)
        {
            deck.Remove(card);
        }

        private void UpdateRemainingCardCountText()
        {
            numberOfCardsText.GetComponent<Text>().text = remainingCardCount.ToString();
        }

        private void Update()
        {
            if (gameControl.isGameOver) return;

            if (isInCooldown)
            {
                if (cooldownTime > 0)
                    cooldownTime -= Time.deltaTime;
                else
                    isInCooldown = false;
            }

            remainingCardCount = handCards.Count + deck.Count;
            UpdateRemainingCardCountText();
        }

        public void Cooldown(float time)
        {
            isInCooldown = true;
            cooldownTime = time;
        }

        private bool _HasCards()
        {
            return deck.Count > 0 || handCards.Count > 0;
        }

        public void GiveCards(Player toThisPlayer)
        {
            if (handCards.Count <= 0) return;

            foreach (var card in handCards.ToList())
            {
                var cardComponent = card.GetComponent<Card>();
                cardComponent.player = toThisPlayer;
                cardComponent.AdjustRotation();
                RemoveCardFromHand(card);
                toThisPlayer.AddCardToHand(card);
                toThisPlayer.SlideCardToPlayerHand(card);
            }

            DrawCard(4);
        }


        public void SlideCardsToPlayerHand(List<GameObject> cardObjects)
        {
            foreach (var card in cardObjects)
            {
                var cardCanvas = card.GetComponent<Canvas>();
                cardCanvas.overrideSorting = true;
                cardCanvas.sortingOrder = -1;
                card.transform.SetParent(hand);

                var cardRectTransform = card.GetComponent<RectTransform>();
                cardRectTransform.anchorMin = anchor;
                cardRectTransform.anchorMax = anchor;
            }

            AlignCards();
        }

        public void SlideCardToPlayerHand(GameObject card)
        {
            var cardCanvas = card.GetComponent<Canvas>();
            cardCanvas.overrideSorting = true;
            cardCanvas.sortingOrder = -1;
            card.transform.SetParent(hand);

            var cardRectTransform = card.GetComponent<RectTransform>();
            cardRectTransform.anchorMin = anchor;
            cardRectTransform.anchorMax = anchor;

            AlignCards();
        }


        public void AlignCards()
        {
            StartCoroutine(AlignCardsCoroutine());
        }

        public IEnumerator AlignCardsCoroutine()
        {
            if (hand.childCount <= 0) yield break;

            var handTransform = hand.transform;

            if (cardArea == Vector3.zero)
                cardArea = new Vector3(handTransform.GetChild(0).GetComponent<RectTransform>().rect.width, 0, 0);

            int cardCount = handTransform.childCount;
            var handTransformPosition = handTransform.position;
            var leftBorderPos = new Vector3(leftBorderX, handTransformPosition.y, 0) + cardArea / 4;
            var rightBorderPos = new Vector3(rightBorderX, handTransformPosition.y, 0) - cardArea / 4;
            var handArea = rightBorderPos - leftBorderPos;

            var distanceBetweenCards = handArea / (cardCount + 1);

            var pos = leftBorderPos;
            for (int i = 0; i < cardCount; i++)
            {
                if (handTransform.childCount < cardCount)
                    break;

                pos += distanceBetweenCards;
                var card = handTransform.GetChild(i);
                float seconds = DealSlideTime;
                float t = 0f;
                while (t <= 1.0)
                {
                    t += Time.deltaTime / seconds;
                    card.transform.position =
                        Vector3.Lerp(card.transform.position, pos, Mathf.SmoothStep(0f, 1f, t));
                    yield return null;
                }

                card.GetComponent<Canvas>().overrideSorting = false;
                yield return null;
            }
        }

        public void DrawCard(int numberOfCards)
        {
            if (!HasCards)
            {
                StartCoroutine(gameControl.GameOver(this));
            }
            else if (handCards.Count < 4 && deck.Count != 0)
            {
                if (deck.Count < numberOfCards)
                    numberOfCards = deck.Count;

                for (int i = 0; i < numberOfCards; i++)
                {
                    var card = deck.Last();
                    if (playSide == PlaySide.Top)
                        card.GetComponent<Card>().AdjustRotation();
                    RemoveCardFromDeck(deck.Last());
                    AddCardToHand(card);
                    SlideCardToPlayerHand(card);
                }
            }
        }
    }
}