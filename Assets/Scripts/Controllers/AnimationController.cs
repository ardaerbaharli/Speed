using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    private static AnimationController instance;
    private void Awake()
    {
        instance = this;
    }

    public static IEnumerator SlideToHand(List<GameObject> cardObjects, Player toWho)
    {
        var targetHand = toWho.PlaySide == PlaySide.Bottom ? GameControl.instance.bottomHand.transform : GameControl.instance.topHand.transform;

        foreach (var card in cardObjects)
        {
            card.GetComponent<Canvas>().overrideSorting = true;
            card.GetComponent<Canvas>().sortingOrder = -1;
            card.transform.SetParent(targetHand);

            card.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
            card.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        }

        instance.StartCoroutine(AlignCards(targetHand));

        yield return null;
    }

    public static IEnumerator AlignCards(Transform hand)
    {
        if (hand.childCount > 0)
        {
            float leftBorderX = 0f;
            float rightBorderX = Screen.width;

            int cardCount = hand.transform.childCount;

            var cardSize = new Vector3(hand.transform.GetChild(0).GetComponent<RectTransform>().rect.width, 0, 0);

            var leftborder = new Vector3(leftBorderX, hand.transform.position.y, 0) + cardSize / 4;
            var rightBorder = new Vector3(rightBorderX, hand.transform.position.y, 0) - cardSize / 4;

            var length = rightBorder - leftborder;
            var distanceBetweenCards = length / (cardCount + 1);

            var pos = leftborder;
            for (int i = 0; i < cardCount; i++)
            {
                if (hand.transform.childCount < cardCount)
                    break;

                pos += distanceBetweenCards;
                var card = hand.transform.GetChild(i);

                float seconds = GameSettings.dealSlideTime;
                float t = 0f;
                while (t <= 1.0)
                {
                    t += Time.deltaTime / seconds;
                    card.transform.position = Vector3.Lerp(card.transform.position, pos, Mathf.SmoothStep(0f, 1f, t));
                    yield return null;
                }
                card.GetComponent<Canvas>().overrideSorting = false;
            }
        }
    }

    public static IEnumerator SlideToMiddle(GameObject card, Transform target)
    {
        yield return instance.StartCoroutine(Slide(card, target, GameSettings.slowSlideTime));
    }

    private static IEnumerator Slide(GameObject card, Transform targetParent, float time, bool setParent = true)
    {
        if (!card.GetComponent<Card>().IsSliding)
        {
            if (setParent)
                card.transform.SetParent(targetParent);

            var position = targetParent.transform.position;
            card.GetComponent<Card>().IsSliding = true;

            card.GetComponent<Canvas>().overrideSorting = true;
            card.GetComponent<Canvas>().sortingOrder = 2;

            float seconds = time;
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                card.transform.position = Vector3.Lerp(card.transform.position, position, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            card.GetComponent<Canvas>().overrideSorting = false;
            card.GetComponent<Card>().IsSliding = false;

            if (!setParent)
                card.transform.SetParent(targetParent);
        }
    }

    public static IEnumerator FlipVertically(GameObject card, float time, float rotateAngle)
    {
        var v = new Vector3(0, 0, rotateAngle);
        var q = Quaternion.Euler(v);

        float seconds = time;
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            card.GetComponent<RectTransform>().rotation = Quaternion.Lerp(card.GetComponent<RectTransform>().rotation, q, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
    }
}
