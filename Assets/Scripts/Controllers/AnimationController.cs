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

    public static IEnumerator SlideTo(List<GameObject> cards, Player toWho)
    {
        Transform targetHand = toWho.playSide == PlaySide.Bottom ? GameControl.instance.bottomHand.transform : GameControl.instance.topHand.transform;
        var parent = cards.First().transform.parent;
        int childCount = cards.First().transform.parent.childCount;

        var pos = GetPositions(targetHand, cards.Count);
        for (int i = 0; i < pos.Count; i++)
        {
            instance.StartCoroutine(Slide(cards[i], targetHand.transform, pos[i], GameSettings.slidingSpeed));
            float rAngle;
            if (toWho.playSide == PlaySide.Top) rAngle = 180f;
            else rAngle = 0;
            instance.StartCoroutine(FlipVertically(cards[i], GameSettings.slidingSpeed, rAngle));
        }

        do
        {
            yield return null;

        } while (parent.childCount != childCount - cards.Count);
    }

    public static IEnumerator SlideTo(GameObject card, GameObject target)
    {
        var parent = card.transform.parent;
        var childCount = parent.childCount;

        var pos = GetPositions(target.transform,1).First();
        instance.StartCoroutine(Slide(card, target.transform, pos, GameSettings.slidingSpeed));

        do
        {
            yield return null;

        } while (parent.childCount != childCount - 1);
    }


    private static IEnumerator Slide(GameObject card, Transform targetParent, Vector3 position, float time)
    {
        if (!card.GetComponent<Card>().IsSliding)
        {
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


            DestroyDummies();

            card.transform.SetParent(targetParent);

            card.GetComponent<Canvas>().overrideSorting = false;
            card.GetComponent<Card>().IsSliding = false;
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

    public static Vector3 GetPosition(Transform targetParent)
    {
        var positionDummy = Instantiate(GameControl.instance.cardPrefab, targetParent) as GameObject;
        positionDummy.AddComponent<Card>();
        positionDummy.GetComponent<Card>().IsDummy = true;
        positionDummy.tag = "dummy";
        positionDummy.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        LayoutRebuilder.ForceRebuildLayoutImmediate(targetParent.GetComponent<RectTransform>());

        var pos = positionDummy.transform.position;
        Debug.Log(positionDummy.transform.parent.transform.position);
        return pos;
    }

    public static List<Vector3> GetPositions(Transform targetParent, int count)
    {
        var posDummies = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var positionDummy = Instantiate(GameControl.instance.cardPrefab, targetParent) as GameObject;
            positionDummy.AddComponent<Card>();
            positionDummy.GetComponent<Card>().IsDummy = true;
            positionDummy.tag = "dummy";
            positionDummy.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            posDummies.Add(positionDummy);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(targetParent.parent.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetParent.GetComponent<RectTransform>());

        var positions = new List<Vector3>();
        foreach (var positionDummy in posDummies)
        {
            var pos = positionDummy.transform.position;
            positions.Add(pos);
        }
        return positions;
    }

    private static void DestroyDummies()
    {
        var dummies = GameObject.FindGameObjectsWithTag("dummy");

        foreach (var dummy in dummies)
        {
            DestroyImmediate(dummy);
        }
    }
}
