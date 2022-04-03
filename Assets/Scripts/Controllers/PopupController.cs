using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Speed.Controllers
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] private Image background;

        [SerializeField] private float fadeTime = 0.5f;
        [SerializeField] private float endValue = 0.4f;


        private void OnEnable()
        {
            print("OnEnable");
            StartCoroutine(FadeIn(background, endValue, fadeTime));
            // background.DOFade(endValue, fadeTime);
        }

        private IEnumerator FadeIn(Image img, float to, float duration)
        {
            var c = img.color;
            var current = c.a;
            var totalDelta = 0f;
            while (to - current > 0.01f)
            {
                totalDelta += Time.deltaTime;
                current += totalDelta / duration * to;
                img.color = new Color(c.r, c.g, c.b, current);
                yield return null;
            }
        }

        private IEnumerator FadeOut(Image img, float to, float duration)
        {
            var c = img.color;
            var current = c.a;
            var totalDelta = 0f;
            while (current - to > 0.01f)
            {
                totalDelta += Time.deltaTime;
                current -= totalDelta / duration;
                img.color = new Color(c.r, c.g, c.b, current);
                yield return null;
            }

            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            print("OnDisable");
            background.color = new Color(background.color.r, background.color.g, background.color.b, 0);
            // StartCoroutine(Fade(background, 0, fadeTime));
        }

        public void Deactivate()
        {
            StartCoroutine(FadeOut(background, 0, fadeTime));
        }
    }
}