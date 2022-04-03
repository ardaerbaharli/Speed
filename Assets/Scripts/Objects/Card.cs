using System.Collections;
using Speed.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Speed.Objects
{
    public class Card : MonoBehaviour
    {
        public Player player;

        public bool isSliding;
        public string cardName;
        public int id;
        public int value;
        public string suit;

        private GameControl gameControl;
        public void SetGameControl(GameControl gc) => gameControl = gc;


        public bool Equals(Card b)
        {
            return cardName == b.cardName &&
                   value == b.value &&
                   suit == b.suit &&
                   id == b.id;
        }

        public void CardClick()
        {
            if (gameControl.isGameOver) return;

            var target = gameControl.GetTargetMiddleHolder(gameObject);

            if (target == null) return;

            Destroy(GetComponent<Button>());
            SlideToMiddle(target.transform);

            player.RemoveCardFromHand(gameObject);
            player.DrawCard(1);
            player.AlignCards();
        }

        public float slowSlideTime = 1f;

        public void SlideToMiddle(Transform targetTransform)
        {
            StartCoroutine(Slide(targetTransform, slowSlideTime));
        }

        private IEnumerator Slide(Transform targetParent, float time, bool setParent = true)
        {
            if (isSliding) yield break;

            if (setParent)
                transform.SetParent(targetParent);

            var position = targetParent.transform.position;
            isSliding = true;

            float seconds = time;
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                transform.position =
                    Vector3.Lerp(transform.position, position, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            GetComponent<Canvas>().overrideSorting = false;
            isSliding = false;

            if (!setParent)
                transform.SetParent(targetParent);
        }

        public IEnumerator FlipVertically(float time, float rotateAngle)
        {
            var v = new Vector3(0, 0, rotateAngle);
            var q = Quaternion.Euler(v);

            float seconds = time;
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                gameObject.GetComponent<RectTransform>().rotation =
                    Quaternion.Lerp(gameObject.GetComponent<RectTransform>().rotation, q, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

        public void AdjustRotation()
        {
            var targetAngle = player.playSide == PlaySide.Top ? 180 : 0;

            var v = new Vector3(0, 0, targetAngle);
            var q = Quaternion.Euler(v);
            gameObject.GetComponent<RectTransform>().rotation = q;
        }
    }
}