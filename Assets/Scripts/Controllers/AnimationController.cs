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
        Transform targetHand = toWho.playSide == PlaySide.Bottom ? GameControl.instance.bottomHand.transform : GameControl.instance.topHand.transform;
        var parent = cardObjects.First().transform.parent;
        int childCount = parent.childCount;

        foreach (var card in cardObjects)
        {
            instance.StartCoroutine(Slide(card, targetHand, GameSettings.slidingSpeed, false));
        }

        do
        {
            yield return null;

        } while (parent.childCount != childCount - cardObjects.Count);

        LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
    }

    public static IEnumerator SlideToMiddle(GameObject card, Transform target)
    {
        yield return instance.StartCoroutine(Slide(card, target, GameSettings.slidingSpeed));
    }

    private static IEnumerator Slide(GameObject card, Transform targetParent, float time, bool a = true)
    {
        if (!card.GetComponent<Card>().IsSliding)
        {
            if (a)
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
            if(!a)
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
