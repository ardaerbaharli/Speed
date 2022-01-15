using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class PopupController : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image tinyBackground;

        private readonly float fadeTime = 0.5f;

        private void OnEnable()
        {
            int index = Random.Range(0, 7);
            tinyBackground.sprite = Resources.Load<Sprite>($"Backgrounds/BG{index}");

            background.DOFade(0.4f, fadeTime);
        }
    }
}