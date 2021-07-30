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
    /// <summary>
    /// Slide the given card to given player.
    /// </summary>
    /// <param name="card">Card to slide</param>
    /// <param name="toWho">Player to slide</param>
    /// <returns></returns>
    public static IEnumerator SlideTo(GameObject card, Player toWho)
    {
        Transform targetHand = toWho.playSide == PlaySide.Bottom ? GameControl.instance.bottomHand.transform : GameControl.instance.topHand.transform;

        var parent = card.transform.parent;
        int childCount = card.transform.parent.childCount;

        var pos = GetPosition(card, targetHand);

        instance.StartCoroutine(Slide(card, targetHand.transform, pos, GameSettings.slidingSpeed));

        do
        {
            yield return null;

        } while (parent.childCount != childCount - 1);

        DestroyDummies();


        while (parent.name == card.transform.parent.name)
            yield return null;


        //if (targetParent.name.Contains("Panel"))
        //    targetParent.transform.GetComponent<VerticalLayoutGroup>().spacing = GameControl.CalculateSpacing(targetParent);
    }


    /// <summary>
    /// Slide the given cards to given player.
    /// </summary>
    /// <param name="cards">Cards to slide</param>
    /// <param name="toWho">Player to slide</param>
    /// <returns></returns>
    public static IEnumerator SlideTo(List<GameObject> cards, Player toWho)
    {
        Transform targetHand = toWho.playSide == PlaySide.Bottom ? GameControl.instance.bottomHand.transform : GameControl.instance.topHand.transform;

        var parent = cards.First().transform.parent;
        int childCount = cards.First().transform.parent.childCount;

        var pos = GetPositions(targetHand, cards.Count);
        for (int i = 0; i < pos.Count; i++)
        {
            instance.StartCoroutine(Slide(cards[i], targetHand.transform, pos[i], GameSettings.slidingSpeed));
        }

        DestroyDummies();

        do
        {
            yield return null;

        } while (parent.childCount != childCount - cards.Count);



        //while (parent.name == card.transform.parent.name)
        //    yield return null;


        //if (targetParent.name.Contains("Panel"))
        //    targetParent.transform.GetComponent<VerticalLayoutGroup>().spacing = GameControl.CalculateSpacing(targetParent);
    }



    /// <summary>
    /// Method to create sliding animation
    /// </summary>
    /// <param name="card">Sliding card</param>
    /// <param name="targetParent">New parent for the given card</param>
    /// <param name="position">Position that the card will have at the end of the sliding animation</param>
    /// <param name="time">Time that sliding animation will take</param>
    /// <returns></returns>
    private static IEnumerator Slide(GameObject card, Transform targetParent, Vector3 position, float time)
    {
        card.GetComponent<Card>().IsSliding = true;
        card.GetComponent<Card>().IsDummy = true;

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
        SetParent(card, targetParent);
    }
   
    public static Vector3 GetPosition(GameObject card, Transform targetParent)
    {
        card.GetComponent<Card>().IsSliding = true;

        var positionDummy = Instantiate(GameControl.instance.cardPrefab, targetParent) as GameObject;
        positionDummy.AddComponent<Card>();
        positionDummy.GetComponent<Card>().IsDummy = true;
        positionDummy.tag = "dummy";
        positionDummy.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        //if (targetParent.parent.name.Contains("Panel"))
        //    targetParent.transform.GetComponent<VerticalLayoutGroup>().spacing = GameControl.CalculateSpacing(targetParent, 1);

        LayoutRebuilder.ForceRebuildLayoutImmediate(targetParent.GetComponent<RectTransform>());

        var pos = positionDummy.transform.position;
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

        //if (targetParent.parent.name.Contains("Panel"))
        //    targetParent.transform.GetComponent<VerticalLayoutGroup>().spacing = GameControl.CalculateSpacing(targetParent, 1);

        LayoutRebuilder.ForceRebuildLayoutImmediate(targetParent.GetComponent<RectTransform>());

        var positions = new List<Vector3>();
        foreach (var positionDummy in posDummies)
        {
            var pos = positionDummy.transform.position;
            positions.Add(pos);
        }
        return positions;
    }
    private static void SetParent(GameObject card, Transform targetParent)
    {
        var parent = card.transform.parent;
        card.transform.SetParent(targetParent);

        //if (parent.name.Contains("Panel"))
        //    parent.GetComponent<VerticalLayoutGroup>().spacing = GameControl.CalculateSpacing(parent); // set the spacing for the panel layout

        card.GetComponent<Canvas>().overrideSorting = false;
        card.GetComponent<Card>().IsSliding = false;
        card.GetComponent<Card>().IsDummy = false;

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
